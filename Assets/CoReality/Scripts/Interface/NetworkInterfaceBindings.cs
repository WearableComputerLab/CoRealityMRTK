using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

/// <summary>
/// Example interface bindings for CoReality networking
/// </summary>
public class NetworkInterfaceBindings : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI _connStatusText;


    [SerializeField]

    TextMeshProUGUI _pingText;

    [SerializeField]
    Button _quitButton;

    void Start()
    {
        if (_connStatusText)
        {
            _connStatusText.text = "Disconnected";
            _connStatusText.color = Color.red;
        }

        _quitButton?.onClick.AddListener(()=>{
            Application.Quit();
        });

        // if (NetworkModule.Instance)
        {
              
            NetworkModule.OnConnectedEvent.AddListener(() =>
            {
                if (_connStatusText)
                {
                    _connStatusText.text = "Connected";
                    _connStatusText.color = Color.green;
                }
            });
            NetworkModule.OnDisconnectedEvent.AddListener(() =>
            {
                if (_connStatusText)
                {
                    _connStatusText.text = "Disconnected";
                    _connStatusText.color = Color.red;
                }
            });

        }
    }




    void Update()
    {
        if (NetworkModule.Instance && PhotonNetwork.IsConnectedAndReady)
        {
            if (_pingText)
                _pingText.text = PhotonNetwork.GetPing() + "ms";
        }
    }

}
