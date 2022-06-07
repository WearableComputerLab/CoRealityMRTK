using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace CoReality.Avatars
{


    /// <summary>
    /// The minimum amount of data to send over the network for
    /// a peripheral pose.
    /// </summary>
    public abstract class BasePose
    {
        public bool IsLeft { get; set; } //1
        public Vector3 Position { get; set; } //12
        public Quaternion Rotation { get; set; } //Quat to prevent gimble lock //16

        /// <summary>
        /// Determins if this hand is active or not, if false
        /// then the client sending this data does not currently
        /// have this hand visable;
        /// </summary>
        /// <value></value>
        public bool IsActive { get; set; } //1
    }
}