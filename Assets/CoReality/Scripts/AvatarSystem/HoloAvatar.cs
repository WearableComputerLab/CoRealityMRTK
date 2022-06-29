using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.Events;
using DisplayProp = CoReality.Avatars.AvatarPeripheralVisualiser.DisplayProp;
using OZMap;
using Mapbox.Utils;
using System.Linq;

namespace CoReality.Avatars
{

    /// <summary>
    /// Handles both local and remote user avatar representations, including head and 
    /// controller/hand tracking and transmission. 
    /// 
    /// Modified for OZMap by Jack Fraser:
    /// - Extrapolated hand/controller tracking.
    /// - Added AvatarBehaviour component system.
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class HoloAvatar : MonoBehaviourPun, IPunObservable
    {
        public enum ControllerType { Hands, Controller }

        private bool _isInitalized = false;

        private bool _local = true;

        /// <summary>
        /// Is this HoloAvatar local or remote?
        /// </summary>
        /// <value></value>
        public bool IsLocal
        {
            get => _local;
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value + (IsLocal ? " (you)" : "");
                PropertyChanged(nameof(Name), Name);

                //RPC to update color to others if locally changed
                if (IsLocal)
                {
                    photonView.RPC(
                        nameof(SetNameRPC),
                        RpcTarget.OthersBuffered,
                        value
                    );
                }
                else
                {
                    //If remote set the name text above their head
                    _head.Name = value;
                }
            }
        }

        [SerializeField]
        private Color _color;

        /// <summary>
        /// Sets the color of this HoloAvatar, and updates on all clients
        /// </summary>
        /// <value></value>
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                PropertyChanged(nameof(Color), Color);

