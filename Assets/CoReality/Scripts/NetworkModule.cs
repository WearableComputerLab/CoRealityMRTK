using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using ExitGames.Client.Photon;

namespace CoReality
{

    /// <summary>
    /// Handles the connection side of the photon networking
    /// </summary>
    public class NetworkModule : MonoBehaviour, IMatchmakingCallbacks, IConnectionCallbacks, IInRoomCallbacks
    {
        public static NetworkModule Instance;

        //--------------------------------

        [SerializeField, Tooltip("Automatically connect to the Photon network, if false you must call Connect() yourself.")]
        protected bool _autoConnect = true;

        [SerializeField, Tooltip("Your Photon cloud app id.")]
        protected string _appID = "";

        [SerializeField, Tooltip("The photon cloud region to use.")]
        protected string _region;

        [SerializeField, Tooltip("The name of the room to connect too.")]
        protected string _roomName = "default";

        [SerializeField, Tooltip("The OPTIONAL vuforia target. This marker is what is used to align each user's networked space.")]
        /// <summary>
        /// If no vuforia target is supplied the default position of the origin will be world zero
        /// and all users will be aligned around that position
        /// On hololens this is the user's inital head position when the launch the application.
        /// </summary>
        protected DefaultTrackableEventHandler _vuforiaTarget;

        [SerializeField, Tooltip("An array of objects that are automatically attached to the networked origin transform at runtime.")]
        /// <summary>
        /// Setting these objects will ensure they follower the networked origin 
        /// (aka. the vuforia target's position in worldspace)
        /// </summary>
        protected GameObject[] _attachedObjects;

        //--------------------------------

        [SerializeField]
        protected UnityEvent _onConnected;
        public static UnityEvent OnConnectedEvent { get => Instance._onConnected; }

        [SerializeField]
        protected UnityEvent _onDisconnected;
        public static UnityEvent OnDisconnectedEvent { get => Instance._onDisconnected; }

        [SerializeField]
        protected PlayerEvent _onPlayerJoinedRoom;
        public static PlayerEvent OnPlayerJoinedRoomEvent { get => Instance._onPlayerJoinedRoom; }

        [SerializeField]
        protected PlayerEvent _onPlayerLeftRoom;
        public static PlayerEvent OnPlayerLeftRoomEvent { get => Instance._onPlayerLeftRoom; }

        //------------------------------

        private GameObject _networkOriginObject;

        /// <summary>
        /// The common transfom for all networked objects used
        /// to align and syncronize all user's space.
        /// Automatically set by vuforia. 
        /// </summary>
        public static Transform NetworkOrigin
        {
            get => Instance._networkOriginObject.transform;
        }

        private bool _vuforiaTargetFound = false;

        void Awake()
        {
            Instance = this;
            //Ensure this is target for photon callbacks
            PhotonNetwork.AddCallbackTarget(this);
            //Create the Network origin object
            _networkOriginObject = new GameObject("__ORIGIN__");
            //Attach objects
            foreach (GameObject go in _attachedObjects)
                go.transform.SetParent(NetworkOrigin);
        }

        void Start()
        {
            if (_autoConnect)
                Connect();

            //Add vuforia target behavoirs
            if (_vuforiaTarget)
            {
                _vuforiaTarget.OnTargetFound.AddListener(() => _vuforiaTargetFound = true);
                _vuforiaTarget.OnTargetLost.AddListener(() => _vuforiaTargetFound = false);
            }
        }

        void Update()
        {
            if (_vuforiaTargetFound)
            {
                _networkOriginObject.transform.position = _vuforiaTarget.transform.position;
                _networkOriginObject.transform.rotation = _vuforiaTarget.transform.rotation;
            }
        }

        public static void Connect()
        {
            if (PhotonNetwork.IsConnected)
                return;
            var setting = new Photon.Realtime.AppSettings()
            {
                AppIdRealtime = Instance._appID,
                FixedRegion = Instance._region,
                Protocol = ConnectionProtocol.Udp,
                EnableProtocolFallback = true,
                NetworkLogging = DebugLevel.ERROR,
            };
            PhotonNetwork.ConnectUsingSettings(setting);
        }

        public static void Disconnect()
        {
            if (!PhotonNetwork.IsConnected)
                return;
            PhotonNetwork.Disconnect();
        }

        #region  PUN Callbacks

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {}

        public void OnCreatedRoom()
        {}

        public void OnCreateRoomFailed(short returnCode, string message)
        {}

        public void OnJoinedRoom()
        {}

        public void OnJoinRoomFailed(short returnCode, string message)
        {}

        public void OnJoinRandomFailed(short returnCode, string message)
        {}

        public void OnLeftRoom()
        {}

        public void OnConnected()
        {}

        public void OnConnectedToMaster()
        {
            _onConnected.Invoke();
            //Join the room
            PhotonNetwork.JoinOrCreateRoom(
                _roomName,
                new RoomOptions
                {
                    IsOpen = true,
                    IsVisible = true,
                    MaxPlayers = 0,
                    PublishUserId = true,
                    CleanupCacheOnLeave = true
                },
                TypedLobby.Default
            );
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogError("Disconnected from server " + cause.ToString());
            _onDisconnected.Invoke();
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {

        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {

        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {

        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            _onPlayerJoinedRoom.Invoke(newPlayer);
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            _onPlayerLeftRoom.Invoke(otherPlayer);
        }

        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {

        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {

        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {

        }


        #endregion

    }


    //SECURITY FIRST

    [Serializable]
    public class PlayerEvent : UnityEvent<Player> { }

}