using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MRTKRiggedHandHelper : MonoBehaviour
{

    [SerializeField]
    public AvatarRiggedHand AvatarRiggedHands;

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
        MRTKRiggedHandHelper[] riggedHands = (MRTKRiggedHandHelper[])GameObject.FindObjectsOfType<MRTKRiggedHandHelper>();
        if (riggedHands.Length > 0)
        {
            if (riggedHands[0].AvatarRiggedHands.Handedness == Handedness.Left)
                leftHand = riggedHands[0];
            else
                rightHand = riggedHands[0];

            if (riggedHands.Length > 1)
            {
                if (riggedHands[1].AvatarRiggedHands.Handedness == Handedness.Left)
                    leftHand = riggedHands[1];
                else
                    rightHand = riggedHands[1];
            }
            return true;
        }
        return false;
    }

}