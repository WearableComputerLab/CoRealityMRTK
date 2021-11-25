using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using ExitGames.Client.Photon;

/// <summary>
/// Handles the connection side of the photon networking
/// </summary>
public class NetworkModule : MonoBehaviour, IMatchmakingCallbacks, IConnectionCallbacks, IInRoomCallbacks
{
    public static NetworkModule Instance;

    [SerializeField, Tooltip("Automatically connect to the Photon network, if false you must call Connect() yourself.")]
    protected bool _autoConnect = true;

    [SerializeField, Tooltip("Your Photon cloud app id")]
    protected string _appID;

    [SerializeField, Tooltip("The photon cloud region")]
    protected string _region;

    [SerializeField, Tooltip("The name of the room to connect too")]
    protected string _roomName;

    //--------------------------------

    [SerializeField]
    protected UnityEvent _onConnected;
    public static UnityEvent OnConnectedEvent { get => Instance._onConnected; }

    [SerializeField]
    protected UnityEvent _onDisconnected;
    public static UnityEvent OnDisconnectedEvent { get => Instance._onDisconnected; }

    [SerializeField]
    protected UnityEvent _onJoinedRoom;
    public static UnityEvent OnJoinedRoomEvent { get => Instance._onJoinedRoom; }

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


    void Awake()
    {
        Instance = this;
        //Ensure this is target for photon callbacks
        PhotonNetwork.AddCallbackTarget(this);

        //Create the Network origin object
        _networkOriginObject = new GameObject("__ORIGIN__");
    }

    void Start()
    {
        if (_autoConnect)
            Connect();
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
    {

    }

    public void OnCreatedRoom()
    {

    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {

    }

    public void OnJoinedRoom()
    {
        _onJoinedRoom.Invoke();
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {

    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {

    }

    public void OnLeftRoom()
    {

    }

    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
        _onConnected.Invoke();
    }

    public void OnDisconnected(DisconnectCause cause)
    {
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

[System.Serializable]
public class PlayerEvent : UnityEvent<Player> { }

