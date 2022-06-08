using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace CoReality.Spectator
{

    public class SpectatorRig : MonoBehaviour
    {

        public static SpectatorRig Instance;


        [Header("Offset this object to be positioned at the physical camera's position")]

        [SerializeField]
        private Camera _spectatorCamera;

        [SerializeField, Tooltip("The index of the webcam to use, pick one of the indexes below")]
        private int _deviceToUse = 0;
        private int _prev = -1;

        // [SerializeField, Tooltip("Reference to the currently selected device")]
        // private WebcamInspectorView _selected = null;

        private WebCamTexture _webCamTexture;

        [SerializeField]
        private RawImage _rawImage;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            print("Trying to start the selected webcam");

            _webCamTexture = new WebCamTexture(WebCamTexture.devices[_deviceToUse].name);
            _rawImage.texture = _webCamTexture;
            _webCamTexture.Play();
        }

        void OnValidate()
        {
            // if (_selected == null || _prev != _deviceToUse)
            // {
            //     if (_deviceToUse >= 0 && _deviceToUse < WebCamTexture.devices.Length)
            //         _selected = new WebcamInspectorView(WebCamTexture.devices[_deviceToUse]);
            //     else
            //         _selected = new WebcamInspectorView(null);
            //     _prev = _deviceToUse;
            // }
        }

        void Update()
        {
        }

        public void InitalizeAvatar(SpectatorAvatar avatar)
        {
        }

        private void ProcessImage()
        {
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
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

        public void OnEvent(EventData photonEvent)
        {
        }
    }

    [System.Serializable]
    public class WebcamInspectorView
    {
        public WebcamInspectorView(WebCamDevice? device)
        {
            if (device.HasValue)
            {
                Device = device.Value;
                Name = Device.Value.name;
            }
            else
            {
                Device = null;
                Name = "Selected device doesn't exist";
            }
        }
        public string Name;
        public WebCamDevice? Device;
    }

}