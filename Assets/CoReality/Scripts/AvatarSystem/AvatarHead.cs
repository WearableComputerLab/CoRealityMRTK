using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Voice.Unity;

namespace CoReality.Avatars
{

    /// <summary>
    /// TODO: Gaze implementation and eye direction
    /// </summary>
    public class AvatarHead : MonoBehaviour
    {
        private string _name;

        /// <summary>
        /// Sets the name label for this Avatar
        /// </summary>
        /// <value></value>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                _nameText.text = value;
            }
        }

        /// <summary>
        /// Shows or hides the speaker indicator object
        /// </summary>
        public bool SetSpeaking
        {
            set
            {
                _speakerIndicator.SetActive(value);
            }
        }

        [SerializeField]
        private TextMeshPro _nameText;

        [SerializeField]
        private GameObject _speakerIndicator;

        [SerializeField, Tooltip("Photon speaker for voice communication")]
        private Speaker _speaker;

        void Awake()
        {
            //Initalize speaking false
            SetSpeaking = false;
        }
    }

}