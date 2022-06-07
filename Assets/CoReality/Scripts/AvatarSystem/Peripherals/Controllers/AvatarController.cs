using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;


namespace CoReality.Avatars
{
    /// <summary>
    /// Base class for emulating a MRTK controller
    /// </summary>
    [System.Serializable]
    public abstract class AvatarController : AvatarPeripheral
    {

        [SerializeField, Tooltip("The Meshrender for this hand")]
        public MeshRenderer MeshRenderer;


        public void ApplyPose(ControllerPose pose)
        {
            Root.transform.localPosition = pose.Position;
            Root.transform.localRotation = pose.Rotation;
        }

    }
}