using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
/// <summary>
/// The minimum amount of data to send over the network for
/// a hand pose
/// </summary>
public class HandPose
{
    public bool IsLeft { get; set; } //1
    public Vector3 Position { get; set; } //12
    public Quaternion Rotation { get; set; } //Quat to prevent gimble lock //16
    public Vector3 Wrist { get; set; } //Eulers //12
    public Vector3 Middle1 { get; set; } //12
    public float Middle2 { get; set; } //Rot on X axis only //4
    public float Middle3 { get; set; } //4
    public Vector3 Pointer1 { get; set; } //12
    public float Pointer2 { get; set; } //4
    public float Pointer3 { get; set; } //4
    public Vector3 Ring1 { get; set; } //12
    public float Ring2 { get; set; } //4
    public float Ring3 { get; set; } //4
    public Vector3 Thumb1 { get; set; } //12 
    public float Thumb2 { get; set; } //4
    public float Thumb3 { get; set; } //4
    public Vector3 WristPadding { get; set; } //12
    public Vector3 Pinky1 { get; set; } //12
    public float Pinky2 { get; set; } //4
    public float Pinky3 { get; set; } //4

    /// <summary>
    /// Determins if this hand is active or not, if false
    /// then the client sending this data does not currently
    /// have this hand visable;
    /// </summary>
    /// <value></value>
    public bool IsActive { get; set; } //1

    /*
        number of bytes to serialize
        153
    */

    public HandPose() { }

    /// <summary>
    /// Creates a new pose from the NetworkRiggedHandPose
    /// </summary>
    /// <param name="hand"></param>
    public HandPose(AvatarRiggedHand hand)
    {
        IsLeft = hand.Handedness == Handedness.Left;
        Position = hand.Root.localPosition;
        Rotation = hand.Root.localRotation;
        Wrist = hand.Wrist.localEulerAngles;
        Middle1 = hand.Middle1.localEulerAngles;
        Middle2 = hand.Middle2.localEulerAngles.z;
        Middle3 = hand.Middle3.localEulerAngles.z;
        Pointer1 = hand.Pointer1.localEulerAngles;
        Pointer2 = hand.Pointer2.localEulerAngles.z;
        Pointer3 = hand.Pointer3.localEulerAngles.z;
        Ring1 = hand.Ring1.localEulerAngles;
        Ring2 = hand.Ring2.localEulerAngles.z;
        Ring3 = hand.Ring3.localEulerAngles.z;
        Thumb1 = hand.Thumb1.localEulerAngles;
        Thumb2 = hand.Thumb2.localEulerAngles.z;
        Thumb3 = hand.Thumb3.localEulerAngles.z;
        WristPadding = hand.WristPadding.localEulerAngles;
        Pinky1 = hand.Pinky1.localEulerAngles;
        Pinky2 = hand.Pinky2.localEulerAngles.z;
        Pinky3 = hand.Pinky3.localEulerAngles.z;
        IsActive = true;
    }

    /// <summary>
    /// Creates a new hand pose from the NetworkriggedHandPose, but with the poistion
    /// and rotation manually set.
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public HandPose(AvatarRiggedHand hand, Vector3 position, Quaternion rotation)
    {
        IsLeft = hand.Handedness == Handedness.Left;
        Position = position;
        Rotation = rotation;
        Wrist = hand.Wrist.localEulerAngles;
        Middle1 = hand.Middle1.localEulerAngles;
        Middle2 = hand.Middle2.localEulerAngles.z;
        Middle3 = hand.Middle3.localEulerAngles.z;
        Pointer1 = hand.Pointer1.localEulerAngles;
        Pointer2 = hand.Pointer2.localEulerAngles.z;
        Pointer3 = hand.Pointer3.localEulerAngles.z;
        Ring1 = hand.Ring1.localEulerAngles;
        Ring2 = hand.Ring2.localEulerAngles.z;
        Ring3 = hand.Ring3.localEulerAngles.z;
        Thumb1 = hand.Thumb1.localEulerAngles;
        Thumb2 = hand.Thumb2.localEulerAngles.z;
        Thumb3 = hand.Thumb3.localEulerAngles.z;
        WristPadding = hand.WristPadding.localEulerAngles;
        Pinky1 = hand.Pinky1.localEulerAngles;
        Pinky2 = hand.Pinky2.localEulerAngles.z;
        Pinky3 = hand.Pinky3.localEulerAngles.z;
        IsActive = true;
    }


