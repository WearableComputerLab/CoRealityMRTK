using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;


namespace CoReality.Avatars
{
    /// <summary>
    /// Emulates a MRTK hand
    /// </summary>
    [System.Serializable]
    public class AvatarRiggedHand : AvatarPeripheral
    {

        [SerializeField, Tooltip("The Meshrender for this hand")]
        public SkinnedMeshRenderer MeshRenderer;

        [SerializeField]
        public Transform Wrist, Middle1, Middle2, Middle3,
        Pointer1, Pointer2, Pointer3, Ring1, Ring2, Ring3, Thumb1, Thumb2, Thumb3,
        WristPadding, Pinky1, Pinky2, Pinky3;

        public override void ApplyPose(BasePose pose)
        {
            HandPose hPose = (HandPose)pose;

            Root.transform.localPosition = hPose.Position;
            Root.transform.localRotation = hPose.Rotation;

            Middle1.transform.localEulerAngles = hPose.Middle1;
            Middle2.transform.localEulerAngles = new Vector3(
                Middle2.transform.localEulerAngles.x,
                Middle2.transform.localEulerAngles.y,
                hPose.Middle2
            );
            Middle3.transform.localEulerAngles = new Vector3(
                Middle3.transform.localEulerAngles.x,
                Middle3.transform.localEulerAngles.y,
                hPose.Middle3
            );

            Pointer1.transform.localEulerAngles = hPose.Pointer1;
            Pointer2.transform.localEulerAngles = new Vector3(
                Pointer2.transform.localEulerAngles.x,
                Pointer2.transform.localEulerAngles.y,
                hPose.Pointer2
            );
            Pointer3.transform.localEulerAngles = new Vector3(
                Pointer3.transform.localEulerAngles.x,
                Pointer3.transform.localEulerAngles.y,
                hPose.Pointer3
            );

            Ring1.transform.localEulerAngles = hPose.Ring1;
            Ring2.transform.localEulerAngles = new Vector3(
                Ring2.transform.localEulerAngles.x,
                Ring2.transform.localEulerAngles.y,
                hPose.Ring2
            );
            Ring3.transform.localEulerAngles = new Vector3(
                Ring3.transform.localEulerAngles.x,
                Ring3.transform.localEulerAngles.y,
                hPose.Ring3
            );

            Thumb1.transform.localEulerAngles = hPose.Thumb1;
            Thumb2.transform.localEulerAngles = new Vector3(
                Thumb2.transform.localEulerAngles.x,
                Thumb2.transform.localEulerAngles.y,
                hPose.Thumb2
            );
            Thumb3.transform.localEulerAngles = new Vector3(
                Thumb3.transform.localEulerAngles.x,
                Thumb3.transform.localEulerAngles.y,
                hPose.Thumb3
            );

            WristPadding.transform.localEulerAngles = hPose.WristPadding;

            Pinky1.transform.localEulerAngles = hPose.Pinky1;
            Pinky2.transform.localEulerAngles = new Vector3(
                Pinky2.transform.localEulerAngles.x,
                Pinky2.transform.localEulerAngles.y,
                hPose.Pinky2
            );
            Pinky3.transform.localEulerAngles = new Vector3(
                Pinky3.transform.localEulerAngles.x,
                Pinky3.transform.localEulerAngles.y,
                hPose.Pinky3
            );
        }


    }
}