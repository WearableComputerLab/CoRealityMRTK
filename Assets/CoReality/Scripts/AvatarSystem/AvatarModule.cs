using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using UnityEngine.Events;
using Photon.Pun.UtilityScripts;
using CoReality.Spectator;
using System.Linq;

namespace CoReality.Avatars
{

    /// <summary>
    /// Controls the spawning of HoloAvatars on photon callbacks
    /// </summary>
    public class AvatarModule : MonoBehaviour, IMatchmakingCallbacks, IInRoomCallbacks, IOnEventCallback
    {
        public static AvatarModule Instance;

        public const byte AVATAR_EVENT = 0x60;

        public const byte SPECTATOR_EVENT = 0x61;

        //------------------------------------

        /// <summary>
        /// The avatar for the local player
        /// </summary>
        private AvatarBase _localAvatar;

        public static AvatarBase LocalAvatar
        {
            get => Instance._localAvatar;
        }

        private Dictionary<int, AvatarBase> _remoteAvatars = new Dictionary<int, AvatarBase>();

        /// <summary>
        /// List of remote avatars in the room, new avatars are added when another player joins the room
        /// Dictionary of actor number, HoloAvatar 
        /// </summary>
        public static Dictionary<int, AvatarBase> RemoteAvatars
        {
            get => Instance._remoteAvatars;
        }

        private List<HoloAvatar> _holoAvatars = new List<HoloAvatar>();

        /// <summary>
        /// Gets a list of all the current HoloAvatars in the scene (not including spectators)
        /// </summary>
        /// <value></value>
        public static List<HoloAvatar> HoloAvatars
        {
            get => Instance._holoAvatars;
        }

        private List<SpectatorAvatar> _spectatorAvatars = new List<SpectatorAvatar>();

        public static List<SpectatorAvatar> SpectatorAvatars
        {
            get => Instance._spectatorAvatars;
        }

        [SerializeField, Tooltip("The prefab for the hololens avatar")]
        private HoloAvatar _holoAvatarPrefab;

        [SerializeField, Tooltip("The prefab for the spectator avatar")]
        private SpectatorAvatar _specAvatarPrefab;

        [SerializeField, Tooltip("The default material for the avatar hands")]
        private Material _defaultHandMaterial;

        public static Material DefaultHandMaterial
        {
            get => Instance._defaultHandMaterial;
        }

        private bool _isSpectator = false;

        public static bool IsSpectator
        {
            get => Instance._isSpectator;
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

            //Create PlayerNumbering gameobject
            new GameObject("PlayerNumberingUtil").AddComponent<Photon.Pun.UtilityScripts.PlayerNumbering>();

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
            //spawn local avatar
            HoloAvatar avatar = Instantiate(_holoAvatarPrefab);

            avatar.Initalize(remote);
            //Forward property change event to static event so interface can listen to it
            avatar.OnPropertyChanged.AddListener((prop, val) => { _onAvatarPropertyChanged?.Invoke(avatar, prop, val); });

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
                    //Set color
                    _localAvatar.Name = _randomAdjectives[UnityEngine.Random.Range(0, _randomAdjectives.Count)] + " "
                                        + _randomNouns[UnityEngine.Random.Range(0, _randomNouns.Count)];
                    //Add listener to player number change
                    PlayerNumbering.OnPlayerNumberingChanged += PlayerColorChanged;
                }
                else
                {
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
        }

        /// <summary>
        /// Spawns a spectator either remotely or locally
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="viewID"></param>
        public void SpawnSpectator(bool remote, int viewID = -1)
        {
            SpectatorAvatar spectator = Instantiate(_specAvatarPrefab);
            spectator.Initalize(remote);

            spectator.OnPropertyChanged.AddListener((prop, val) => { _onAvatarPropertyChanged?.Invoke(spectator, prop, val); });

            if (!remote)
            {
                _localAvatar = spectator;
                if (PhotonNetwork.AllocateViewID(spectator.photonView))
                {
                    PhotonNetwork.RaiseEvent(
                        SPECTATOR_EVENT,
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
                    _onAvatarCreated?.Invoke(spectator);
                    PopulateAvatarList();

                    //Update name and color to be spectator
                    _localAvatar.Name = "Spectator";
                    _localAvatar.Color = Color.gray;
                }
                else
                {
                    Destroy(spectator.gameObject);
                }
            }
            else
            {
                spectator.photonView.ViewID = viewID;
                _remoteAvatars.Add(spectator.photonView.OwnerActorNr, spectator);
                PopulateAvatarList();
                _onAvatarCreated?.Invoke(spectator);
            }
        }

        /// <summary>
        /// Yeah, same
        /// </summary>
        private void PlayerColorChanged()
        {
            //Set color
            _localAvatar.Color = _randomColors[PhotonNetwork.LocalPlayer.GetPlayerNumber()];
            //Remove listener
            PlayerNumbering.OnPlayerNumberingChanged -= PlayerColorChanged;
        }

        /// <summary>
        /// Populates the avatar list with the current avatars [0] is always local
        /// </summary>
        private void PopulateAvatarList()
        {
            _holoAvatars.Clear();
            _spectatorAvatars.Clear();

            //Local avatar could be a spectator
            if (LocalAvatar != null)
            {
                if (LocalAvatar is HoloAvatar localHolo)
                    _holoAvatars.Add(localHolo);
                if (LocalAvatar is SpectatorAvatar localSpec)
                    _spectatorAvatars.Add(localSpec);
            }

            foreach (AvatarBase b in RemoteAvatars.Values)
            {
                if (b is HoloAvatar holo)
                    _holoAvatars.Add(holo);
                if (b is SpectatorAvatar spec)
                    _spectatorAvatars.Add(spec);
            }
        }

        #region Helper Functions

        /// <summary>
        /// Gets the origin-space center position between all of the
        /// connected HoloAvatars using an encapsulating bounds
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
            if (SpectatorRig.Instance == null)
            {
                SpawnAvatar(false);
            }
            //Spawn spectator if module exists
            else
            {
                SpawnSpectator(false);
            }
        }

        public void OnLeftRoom()
        {
            //Remove all avatars including self
            _localAvatar.Destroy();
            _localAvatar = null;
            foreach (AvatarBase avatar in _remoteAvatars.Values)
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
            if (photonEvent.Code == AVATAR_EVENT)
            {
                SpawnAvatar(true, (int)photonEvent.CustomData);
            }
            else if (photonEvent.Code == SPECTATOR_EVENT)
            {
                SpawnSpectator(true, (int)photonEvent.CustomData);
            }
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
    public class HoloAvatarEvent : UnityEvent<AvatarBase> { }

    public class HoloAvatarPropertyChanged : UnityEvent<AvatarBase, string, object> { }

}