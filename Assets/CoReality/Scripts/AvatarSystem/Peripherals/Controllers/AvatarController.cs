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
    public class AvatarController : AvatarPeripheral
    {

        [SerializeField, Tooltip("The Meshrender for this hand")]
        public List<SkinnedMeshRenderer> MeshRenderers;

        public override void ApplyPose(BasePose pose)
        {
            Root.transform.localPosition = pose.Position;
            Root.transform.localRotation = pose.Rotation;
        }

        public void SetMaterial(Material material)
        {
            foreach (SkinnedMeshRenderer renderer in MeshRenderers)
            {
                renderer.material = material;
            }
        }

        public void SetColour(Color color)
        {
            foreach (SkinnedMeshRenderer renderer in MeshRenderers)
            {
                renderer.material.color = color;
            }
        }

    }
}