                //RPC to update color to others if locally changed
                if (IsLocal)
                {
                    photonView.RPC(
                        nameof(SetColorRPC),
                        RpcTarget.OthersBuffered,
                        new Vector3(value.r, value.g, value.b)
                    );
                }
                else
                {
                    //If remote update the meshes
                    _handVisualiser.SetDisplayProperty(DisplayProp.Color, value);
                    _head.GetComponentInChildren<MeshRenderer>().materials[1].color = value;
                    foreach (AvatarNetBehaviour beh in _avatarBehaviours) beh.SetAvatarColor(value);
                }
            }
        }

        //---------------------------------

        private PhotonStreamQueue _streamQueue = new PhotonStreamQueue(120);

        //---------------------------------

        [SerializeField] private ControllerType _controllerType;
        public ControllerType HandControllerType
        {
            get => _controllerType;
        }

        //Reference objects for local player 
        private GameObject _headRef, _lHandRef, _rHandRef;


        //Remote avatar objects, only created for remote HoloAvatars

        [SerializeField]
        private AvatarHead _headPrefab;
        private AvatarHead _head;

        public AvatarHead Head
        {
            get => _head;
        }

        /// <summary>
        /// Gets the current head position of this
        /// avatar in local-space
        /// (Its location relative to the networked origin)
        /// </summary>
        public Vector3 HeadLocalPosition
        {
            get
            {
                if (_local)
                    return _headRef.transform.localPosition;
                else
                    return _head.transform.localPosition;
            }
        }

        public Vector3 HeadWorldPos
        {
            get
            {
                if (_local)
                    return _headRef.transform.position;
                else
                    return _head.transform.position;
            }
        }
        /// <summary>
        /// Reference to the component that will handle visualisation/synced view
        /// of the avatar's hands or controllers.
        /// </summary>
        private AvatarPeripheralVisualiser _handVisualiser;

        [Header("Hand Visualiser Prefabs")]
        [SerializeField] private AvatarRiggedHands _riggedHandsVisPrefab;
        [SerializeField] private AvatarControllers _controllersVisPrefab;

        public AvatarPeripheralVisualiser HandVisualiser { get => _handVisualiser; }

        //---------------------------------------------

        private PropertyChangedEvent _onPropertyChanged = new PropertyChangedEvent();

        public PropertyChangedEvent OnPropertyChanged
        {
            get => _onPropertyChanged;
        }

        private void PropertyChanged(string property, object value)
        {
            _onPropertyChanged?.Invoke(property, value);
        }


        private bool _sendDelay = false;

        private List<AvatarNetBehaviour> _avatarBehaviours = new List<AvatarNetBehaviour>();

        void Awake()
        {
        }

        void Start()
        {

        }

        /// <summary>
        /// Initalize this HoloAvatar
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="parent"></param>
        /// <param name="color"></param>
        public HoloAvatar Initalize(bool remote, ControllerType cType = ControllerType.Hands)
        {
            _local = !remote;

            _controllerType = cType;

            _avatarBehaviours.AddRange(GetComponents<AvatarNetBehaviour>());
            _avatarBehaviours = _avatarBehaviours.OrderBy(x => x.ExecutionOrder).ToList();

            transform.SetParent(NetworkModule.NetworkOrigin);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = NetworkModule.NetworkOrigin.localScale;

            name = (remote ? "Remote" : "Local") + "Avatar";

            AvatarPeripheralVisualiser peripheralPrefab;
            if (_controllerType == ControllerType.Hands) peripheralPrefab = _riggedHandsVisPrefab;
            else peripheralPrefab = _controllersVisPrefab;

            if (IsLocal)
            {
                //Spawn the reference objects (for positioning)
                _headRef = new GameObject("__HeadReference");
                _headRef.transform.parent = NetworkModule.NetworkOrigin;

                _handVisualiser = Instantiate(peripheralPrefab, _headRef.transform);
                _handVisualiser.InitLocal(_headRef.transform);

                foreach (AvatarNetBehaviour behaviour in _avatarBehaviours)
                {
                    behaviour.InitLocal(this);
                }
            }
            else
            {
                //instantiate the remote objects for this avatar
                _head = Instantiate(_headPrefab, Vector3.zero, Quaternion.identity, transform);

                _handVisualiser = Instantiate(peripheralPrefab, _head.transform);
                _handVisualiser.InitRemote();

                foreach (AvatarNetBehaviour behaviour in _avatarBehaviours)
                {
                    behaviour.InitRemote(this);
                }
            }

            _isInitalized = true;

            return this;
        }

        IEnumerator SendDelayRoutine()
        {
            Debug.Log("Waiting for send delay");
            yield return new WaitForSecondsRealtime(10);
            Debug.Log("Send delay = true");
            _sendDelay = true;
        }

        void Update()
        {
            if (!_isInitalized) return;

            //Ensure in photon room else reset and return
            if (!PhotonNetwork.InRoom)
            {
                _streamQueue.Reset();
                return;
            }

            //Serialize data if owner, else deserialize it
            if (IsLocal && PhotonNetwork.CurrentRoom.PlayerCount > 0)
            {
                this.SerializeData();
            }
            else if (_streamQueue.HasQueuedObjects())
                this.DeserializeData();

        }

        private void SerializeData()
        {
            // HEAD
            _headRef.transform.position = Camera.main.transform.position;
            _headRef.transform.rotation = Camera.main.transform.rotation;

            _streamQueue.SendNext(_headRef.transform.localPosition);
            _streamQueue.SendNext(_headRef.transform.localRotation);

            // HANDS
            _handVisualiser.SerializeData(_streamQueue);

            // BEHAVIOURS 
            foreach (AvatarNetBehaviour behaviour in _avatarBehaviours)
            {
                behaviour.SerializeData(_streamQueue);
            }
        }

        private void DeserializeData()
        {
            // HEAD
            _head.transform.localPosition = (Vector3)_streamQueue.ReceiveNext();
            _head.transform.localRotation = (Quaternion)_streamQueue.ReceiveNext();

            // HANDS
            _handVisualiser.DeserializeData(_streamQueue);

            // BEHAVIOURS 
            foreach (AvatarNetBehaviour behaviour in _avatarBehaviours)
            {
                behaviour.DeserializeData(_streamQueue);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting == true)
            {
                this._streamQueue.Serialize(stream);
            }
            else
            {
                this._streamQueue.Deserialize(stream);
            }
        }

        public void Destroy()
        {
            //Clean up all listeners
            _onPropertyChanged.RemoveAllListeners();
            if (IsLocal)
            {
                Destroy(_headRef.gameObject);
                // Destroy(_lHandRef.gameObject);
                // Destroy(_rHandRef.gameObject);
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// Adds and registers a new AvatarNetBehaviour derived component to this HoloAvatar
        /// and GameObject.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddAvatarBehaviour<T>(int executionOrder = 0) where T : AvatarNetBehaviour
        {
            T behaviour = gameObject.AddComponent(typeof(T)) as T;
            _avatarBehaviours.Add(behaviour);
            _avatarBehaviours = _avatarBehaviours.OrderBy(x => x.ExecutionOrder).ToList();
            return behaviour;
        }

        /// <summary>
        /// Attempts to remove a given AvatarNetBehaviour if it exists on this HoloAvatar.
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public bool RemoveAvatarBehaviour(AvatarNetBehaviour behaviour)
        {
            if (_avatarBehaviours.Contains(behaviour))
            {
                _avatarBehaviours.Remove(behaviour);
                _avatarBehaviours = _avatarBehaviours.OrderBy(x => x.ExecutionOrder).ToList();
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Attempts to remove all registered AvatarNetBehaviours of type T if they exist on this
        /// HoloAvatar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool RemoveAvatarBehaviour<T>() where T : AvatarNetBehaviour
        {
            bool foundRemove = false;
            for (int i = _avatarBehaviours.Count - 1; i >= 0; i--)
            {
                AvatarNetBehaviour behaviour = _avatarBehaviours[i];
                if (behaviour is T)
                {
                    _avatarBehaviours.RemoveAt(i);
                    _avatarBehaviours = _avatarBehaviours.OrderBy(x => x.ExecutionOrder).ToList();
                    if (foundRemove)
                    {
                        Debug.LogWarning($"Multiple AvatarNetBehaviours of type {typeof(T).Name} found, removing...");
                    }
                    foundRemove = true;
                }
            }
            return foundRemove;
        }

        #region RPCs

        /// <summary>
        /// Remotely set the color of this player
        /// </summary>
        /// <param name="color"></param>
        [PunRPC]
        void SetColorRPC(Vector3 color)
        {
            Color = new Color(color.x, color.y, color.z);
        }

        /// <summary>
        /// Remotely sets the name of this player
        /// </summary>
        /// <param name="name"></param>
        [PunRPC]
        void SetNameRPC(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Remotely sets the scale of this player's avatar
        /// </summary>
        /// <param name="localScale"></param>
        [PunRPC]
        void SetScaleRPC(Vector3 localScale)
        {
            //Ensure we never set a local avatar's scale
            if (!IsLocal)
            {


            }
        }

        [PunRPC]
        void PropertyChangedRPC(string property, object value)
        {
            //TODO: Implement if I added any more property RPCs
        }

        #endregion

    }


    public class PropertyChangedEvent : UnityEvent<string, object> { }

}