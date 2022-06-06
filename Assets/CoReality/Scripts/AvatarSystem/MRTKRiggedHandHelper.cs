using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoReality.Avatars
{

    public class MRTKRiggedHandHelper : MonoBehaviour
    {

        [SerializeField]
        public AvatarRiggedHand AvatarRiggedHands;

        //Reference to the left hand helper if it exists
        private static MRTKRiggedHandHelper _leftHelperRef;

        //Reference to the right hand helper if it exists
        private static MRTKRiggedHandHelper _rightHelperRef;

        /// <summary>
        /// Trys to get the rigged hands from MRTK
        /// returns true if atleast one hand is found,
        /// false if no hands a found
        /// either or can be null so ensure its checked
        /// </summary>
        /// <returns></returns>
        public static bool TryGetRiggedHands(out MRTKRiggedHandHelper leftHand, out MRTKRiggedHandHelper rightHand)
        {
            leftHand = rightHand = default(MRTKRiggedHandHelper);
            try
            {
                if (_leftHelperRef != null)
                    leftHand = _leftHelperRef;
                if (_rightHelperRef != null)
                    rightHand = _rightHelperRef;

                //Early out if the reference hasn't been destroyed
                if (leftHand != null || rightHand != null)
                    return true;

                //Search for objects of Type MRTKRiggedHandHelper
                MRTKRiggedHandHelper[] riggedHands = (MRTKRiggedHandHelper[])GameObject.FindObjectsOfType<MRTKRiggedHandHelper>();
                if (riggedHands.Length > 0)
                {
                    if (riggedHands[0].AvatarRiggedHands.Handedness == Handedness.Left)
                        leftHand = _leftHelperRef = riggedHands[0];
                    else
                        rightHand = _rightHelperRef = riggedHands[0];

                    if (riggedHands.Length > 1)
                    {
                        if (riggedHands[1].AvatarRiggedHands.Handedness == Handedness.Left)
                            leftHand = _leftHelperRef = riggedHands[1];
                        else
                            rightHand = _rightHelperRef = riggedHands[1];
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to get rigged hands: " + e);
                return false;
            }
        }

    }

}