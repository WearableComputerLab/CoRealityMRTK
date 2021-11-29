using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Controls the spawning of HoloAvatars on photon callbacks
/// </summary>
public class AvatarModule : MonoBehaviour, IMatchmakingCallbacks, IInRoomCallbacks
{
    public static AvatarModule Instance;

    //------------------------------------

    [SerializeField, Tooltip("Enables the Avatar hands and hand data to be sent over the network, see readme" +
        " on how to set up this feature.")]
    private bool _enableHands;

    public static bool EnableHands
    {
        get => Instance._enableHands;
    }

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
    /// </summary>
    public static Dictionary<int, HoloAvatar> RemoteAvatars
    {
        get => Instance._remoteAvatars;
    }

    void Awake()
    {
        Instance = this;
        PhotonNetwork.AddCallbackTarget(this);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnJoinedRoom()
    {
        //Spawn local avatar and raise event

    }

    public void OnLeftRoom()
    {
        //Remove all avatars including own

    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Remove the avatar for the player who left
        
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
}