    /// <summary>
    /// Serializes the given hand pose so it can be sent through PUN
    /// </summary>
    /// <param name="customObject"></param>
    /// <returns></returns>
    public static byte[] SerializeHandPose(object customObject)
    {
        HandPose pose = (HandPose)customObject;
        byte[] bytes = new byte[154];

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
        //Wrist
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Wrist.x), 0, bytes, 29, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Wrist.y), 0, bytes, 33, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Wrist.z), 0, bytes, 37, 4);
        //Middle1
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Middle1.x), 0, bytes, 41, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Middle1.y), 0, bytes, 45, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Middle1.z), 0, bytes, 49, 4);
        //Middle2
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Middle2), 0, bytes, 53, 4);
        //Middle3
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Middle3), 0, bytes, 57, 4);
        //Pointer1
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Pointer1.x), 0, bytes, 61, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Pointer1.y), 0, bytes, 65, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Pointer1.z), 0, bytes, 69, 4);
        //Pointer2
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Pointer2), 0, bytes, 73, 4);
        //Pointer3
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Pointer3), 0, bytes, 77, 4);
        //Ring1
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Ring1.x), 0, bytes, 81, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Ring1.y), 0, bytes, 85, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Ring1.z), 0, bytes, 89, 4);
        //Ring2
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Ring2), 0, bytes, 93, 4);
        //Ring3
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Ring3), 0, bytes, 97, 4);
        //Thumb1
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Thumb1.x), 0, bytes, 101, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Thumb1.y), 0, bytes, 105, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Thumb1.z), 0, bytes, 109, 4);
        //Thumb2
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Thumb2), 0, bytes, 113, 4);
        //Thumb3
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Thumb3), 0, bytes, 117, 4);
        //WristPadding
        Buffer.BlockCopy(BitConverter.GetBytes(pose.WristPadding.x), 0, bytes, 121, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.WristPadding.y), 0, bytes, 125, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.WristPadding.z), 0, bytes, 129, 4);
        //Pinky1
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Pinky1.x), 0, bytes, 133, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Pinky1.y), 0, bytes, 137, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Pinky1.z), 0, bytes, 141, 4);
        //Pinky2
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Pinky2), 0, bytes, 145, 4);
        //Pinky3
        Buffer.BlockCopy(BitConverter.GetBytes(pose.Pinky3), 0, bytes, 149, 4);

        bytes[153] = BitConverter.GetBytes(pose.IsActive)[0];

        return bytes;
    }

    /// <summary>
    /// Deserializes the byte array into a hand pose
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static object DeserializeHandPose(byte[] bytes)
    {
        HandPose pose = new HandPose();
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
        pose.Wrist = new Vector3(
            BitConverter.ToSingle(bytes, 29),
            BitConverter.ToSingle(bytes, 33),
            BitConverter.ToSingle(bytes, 37)
        );
        pose.Middle1 = new Vector3(
            BitConverter.ToSingle(bytes, 41),
            BitConverter.ToSingle(bytes, 45),
            BitConverter.ToSingle(bytes, 49)
        );
        pose.Middle2 = BitConverter.ToSingle(bytes, 53);
        pose.Middle3 = BitConverter.ToSingle(bytes, 57);
        pose.Pointer1 = new Vector3(
            BitConverter.ToSingle(bytes, 61),
            BitConverter.ToSingle(bytes, 65),
            BitConverter.ToSingle(bytes, 69)
        );
        pose.Pointer2 = BitConverter.ToSingle(bytes, 73);
        pose.Pointer3 = BitConverter.ToSingle(bytes, 77);
        pose.Ring1 = new Vector3(
            BitConverter.ToSingle(bytes, 81),
            BitConverter.ToSingle(bytes, 85),
            BitConverter.ToSingle(bytes, 89)
        );
        pose.Ring2 = BitConverter.ToSingle(bytes, 93);
        pose.Ring3 = BitConverter.ToSingle(bytes, 97);
        pose.Thumb1 = new Vector3(
            BitConverter.ToSingle(bytes, 101),
            BitConverter.ToSingle(bytes, 105),
            BitConverter.ToSingle(bytes, 109)
        );
        pose.Thumb2 = BitConverter.ToSingle(bytes, 113);
        pose.Thumb3 = BitConverter.ToSingle(bytes, 117);
        pose.WristPadding = new Vector3(
            BitConverter.ToSingle(bytes, 121),
            BitConverter.ToSingle(bytes, 125),
            BitConverter.ToSingle(bytes, 129)
        );
        pose.Pinky1 = new Vector3(
            BitConverter.ToSingle(bytes, 133),
            BitConverter.ToSingle(bytes, 137),
            BitConverter.ToSingle(bytes, 141)
        );
        pose.Pinky2 = BitConverter.ToSingle(bytes, 145);
        pose.Pinky3 = BitConverter.ToSingle(bytes, 149);

        pose.IsActive = BitConverter.ToBoolean(bytes, 153);

        return pose;
    }
}