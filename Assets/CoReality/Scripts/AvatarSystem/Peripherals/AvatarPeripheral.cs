using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;


namespace CoReality.Avatars
{
    /// <summary>
    /// Base class for emulating an MRTK peripheral
    /// </summary>
    [System.Serializable]
    public abstract class AvatarPeripheral : MonoBehaviour
    {

        [SerializeField]
        public Handedness Handedness = Handedness.Left;

        [SerializeField]
        public Transform Root;

        public abstract void ApplyPose(BasePose pose);

    }
}