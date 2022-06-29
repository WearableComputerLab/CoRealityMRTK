using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using UnityEngine;


namespace CoReality.Avatars
{

    /// <summary>
    /// Defines a base template and serialization function for a peripheral visualiser 
    /// which defines a peripheral of base type AvatarPeripheral and a pose data base
    /// type of BasePose.
    /// </summary>
    /// <typeparam name="T">Type of hand/controller.</typeparam>
    /// <typeparam name="U">Pose data type for serialization.</typeparam>
    public abstract class TypedAvatarPeripheralVisualiser<T, U> : AvatarPeripheralVisualiser
        where T : AvatarPeripheral
        where U : BasePose
    {

        [SerializeField] protected T _lHandPrefab;
        protected T _lControllerPrefab;
        protected T _leftHand;
        public T LeftHand
        {
            get => _leftHand;
        }

        [SerializeField] protected T _rHandPrefab;
        protected T _rightHand;
        public T RightHand
        {
            get => _rightHand;
        }

        public override AvatarPeripheral LeftPeripheral { get => _leftHand; }
        public override AvatarPeripheral RightPeripheral { get => _rightHand; }

        private Transform _headRef;

        public override void InitRemote()
        {
            _rightHand = Instantiate(_rHandPrefab);
            _leftHand = Instantiate(_lHandPrefab);
            _rightHand.transform.SetParent(transform);
            _leftHand.transform.SetParent(transform);
            _rightHand.transform.localPosition = _leftHand.transform.localPosition = Vector3.zero;
            _rightHand.transform.localRotation = _leftHand.transform.localRotation = Quaternion.identity;
        }

        public override void InitLocal(Transform headRef)
        {
            _headRef = headRef;
            _lHandRef = new GameObject("__LeftHandReference");
            _rHandRef = new GameObject("__RightHandReference");
            _lHandRef.transform.parent = _headRef;
            _rHandRef.transform.parent = _headRef;
        }

        public override void SerializeData(PhotonStreamQueue _streamQueue)
        {
            MRTKPeripheralHelper left, right;
            if (MRTKPeripheralHelper.TryGetRiggedHands(out left, out right))
            {
                if (left)
                {
                    _lHandRef.transform.position = left.AvatarPeripherals.Root.position;
                    _lHandRef.transform.rotation = left.AvatarPeripherals.Root.rotation;
                    _streamQueue.SendNext(CreatePose((T)left.AvatarPeripherals, _lHandRef.transform));
                }
                else
                    _streamQueue.SendNext(CreateEmptyPose(isLeft: true, isActive: false));

                if (right)
                {
                    _rHandRef.transform.position = right.AvatarPeripherals.Root.position;
                    _rHandRef.transform.rotation = right.AvatarPeripherals.Root.rotation;
                    _streamQueue.SendNext(CreatePose((T)right.AvatarPeripherals, _rHandRef.transform));
                }
                else
                    _streamQueue.SendNext(CreateEmptyPose(isLeft: false, isActive: false));
            }
            else
            {
                //Always send a hand pose even if we don't have hands active
                _streamQueue.SendNext(CreateEmptyPose(isLeft: true, isActive: false));
                _streamQueue.SendNext(CreateEmptyPose(isLeft: false, isActive: false));
            }
        }

        public override void DeserializeData(PhotonStreamQueue _streamQueue)
        {
            U leftPose = (U)_streamQueue.ReceiveNext();
            if (leftPose.IsActive)
            {
                if (!_leftHand.gameObject.activeInHierarchy)
                    _leftHand.gameObject.SetActive(true);
                _leftHand.ApplyPose(leftPose);
            }
            else
            {
                if (_leftHand.gameObject.activeInHierarchy)
                    _leftHand.gameObject.SetActive(false);
            }

            U rightPose = (U)_streamQueue.ReceiveNext();
            if (rightPose.IsActive)
            {
                if (!_rightHand.gameObject.activeInHierarchy)
                    _rightHand.gameObject.SetActive(true);
                _rightHand.ApplyPose(rightPose);
            }
            else
            {
                if (_rightHand.gameObject.activeInHierarchy)
                    _rightHand.gameObject.SetActive(false);
            }
        }

        protected abstract U CreatePose(T peripheral, Transform reference);
        protected abstract U CreateEmptyPose(bool isLeft, bool isActive);

    }
}