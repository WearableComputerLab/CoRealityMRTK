using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using UnityEngine.Events;
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
    /// </summary>
    public static Dictionary<int, HoloAvatar> RemoteAvatars
    {
        get => Instance._remoteAvatars;
    }

    [SerializeField]
    private HoloAvatar _holoAvatarPrefab;

    //--------------------------------------

    [SerializeField, Tooltip("Called when a new HoloAvatar object is createds")]
    protected HoloAvatarEvent _onAvatarCreated;

    /// <summary>
    /// Called when a new avatar is created
    /// </summary>
    /// <value></value>
    public HoloAvatarEvent OnAvatarCreated { get => _onAvatarCreated; }

    [SerializeField, Tooltip("This is called just before the HoloAvatar's gameobject is destoryed.")]
    protected HoloAvatarEvent _onAvatarDestroyed;

    public HoloAvatarEvent OnAvatarDestroyed { get => _onAvatarDestroyed; }

    void Awake()
    {
        Instance = this;

        //Check for network module (Ensure this is after NetworkModule in Script Execution Order)
        if (!NetworkModule.Instance)
            throw new System.Exception("NetworkModule not present in scene? Can't start AvatarModule until it exists.");

        //Add HandPose to Photon's known serializable data types
        PhotonPeer.RegisterType(typeof(HandPose), 0x68, HandPose.SerializeHandPose, HandPose.DeserializeHandPose);
        PhotonNetwork.AddCallbackTarget(this);
    }


    private void SpawnAvatar(bool remote, int viewID = -1)
    {
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
                        CachingOption = EventCaching.AddToRoomCache
                    },
                    new SendOptions
                    {
                        Reliability = true
                    }
                );
                _onAvatarCreated?.Invoke(avatar);
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
            _onAvatarCreated?.Invoke(avatar);
        }
        //Set color
        _localAvatar.Color = new Color(1, 0, 0);
    }

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
    }

    public void OnEvent(EventData photonEvent)
    {
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

}

[Serializable]
public class HoloAvatarEvent : UnityEvent<HoloAvatar> { }