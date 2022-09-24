using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CoReality.Spectator
{
    [RequireComponent(typeof(SpectatorMove))]
    public class SpectatorRig : MonoBehaviour
    {

        public static SpectatorRig Instance;

        [Header("Offset this object to be positioned at the physical camera's position")]

        [SerializeField]
        private Camera _spectatorCamera;

        [SerializeField, Tooltip("The index of the webcam to use, pick one of the indexes below")]
        private int _deviceToUse = 0;
        private int _prev = -1;

        [SerializeField]
        private bool _swapCamWithArrowKeys = true;

        private WebCamTexture _webCamTexture;

        [SerializeField]
        private RawImage _rawImage;

        [SerializeField]
        private WebcamEvent _onWebcamChanged = new WebcamEvent();

        public WebcamEvent OnWebcamChanged
        {
            get => _onWebcamChanged;
        }

        private SpectatorMove _spectatorMove;

        public SpectatorMove SpectatorMove
        {
            get
            {
                if(_spectatorMove == null)
                    _spectatorMove = GetComponent<SpectatorMove>();
                return _spectatorMove;
            }
        }

        void Awake()
        {
            Instance = this;
            
        }

        void Start()
        {
            SetWebcamTexture(_deviceToUse);
        }

        void Update()
        {
            if (_swapCamWithArrowKeys)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    _deviceToUse++;
                    if (_deviceToUse > WebCamTexture.devices.Length - 1)
                        _deviceToUse = 0;
                    SetWebcamTexture(_deviceToUse);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    _deviceToUse--;
                    if (_deviceToUse < 0)
                        _deviceToUse = WebCamTexture.devices.Length - 1;
                    SetWebcamTexture(_deviceToUse);
                }
            }
        }

        void SetWebcamTexture(int webcamNumber)
        {
            if (_webCamTexture != null)
                _webCamTexture.Stop();
            Debug.Log("Using webcam " + _deviceToUse + " " + WebCamTexture.devices[_deviceToUse].name);
            _webCamTexture = new WebCamTexture(WebCamTexture.devices[_deviceToUse].name);
            _rawImage.texture = _webCamTexture;
            _webCamTexture.Play();
            //Invoke event
            _onWebcamChanged.Invoke(webcamNumber, WebCamTexture.devices[_deviceToUse].name);
        }

    }

    public class WebcamEvent : UnityEvent<int, string> { }

}