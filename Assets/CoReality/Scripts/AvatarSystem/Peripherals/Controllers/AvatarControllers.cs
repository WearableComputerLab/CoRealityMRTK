using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using UnityEngine;


namespace CoReality.Avatars
{

    /// <summary>
    /// TypedAvatarHandVisualiser concrete implementation to handle a VR controller
    /// type hand peripheral.
    /// </summary>
    public class AvatarControllers : TypedAvatarPeripheralVisualiser<AvatarController, ControllerPose>
    {
        public override void InitRemote()
        {
            base.InitRemote();

            //Set the default hand material if its not null
            if (AvatarModule.DefaultHandMaterial)
            {
                _leftHand.SetMaterial(AvatarModule.DefaultHandMaterial);
                _rightHand.SetMaterial(AvatarModule.DefaultHandMaterial);
            }
        }

        public override void SetDisplayProperty(DisplayProp prop, object value)
        {
            switch (prop)
            {
                case DisplayProp.Color:
                    Color color = (Color)value;
                    _leftHand.SetColour(color);
                    _rightHand.SetColour(color);
                    break;

                case DisplayProp.Scale:

                    break;
            }
        }

        protected override ControllerPose CreatePose(AvatarController peripheral, Transform reference)
        {
            return new ControllerPose(peripheral, reference.localPosition, reference.localRotation);
        }

        protected override ControllerPose CreateEmptyPose(bool isLeft, bool isActive)
        {
            return new ControllerPose { IsLeft = false, IsActive = false };
        }
    }
}