using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using UnityEngine.Events;

namespace CoReality.Avatars
{

    /// <summary>
    /// Controls the spawning of HoloAvatars on photon callbacks
    /// </summary>
    public class AvatarModule : MonoBehaviour, IMatchmakingCallbacks, IInRoomCallbacks, IOnEventCallback
    {
        public static AvatarModule Instance;

        public const byte AVATAR_EVENT = 0x60;


        //------------------------------------

        /// <summary>
        /// The avatar for the local player
        /// </summary>
        private HoloAvatar _localAvatar;

        public static HoloAvatar LocalAvatar
        {
            get => Instance._localAvatar;
        }

        private Dictionary<int, HoloAvatar> _remoteAvatars = new Dictionary<int, HoloAvatar>();

        /// <summary>
        /// List of remote avatars in the room, new avatars are added when another player joins the room
        /// Dictionary of actor number, HoloAvatar 
        /// </summary>
        public static Dictionary<int, HoloAvatar> RemoteAvatars
        {
            get => Instance._remoteAvatars;
        }

        private List<HoloAvatar> _holoAvatars = new List<HoloAvatar>();

        /// <summary>
        /// Gets a list of all the current HoloAvatars in the scene
        /// </summary>
        /// <value></value>
        public static List<HoloAvatar> HoloAvatars
        {
            get => Instance._holoAvatars;
        }

        [SerializeField, Tooltip("The prefab for the hololens avatar")]
        private HoloAvatar _holoAvatarPrefab;

        [SerializeField, Tooltip("The default material for the avatar hands")]
        private Material _defaultHandMaterial;

        public static Material DefaultHandMaterial
        {
            get => Instance._defaultHandMaterial;
        }

        [Header("Uniqueness Generator")]

        [SerializeField]
        private List<string> _randomAdjectives = new List<string>{
        "Mighty",
        "Tiny",
        "Brave",
        "Plain",
        "Chilly",
        "Basic",
        "Crazy",
        "Calm",
        "Untidy",
        "Gentle",
        "Keen",
        "Quiet",
        "Vague",
        "Rare",
        "Jolly"
        };

        [SerializeField]
        private List<string> _randomNouns = new List<string>{
        "Cupboard",
        "Kettle",
        "Lion",
        "Avalanche",
        "Apple",
        "Seaweed",
        "Rabbit",
        "Archer",
        "Expert",
        "Shoes",
        "Guitar",
        "Shadow",
        "Calculator",
        "Texture",
        "Force"
        };

        [SerializeField]
        private List<Color> _randomColors = new List<Color>{
        Color.red,
        Color.yellow,
        Color.green,
        Color.blue,
        Color.cyan,
        Color.magenta,
        };

        //--------------------------------------

        [SerializeField, Tooltip("Called when a new HoloAvatar object is created")]
        protected HoloAvatarEvent _onAvatarCreated = new HoloAvatarEvent();

        /// <summary>
        /// Called when a new avatar is created
        /// </summary>
        /// <value></value>
        public static HoloAvatarEvent OnAvatarCreated { get => Instance._onAvatarCreated; }

        [SerializeField, Tooltip("This is called just before the HoloAvatar's gameobject is destoryed.")]
        protected HoloAvatarEvent _onAvatarDestroyed = new HoloAvatarEvent();
        public static HoloAvatarEvent OnAvatarDestroyed { get => Instance._onAvatarDestroyed; }

        [SerializeField, Tooltip("Called when an HoloAvatar's property has changed")]
        protected HoloAvatarPropertyChanged _onAvatarPropertyChanged = new HoloAvatarPropertyChanged();
        public static HoloAvatarPropertyChanged OnAvatarPropertyChanged { get => Instance._onAvatarPropertyChanged; }

        void Awake()
        {
            Instance = this;

            //Check for network module (Ensure this is after NetworkModule in Script Execution Order)
            if (!NetworkModule.Instance)
                throw new System.Exception("NetworkModule not present in scene? Can't start AvatarModule without it.");

            //Add HandPose to Photon's known serializable data types
            PhotonPeer.RegisterType(typeof(HandPose), 0x68, HandPose.SerializeHandPose, HandPose.DeserializeHandPose);
            PhotonNetwork.AddCallbackTarget(this);
        }

