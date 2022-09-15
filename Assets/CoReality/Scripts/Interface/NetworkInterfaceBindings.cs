using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using CoReality.Avatars;

namespace CoReality
{
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

        [SerializeField]
        private Button _connectionButton;

        [SerializeField]
        private TextMeshProUGUI _connectionButtonText;

        //Requires AvatarModule
        [SerializeField]
        private VerticalLayout _userLayout;

        [SerializeField]
        private UserItem _userItemPrefab;

        private Dictionary<AvatarBase, UserItem> _userItems = new Dictionary<AvatarBase, UserItem>();

        void Awake()
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

            _connectionButton?.onClick.AddListener(() =>
            {
                _connectionButton.interactable = false;
                if (PhotonNetwork.IsConnected)
                {
                    NetworkModule.Disconnect();
                }
                else
                {
                    NetworkModule.Connect();
                }
            });

            //Listen to connected event
            NetworkModule.OnConnectedEvent.AddListener(() =>
            {
                if (_connStatusText)
                {
                    _connStatusText.text = "Connected";
                    _connStatusText.color = Color.green;
                }
                if (_connectionButtonText)
                    _connectionButtonText.text = "Disconnect";

                if (_connectionButton)
                    _connectionButton.interactable = true;
            });

            //Listen to disconnected event
            NetworkModule.OnDisconnectedEvent.AddListener(() =>
            {
                if (_connStatusText)
                {
                    _connStatusText.text = "Disconnected";
                    _connStatusText.color = Color.red;
                }
                if (_connectionButtonText)
                    _connectionButtonText.text = "Connect";

                if (_connectionButton)
                    _connectionButton.interactable = true;
            });


            //Listen to avatar created event
            AvatarModule.OnAvatarCreated.AddListener((avatar) =>
            {
                UserItem item = Instantiate(_userItemPrefab);
                item.Color = avatar.Color;
                item.Name = avatar.Name;
                _userLayout.AddItem(item.RectTransform);
                _userItems.Add(avatar, item);
            });

            //Listen to Avatar property change events
            AvatarModule.OnAvatarPropertyChanged.AddListener((avatar, prop, val) =>
            {
                switch (prop)
                {
                    case nameof(avatar.Color):
                        {
                            Vector3 vec = (Vector3)val;
                            _userItems[avatar].Color = new Color(vec.x, vec.y, vec.z);
                            break;
                        }
                    case nameof(avatar.Name):
                        {
                            _userItems[avatar].Name = (string)val;
                            break;
                        }
                }
            });

            //Listen to avatar destroyed events
            AvatarModule.OnAvatarDestroyed.AddListener((avatar) =>
            {
                _userLayout.RemoveItem(_userItems[avatar].RectTransform);
            });

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

}