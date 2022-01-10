using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

/// <summary>
/// Interface bindings for the network interface
/// </summary>
public class NetworkInterfaceBindings : MonoBehaviour
{
    //Requires NetworkInterface
    [SerializeField]
    private TextMeshProUGUI _connStatusText;

    [SerializeField]
    private TextMeshProUGUI _pingText;

    [SerializeField]
    private Button _quitButton;

    //Requires AvatarModule
    [SerializeField]
    private VerticalLayout _userLayout;

    [SerializeField]
    private UserItem _userItemPrefab;

    private Dictionary<HoloAvatar, UserItem> _userItems = new Dictionary<HoloAvatar, UserItem>();

    void Start()
    {
        if (_connStatusText)
        {
            _connStatusText.text = "Disconnected";
            _connStatusText.color = Color.red;
        }

        _quitButton?.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        if (NetworkModule.Instance)
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

        if (AvatarModule.Instance)
        {
            AvatarModule.OnAvatarCreated.AddListener((avatar) =>
            {
                UserItem item = Instantiate(_userItemPrefab);
                item.Color = avatar.Color;
                item.Name = "test";
                _userLayout.AddItem(item.RectTransform);
                _userItems.Add(avatar, item);
            });

            AvatarModule.OnAvatarDestroyed.AddListener((avatar) =>
            {
                _userLayout.RemoveItem(_userItems[avatar].RectTransform);
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
