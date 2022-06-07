using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace CoReality.Avatars
{


    /// <summary>
    /// The minimum amount of data to send over the network for
    /// a controller pose (position + rotation)
    /// </summary>
    public class ControllerPose : BasePose
    {

        /*
            number of bytes to serialize
            153
        */

        public ControllerPose() { }

        /// <summary>
        /// Creates a new pose from the NetworkRiggedHandPose
        /// </summary>
        /// <param name="hand"></param>
        public ControllerPose(AvatarController hand)
        {
            IsLeft = hand.Handedness == Handedness.Left;
            Position = hand.Root.localPosition;
            Rotation = hand.Root.localRotation;
            IsActive = true;
        }

        /// <summary>
        /// Creates a new hand pose from the NetworkriggedHandPose, but with the poistion
        /// and rotation manually set.
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public ControllerPose(AvatarController hand, Vector3 position, Quaternion rotation)
        {
            IsLeft = hand.Handedness == Handedness.Left;
            Position = position;
            Rotation = rotation;
            IsActive = true;
        }


        /// <summary>
        /// Serializes the given controller pose so it can be sent through PUN
        /// </summary>
        /// <param name="customObject"></param>
        /// <returns></returns>
        public static byte[] SerializeControllerPose(object customObject)
        {
            HandPose pose = (HandPose)customObject;
            byte[] bytes = new byte[28];

            bytes[0] = BitConverter.GetBytes(pose.IsLeft)[0];

            //Position
            Buffer.BlockCopy(BitConverter.GetBytes(pose.Position.x), 0, bytes, 1, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(pose.Position.y), 0, bytes, 5, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(pose.Position.z), 0, bytes, 9, 4);
            //Rotation
            Buffer.BlockCopy(BitConverter.GetBytes(pose.Rotation.x), 0, bytes, 13, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(pose.Rotation.y), 0, bytes, 17, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(pose.Rotation.z), 0, bytes, 21, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(pose.Rotation.w), 0, bytes, 25, 4);

            bytes[27] = BitConverter.GetBytes(pose.IsActive)[0];

            return bytes;
        }

        /// <summary>
        /// Deserializes the byte array into a controller pose
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static object DeserializeControllerPose(byte[] bytes)
        {
            ControllerPose pose = new ControllerPose();
            pose.IsLeft = BitConverter.ToBoolean(bytes, 0);
            pose.Position = new Vector3(
                BitConverter.ToSingle(bytes, 1),
                BitConverter.ToSingle(bytes, 5),
                BitConverter.ToSingle(bytes, 9)
            );
            pose.Rotation = new Quaternion(
                BitConverter.ToSingle(bytes, 13),
                BitConverter.ToSingle(bytes, 17),
                BitConverter.ToSingle(bytes, 21),
                BitConverter.ToSingle(bytes, 25)
            );

            pose.IsActive = BitConverter.ToBoolean(bytes, 27);

            return pose;
        }
    }
}