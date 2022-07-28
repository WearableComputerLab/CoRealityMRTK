using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace CoReality
{

    public class LobbySoundModule : MonoBehaviour
    {
        [SerializeField, Tooltip("The audio source to play these sounds through")]
        private AudioSource _source;

        [SerializeField]
        private AudioClip _onConnectedAudio,
         _onDisconnectedAudio,
         _playerJoinAudio,
         _playLeftAudio;

        void Start()
        {
            NetworkModule.OnConnectedEvent.AddListener(OnConnected);
            NetworkModule.OnDisconnectedEvent.AddListener(OnDisconnected);
            NetworkModule.OnPlayerJoinedRoomEvent.AddListener(OnPlayerJoinedRoom);
            NetworkModule.OnPlayerLeftRoomEvent.AddListener(OnPlayerLeftRoom);
        }

        private void OnConnected()
        {
            if (_source)
                _source.PlayOneShot(_onConnectedAudio, 0.5f);
        }

        private void OnDisconnected()
        {
            if (_source)
                _source.PlayOneShot(_onDisconnectedAudio, 0.5f);
        }

        private void OnPlayerJoinedRoom(Player p)
        {
            if (_source)
                _source.PlayOneShot(_playerJoinAudio, 0.5f);
        }

        private void OnPlayerLeftRoom(Player p)
        {
            if (_source)
                _source.PlayOneShot(_playLeftAudio, 0.5f);
        }

    }

}