using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.Events;
using DisplayProp = CoReality.Avatars.AvatarPeripheralVisualiser.DisplayProp;

namespace CoReality.Avatars
{

    /// <summary>
    /// Base class for a networked avatar behaivour, that modifies or adds some type
    /// of behaviour to a HoloAvatar. Primarily, derived classes can piggyback off of
    /// the PhotonStreamQueue handled by the HoloAvatar to send and receive data. 
    /// 
    /// Is this concept worth it? Unlikely, unless AvatarNetBehaviour is also used to 
    /// encapsulate avatar-specific functionality such as property changes across all
    /// behaviours (such as the player's colour updating).
    /// </summary>
    public abstract class AvatarNetBehaviour : MonoBehaviour
    {

        [Tooltip("Defines the order that that the HoloAvatar will process this behaviour.")]
        [SerializeField] private int _executionOrder;

        private HoloAvatar _avatar;

        public HoloAvatar Avatar { get => _avatar; }
        public int ExecutionOrder { get => _executionOrder; }

        public virtual void InitLocal(HoloAvatar avatar)
        {
            _avatar = avatar;
        }

        public virtual void InitRemote(HoloAvatar avatar)
        {
            _avatar = avatar;
        }

        public virtual void SetAvatarColor(Color color) { }

        public abstract void SerializeData(PhotonStreamQueue _stream);
        public abstract void DeserializeData(PhotonStreamQueue _stream);


    }

}