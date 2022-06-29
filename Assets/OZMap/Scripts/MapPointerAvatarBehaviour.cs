using CoReality.Avatars;
using Photon.Pun;
using UnityEngine;

namespace OZMap.Networking
{
    [RequireComponent(typeof(MapAvatarBehaviour))]
    public class MapPointerAvatarBehaviour : AvatarNetBehaviour
    {

        private AvatarPeripheral _leftHand;
        private ControllerPointer _leftPointer;
        private AvatarPeripheral _rightHand;
        private ControllerPointer _rightPointer;
        private MapAvatarBehaviour _mapBehaviour;

        public override void InitLocal(HoloAvatar avatar)
        {
            base.InitLocal(avatar);
        }

        public override void InitRemote(HoloAvatar avatar)
        {
            _leftHand = avatar.HandVisualiser.LeftPeripheral;
            _leftPointer = _leftHand.GetComponentInChildren<ControllerPointer>();
            _rightHand = avatar.HandVisualiser.RightPeripheral;
            _rightPointer = _rightHand.GetComponentInChildren<ControllerPointer>();
            _mapBehaviour = GetComponent<MapAvatarBehaviour>();
            base.InitLocal(avatar);
        }

        public override void SerializeData(PhotonStreamQueue _stream)
        {
            MRTKPeripheralHelper left, right;
            if (MRTKPeripheralHelper.TryGetRiggedHands(out left, out right))
            {
                if (left)
                {
                    ControllerPointer pointer = left.GetComponentInChildren<ControllerPointer>();
                    _stream.SendNext(pointer.IsPointerActive);
                    _stream.SendNext(pointer.FocusWorldPosition - OZMapAppState.instance.MapInstance.GetMapScenePosition());
                }
                else
                {
                    _stream.SendNext(false);
                    _stream.SendNext(Vector3.zero);
                }
                if (right)
                {
                    ControllerPointer pointer = right.GetComponentInChildren<ControllerPointer>();
                    _stream.SendNext(pointer.IsPointerActive);
                    _stream.SendNext(pointer.FocusWorldPosition - OZMapAppState.instance.MapInstance.GetMapScenePosition());
                }
                else
                {
                    _stream.SendNext(false);
                    _stream.SendNext(Vector3.zero);
                }
            }
            else
            {
                _stream.SendNext(false);
                _stream.SendNext(Vector3.zero);
                _stream.SendNext(false);
                _stream.SendNext(Vector3.zero);
            }
        }

        public override void DeserializeData(PhotonStreamQueue _stream)
        {
            bool leftActive = (bool)_stream.ReceiveNext();
            Vector3 leftFocusPos = (Vector3)_stream.ReceiveNext();
            bool rightActive = (bool)_stream.ReceiveNext();
            Vector3 rightFocusPos = (Vector3)_stream.ReceiveNext();

            if (leftActive && !_leftPointer.IsPointerActive) _leftPointer.ActivatePointer();
            else if (!leftActive && _leftPointer.IsPointerActive) _leftPointer.DeactivatePointer();
            _leftPointer.ManualFocusPosition = _mapBehaviour.GetRemoteMapRelativePosition(leftFocusPos);

            if (rightActive && !_rightPointer.IsPointerActive) _rightPointer.ActivatePointer();
            else if (!rightActive && _rightPointer.IsPointerActive) _rightPointer.DeactivatePointer();
            _rightPointer.ManualFocusPosition = _mapBehaviour.GetRemoteMapRelativePosition(rightFocusPos);
        }

        public override void SetAvatarColor(Color color)
        {
            if (_leftPointer != null) _leftPointer.SetPointerColor(color);
            if (_rightPointer != null) _rightPointer.SetPointerColor(color);
        }

    }
}