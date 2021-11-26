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
    TextMeshProUGUI _playerCountText;
    [SerializeField]
    TextMeshProUGUI _connStatusText;
    [SerializeField]
    Button _connectionButton;

    void Start()
    {
        _connStatusText.text = "Disconnected";
        _connStatusText.color = Color.red;

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
            NetworkModule.OnPlayerJoinedRoomEvent.AddListener(player =>
            {
                _playerCountText.text = "Players = " + PhotonNetwork.PlayerList.Length;
            });
            NetworkModule.OnPlayerLeftRoomEvent.AddListener(player =>
            {
                _playerCountText.text = "Players = " + PhotonNetwork.PlayerList.Length;
            });
        }
    }

}
