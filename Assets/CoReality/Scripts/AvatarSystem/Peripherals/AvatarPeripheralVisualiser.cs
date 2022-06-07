using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using UnityEngine;


namespace CoReality.Avatars
{

    /// <summary>
    /// Base parent class for an avatar hand visualiser component which will represent
    /// an avatar's hands or controllers.
    /// </summary>
    public abstract class AvatarPeripheralVisualiser : MonoBehaviour
    {

        public enum DisplayProp
        {
            Color,
            Scale
        }

        protected GameObject _lHandRef, _rHandRef;

        public GameObject LHandRef { get => _lHandRef; set => _lHandRef = value; }
        public GameObject RHandRef { get => _rHandRef; set => _rHandRef = value; }

        public abstract void InitRemoteHands();
        public abstract void SerializeData(PhotonStreamQueue _streamQueue);
        public abstract void DeserialiseData(PhotonStreamQueue _streamQueue);
        public abstract void SetDisplayProperty(DisplayProp prop, object value);

    }
}