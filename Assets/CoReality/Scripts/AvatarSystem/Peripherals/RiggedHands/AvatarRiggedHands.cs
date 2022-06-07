using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using UnityEngine;


namespace CoReality.Avatars
{

    /// <summary>
    /// TypedAvatarHandVisualiser concrete implementation to handle AR rigged hands
    /// type peripheral.
    /// </summary>
    public class AvatarRiggedHands : TypedAvatarPeripheralVisualiser<AvatarRiggedHand, HandPose>
    {

        public override void InitRemoteHands()
        {
            base.InitRemoteHands();

            //Set the default hand material if its not null
            if (AvatarModule.DefaultHandMaterial)
                _rightHand.MeshRenderer.material = _leftHand.MeshRenderer.material = AvatarModule.DefaultHandMaterial;
        }

        public override void SetDisplayProperty(DisplayProp prop, object value)
        {
            switch (prop)
            {
                case DisplayProp.Color:
                    Color color = (Color)value;
                    _rightHand.MeshRenderer.material.color = color;
                    _leftHand.MeshRenderer.material.color = color;
                    break;

                case DisplayProp.Scale:

                    break;
            }
        }

        protected override HandPose CreatePose(AvatarRiggedHand peripheral, Transform reference)
        {
            throw new System.NotImplementedException();
        }

        protected override HandPose CreateEmptyPose(bool isLeft, bool isActive)
        {
            throw new System.NotImplementedException();
        }
    }
}