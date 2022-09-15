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
        private static MRTKRiggedHandHelper _leftHandCache;

        //Reference to the right hand helper if it exists
        private static MRTKRiggedHandHelper _rightHandCache;

        /// <summary>
        /// Trys to get the rigged hands from MRTK
        /// returns true if atleast one hand is found,
        /// false if no hands are found
        /// </summary>
        /// <returns></returns>
        public static bool TryGetRiggedHands(out MRTKRiggedHandHelper leftHand, out MRTKRiggedHandHelper rightHand)
        {
            leftHand = rightHand = default(MRTKRiggedHandHelper);
            try
            {
                if (_leftHandCache != null)
                    leftHand = _leftHandCache;
                if (_rightHandCache != null)
                    rightHand = _rightHandCache;

                //Early out if the reference hasn't been destroyed
                if (leftHand != null && rightHand != null)
                    return true;

                //Search for objects of Type MRTKRiggedHandHelper
                MRTKRiggedHandHelper[] riggedHands = (MRTKRiggedHandHelper[])GameObject.FindObjectsOfType<MRTKRiggedHandHelper>();
                if (riggedHands.Length > 0)
                {
                    if (riggedHands[0].AvatarRiggedHands.Handedness == Handedness.Left)
                        leftHand = _leftHandCache = riggedHands[0];
                    else
                        rightHand = _rightHandCache = riggedHands[0];

                    if (riggedHands.Length > 1)
                    {
                        if (riggedHands[1].AvatarRiggedHands.Handedness == Handedness.Left)
                            leftHand = _leftHandCache = riggedHands[1];
                        else
                            rightHand = _rightHandCache = riggedHands[1];
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