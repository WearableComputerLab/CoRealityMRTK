using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

/// <summary>
/// Interface bindings
/// </summary>
public class NetworkInterfaceBindings : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _userCountText;
    [SerializeField]
    TextMeshProUGUI _connStatusText;
    [SerializeField]
    TextMeshProUGUI _roomNameText;
    [SerializeField]
    Button _connectionButton;

    void Start()
    {
        if (NetworkModule.Instance)
        {
            NetworkModule.OnConnectedEvent.AddListener(() =>
            {
                _connStatusText.text = "Connected";
                _connStatusText.color = Color.green;
            });
            NetworkModule.OnDisconnectedEvent.AddListener(() =>
            {
                _connStatusText.text = "Disconnected";
                _connStatusText.color = Color.red;
            });
            NetworkModule.OnJoinedRoomEvent.AddListener(() =>
            {
                _roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            });
            NetworkModule.OnPlayerJoinedRoomEvent.AddListener(player =>
            {

            });
            NetworkModule.OnPlayerLeftRoomEvent.AddListener(player =>
            {

            });
        }
    }

}