        /// <summary>
        /// Spawns either a remote or local avatar
        /// </summary>
        /// <param name="remote">flag to spawn a remote or local avatar</param>
        /// <param name="viewID"> the avatar's view idea, only needed for remote spawning</param>
        private void SpawnAvatar(bool remote, int viewID = -1)
        {
            print("Spawn Avatar Start");
            //spawn local avatar
            HoloAvatar avatar = Instantiate(_holoAvatarPrefab);
            avatar.Initalize(remote);

            //If the avatar is local, raise event to create a remote version for everyone else
            if (!remote)
            {
                _localAvatar = avatar;
                if (PhotonNetwork.AllocateViewID(_localAvatar.photonView))
                {
                    PhotonNetwork.RaiseEvent(
                        AVATAR_EVENT,
                        _localAvatar.photonView.ViewID,
                        new RaiseEventOptions
                        {
                            Receivers = ReceiverGroup.Others,
                            CachingOption = EventCaching.AddToRoomCache,
                        },
                        new SendOptions
                        {
                            DeliveryMode = DeliveryMode.Reliable
                        }
                    );
                    _onAvatarCreated?.Invoke(avatar);
                    PopulateAvatarList();
                }
                else
                {
                    Debug.LogError("Failed to allocate viewID for Avatar");
                    Destroy(_localAvatar.gameObject);
                }
            }
            else
            {
                //Added avatar to the remote avatar's dictionary
                avatar.photonView.ViewID = viewID;
                _remoteAvatars.Add(avatar.photonView.OwnerActorNr, avatar);
                PopulateAvatarList();
                _onAvatarCreated?.Invoke(avatar);
            }

            //Forward property change event
            avatar.OnPropertyChanged.AddListener((prop, val) => { _onAvatarPropertyChanged?.Invoke(avatar, prop, val); });

            //Set color
            _localAvatar.Name = _randomAdjectives[UnityEngine.Random.Range(0, _randomAdjectives.Count)] + " "
                                + _randomNouns[UnityEngine.Random.Range(0, _randomNouns.Count)];

            //Set the avatar's color to the actor number
            int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
            if (actorNum >= 0) //Ensure not -1
                _localAvatar.Color = _randomColors[PhotonNetwork.LocalPlayer.ActorNumber];

        }

        /// <summary>
        /// Populates the avatar list with the current avatars [0] is always local
        /// </summary>
        private void PopulateAvatarList()
        {
            _holoAvatars.Clear();
            if (LocalAvatar != null)
                _holoAvatars.Add(LocalAvatar);
            _holoAvatars.AddRange(RemoteAvatars.Values);
        }

        #region Helper Functions

        /// <summary>
        /// Gets the origin-space center position between all of the
        /// connected avatars using an encapsulating bounds
        /// </summary>
        public static Vector3 CenterBetweenAvatars()
        {
            if (HoloAvatars.Count > 0)
            {
                Bounds b = new Bounds(HoloAvatars[0].HeadLocalPosition, Vector3.zero);
                for (int i = 0; i < HoloAvatars.Count; i++)
                    b.Encapsulate(HoloAvatars[i].HeadLocalPosition);
                return b.center;
            }
            return Vector3.zero;
        }

        #endregion


        #region PUN Callbacks

        public void OnJoinedRoom()
        {
            //Spawn local avatar
            SpawnAvatar(false);
        }

        public void OnLeftRoom()
        {
            //Remove all avatars including self
            _localAvatar.Destroy();
            _localAvatar = null;
            foreach (HoloAvatar avatar in _remoteAvatars.Values)
            {
                _onAvatarDestroyed?.Invoke(avatar);
                avatar.Destroy();
            }
            _remoteAvatars.Clear();
            PopulateAvatarList();
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            //Remove the avatar for the player who left
            if (otherPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                return;
            var avatar = _remoteAvatars[otherPlayer.ActorNumber];
            _onAvatarDestroyed?.Invoke(avatar);
            avatar.Destroy();
            _remoteAvatars.Remove(otherPlayer.ActorNumber);
            PopulateAvatarList();
        }

        public void OnEvent(EventData photonEvent)
        {
            print("On Event");
            if (photonEvent.Code == AVATAR_EVENT)
                SpawnAvatar(true, (int)photonEvent.CustomData);
        }

        //Unused 
        public void OnPlayerEnteredRoom(Player newPlayer)
        { }
        public void OnCreatedRoom()
        { }
        public void OnCreateRoomFailed(short returnCode, string message)
        { }
        public void OnFriendListUpdate(List<FriendInfo> friendList)
        { }
        public void OnJoinRandomFailed(short returnCode, string message)
        { }
        public void OnJoinRoomFailed(short returnCode, string message)
        { }
        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        { }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        { }
        public void OnMasterClientSwitched(Player newMasterClient)
        { }

        #endregion

    }

    [Serializable]
    public class HoloAvatarEvent : UnityEvent<HoloAvatar> { }

    public class HoloAvatarPropertyChanged : UnityEvent<HoloAvatar, string, object> { }

}