using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.Events;

[RequireComponent(typeof(PhotonView))]
public class HoloAvatar : MonoBehaviourPun, IPunObservable
{
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

    private Color _color;
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
                _rightHand.MeshRenderer.material.color = value;
                _leftHand.MeshRenderer.material.color = value;
                _head.GetComponentInChildren<MeshRenderer>().material.color = value;
            }
        }
    }

    //---------------------------------

    private PhotonStreamQueue _streamQueue = new PhotonStreamQueue(120);

    //---------------------------------

    private GameObject _headRef, _lHandRef, _rHandRef;

    [SerializeField]
    private AvatarHead _headPrefab;
    private AvatarHead _head;

    public AvatarHead Head
    {
        get => _head;
    }

    [SerializeField]
    private AvatarRiggedHand _lHandPrefab;
    private AvatarRiggedHand _leftHand;

    public AvatarRiggedHand LeftHand
    {
        get => _leftHand;
    }

    [SerializeField]
    private AvatarRiggedHand _rHandPrefab;
    private AvatarRiggedHand _rightHand;

    public AvatarRiggedHand RightHand
    {
        get => _rightHand;
    }

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
    public HoloAvatar Initalize(bool remote)
    {
        _local = !remote;

        transform.SetParent(NetworkModule.NetworkOrigin);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = NetworkModule.NetworkOrigin.localScale;

        name = (remote ? "Remote" : "Local") + "Avatar";

        if (IsLocal)
        {
            //Spawn the reference objects (for positioning)
            _headRef = new GameObject("__HeadReference");
            _lHandRef = new GameObject("__LeftHandReference");
            _rHandRef = new GameObject("__RightHandReference");
            _headRef.transform.parent =
            _lHandRef.transform.parent =
            _rHandRef.transform.parent =
            NetworkModule.NetworkOrigin;
        }
        else
        {
            //instantiate the remote objects for this avatar
            _head = Instantiate(_headPrefab, Vector3.zero, Quaternion.identity, transform);
            _rightHand = Instantiate(_rHandPrefab);
            _leftHand = Instantiate(_lHandPrefab);
            _rightHand.transform.SetParent(transform);
            _leftHand.transform.SetParent(transform);
            _rightHand.transform.localPosition = _leftHand.transform.localPosition = Vector3.zero;
            _rightHand.transform.localRotation = _leftHand.transform.localRotation = Quaternion.identity;
        }

        _isInitalized = true;
        return this;
    }

    void Update()
    {
        //Ensure in photon room else reset and return
        if (!PhotonNetwork.InRoom)
        {
            _streamQueue.Reset();
            return;
        }

        //Serialize data if owner, else deserialize it
        if (IsLocal)
            this.SerializeData();
        else if (_streamQueue.HasQueuedObjects())
            this.DeserializeData();
    }

    private void SerializeData()
    {
        //HEAD
        _headRef.transform.position = Camera.main.transform.position;
        _headRef.transform.rotation = Camera.main.transform.rotation;

        _streamQueue.SendNext(_headRef.transform.localPosition);
        _streamQueue.SendNext(_headRef.transform.localRotation);

        //HANDS
        MRTKRiggedHandHelper left, right;
        if (MRTKRiggedHandHelper.TryGetRiggedHands(out left, out right))
        {
            if (left)
            {
                _lHandRef.transform.position = left.AvatarRiggedHands.Root.position;
                _lHandRef.transform.rotation = left.AvatarRiggedHands.Root.rotation;
                HandPose pose = new HandPose(left.AvatarRiggedHands, _lHandRef.transform.localPosition, _lHandRef.transform.localRotation);
                _streamQueue.SendNext(pose);
            }
            else
                _streamQueue.SendNext(new HandPose { IsLeft = true, IsActive = false });

            if (right)
            {
                _rHandRef.transform.position = right.AvatarRiggedHands.Root.position;
                _rHandRef.transform.rotation = right.AvatarRiggedHands.Root.rotation;
                HandPose pose = new HandPose(right.AvatarRiggedHands, _rHandRef.transform.localPosition, _rHandRef.transform.localRotation);
                _streamQueue.SendNext(pose);
            }
            else
                _streamQueue.SendNext(new HandPose { IsLeft = false, IsActive = false });
        }
        else
        {
            //Always send a hand pose even if we don't have hands active
            _streamQueue.SendNext(new HandPose { IsLeft = true, IsActive = false });
            _streamQueue.SendNext(new HandPose { IsLeft = false, IsActive = false });
        }
    }

    private void DeserializeData()
    {
        //HEAD
        _head.transform.localPosition = (Vector3)_streamQueue.ReceiveNext();
        _head.transform.localRotation = (Quaternion)_streamQueue.ReceiveNext();
        //HANDS
        HandPose leftPose = (HandPose)_streamQueue.ReceiveNext();
        if (leftPose.IsActive)
        {
            if (!_leftHand.gameObject.activeInHierarchy)
                _leftHand.gameObject.SetActive(true);
            _leftHand.ApplyPose(leftPose);
        }
        else
        {
            if (_leftHand.gameObject.activeInHierarchy)
                _leftHand.gameObject.SetActive(false);
        }

        HandPose rightPose = (HandPose)_streamQueue.ReceiveNext();
        if (rightPose.IsActive)
        {
            if (!_rightHand.gameObject.activeInHierarchy)
                _rightHand.gameObject.SetActive(true);
            _rightHand.ApplyPose(rightPose);
        }
        else
        {
            if (_rightHand.gameObject.activeInHierarchy)
                _rightHand.gameObject.SetActive(false);
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
            Destroy(_lHandRef.gameObject);
            Destroy(_rHandRef.gameObject);
        }
        Destroy(gameObject);
    }

    #region RPCs

    [PunRPC]
    void SetColorRPC(Vector3 color)
    {
        Color = new Color(color.x, color.y, color.z);
    }

    [PunRPC]
    void SetNameRPC(string name)
    {
        Name = name;
    }

    [PunRPC]
    void PropertyChangedRPC(string property, object value)
    {
        //TODO
    }

    #endregion

}


public class PropertyChangedEvent : UnityEvent<string, object> { }