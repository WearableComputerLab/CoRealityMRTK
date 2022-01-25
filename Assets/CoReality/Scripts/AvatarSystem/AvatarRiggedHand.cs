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
    public class AvatarRiggedHand : MonoBehaviour
    {

        [SerializeField, Tooltip("The Meshrender for this hand")]
        public SkinnedMeshRenderer MeshRenderer;

        [SerializeField]
        public Handedness Handedness = Handedness.Left;

        [SerializeField]
        public Transform Root;

        [SerializeField]
        public Transform Wrist, Middle1, Middle2, Middle3,
        Pointer1, Pointer2, Pointer3, Ring1, Ring2, Ring3, Thumb1, Thumb2, Thumb3,
        WristPadding, Pinky1, Pinky2, Pinky3;

        public void ApplyPose(HandPose pose)
        {
            Root.transform.localPosition = pose.Position;
            Root.transform.localRotation = pose.Rotation;

            Middle1.transform.localEulerAngles = pose.Middle1;
            Middle2.transform.localEulerAngles = new Vector3(
                Middle2.transform.localEulerAngles.x,
                Middle2.transform.localEulerAngles.y,
                pose.Middle2
            );
            Middle3.transform.localEulerAngles = new Vector3(
                Middle3.transform.localEulerAngles.x,
                Middle3.transform.localEulerAngles.y,
                pose.Middle3
            );

            Pointer1.transform.localEulerAngles = pose.Pointer1;
            Pointer2.transform.localEulerAngles = new Vector3(
                Pointer2.transform.localEulerAngles.x,
                Pointer2.transform.localEulerAngles.y,
                pose.Pointer2
            );
            Pointer3.transform.localEulerAngles = new Vector3(
                Pointer3.transform.localEulerAngles.x,
                Pointer3.transform.localEulerAngles.y,
                pose.Pointer3
            );

            Ring1.transform.localEulerAngles = pose.Ring1;
            Ring2.transform.localEulerAngles = new Vector3(
                Ring2.transform.localEulerAngles.x,
                Ring2.transform.localEulerAngles.y,
                pose.Ring2
            );
            Ring3.transform.localEulerAngles = new Vector3(
                Ring3.transform.localEulerAngles.x,
                Ring3.transform.localEulerAngles.y,
                pose.Ring3
            );

            Thumb1.transform.localEulerAngles = pose.Thumb1;
            Thumb2.transform.localEulerAngles = new Vector3(
                Thumb2.transform.localEulerAngles.x,
                Thumb2.transform.localEulerAngles.y,
                pose.Thumb2
            );
            Thumb3.transform.localEulerAngles = new Vector3(
                Thumb3.transform.localEulerAngles.x,
                Thumb3.transform.localEulerAngles.y,
                pose.Thumb3
            );

            WristPadding.transform.localEulerAngles = pose.WristPadding;

            Pinky1.transform.localEulerAngles = pose.Pinky1;
            Pinky2.transform.localEulerAngles = new Vector3(
                Pinky2.transform.localEulerAngles.x,
                Pinky2.transform.localEulerAngles.y,
                pose.Pinky2
            );
            Pinky3.transform.localEulerAngles = new Vector3(
                Pinky3.transform.localEulerAngles.x,
                Pinky3.transform.localEulerAngles.y,
                pose.Pinky3
            );
        }


    }
}