using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.Events;

namespace CoReality.Avatars
{
    [RequireComponent(
        typeof(PhotonView)
    )]
    public class HoloAvatar : AvatarBase
    {
        //Reference objects for local player 
        private GameObject _headRef, _lHandRef, _rHandRef;

        //Remote avatar objects, only created for remote HoloAvatars

        [SerializeField]
        private AvatarHead _headPrefab;
        private AvatarHead _head;

        public AvatarHead Head
        {
            get => _head;
        }

        /// <summary>
        /// Gets the current head position of this
        /// avatar in local-space
        /// (Its location relative to the networked origin)
        /// </summary>
        public Vector3 HeadLocalPosition
        {
            get
            {
                if (AmController)
                    return _headRef.transform.localPosition;
                else
                    return _head.transform.localPosition;
            }
        }

        [SerializeField]
        private AvatarRiggedHand _lHandPrefab;
        private AvatarRiggedHand _leftHand;

        public AvatarRiggedHand LeftHand
        {
            get => _leftHand;
        }

        [SerializeField]
        private AvatarRiggedHand _rHandPrefab;
        private AvatarRiggedHand _rightHand;

        public AvatarRiggedHand RightHand
        {
            get => _rightHand;
        }

        private bool _isRemote = false;

        //---------------------------------------------

        /// <summary>
        /// Initalize this HoloAvatar
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="parent"></param>
        /// <param name="color"></param>
        public override AvatarBase Initalize(bool remote)
        {
            _isRemote = remote;

            transform.SetParent(NetworkModule.NetworkOrigin);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = NetworkModule.NetworkOrigin.localScale;


            name = (remote ? "Remote" : "Local") + "Avatar";

            if (!remote)
            {
                //Spawn Local reference objects
                //Spawn the reference objects (for positioning)
                _headRef = new GameObject("__HeadReference");
                _lHandRef = new GameObject("__LeftHandReference");
                _rHandRef = new GameObject("__RightHandReference");
                _headRef.transform.parent =
                _lHandRef.transform.parent =
                _rHandRef.transform.parent =
                NetworkModule.NetworkOrigin;

            }
            else
            {
                //Spawn Remote Objects
                //instantiate the remote objects for this avatar
                _head = Instantiate(_headPrefab, Vector3.zero, Quaternion.identity, transform);

                //instantiate hands
                _rightHand = Instantiate(_rHandPrefab);
                _leftHand = Instantiate(_lHandPrefab);
                _rightHand.transform.SetParent(transform);
                _leftHand.transform.SetParent(transform);
                _rightHand.transform.localPosition = _leftHand.transform.localPosition = Vector3.zero;
                _rightHand.transform.localRotation = _leftHand.transform.localRotation = Quaternion.identity;

                //Set the default hand material if its not null
                if (AvatarModule.DefaultHandMaterial)
                    _rightHand.MeshRenderer.material = _leftHand.MeshRenderer.material = AvatarModule.DefaultHandMaterial;
            }

            _isInitalized = true;

            return this;
        }

        public override void SerializeData()
        {
            //HEAD
            _headRef.transform.position = Camera.main.transform.position;
            _headRef.transform.rotation = Camera.main.transform.rotation;

            _streamQueue.SendNext(_headRef.transform.localPosition);
            _streamQueue.SendNext(_headRef.transform.localRotation);

            //HANDS
            MRTKRiggedHandHelper left, right;
            if (MRTKRiggedHandHelper.TryGetRiggedHands(out left, out right))
            {
                if (left)
                {
                    _lHandRef.transform.position = left.AvatarRiggedHands.Root.position;
                    _lHandRef.transform.rotation = left.AvatarRiggedHands.Root.rotation;
                    HandPose pose = new HandPose(left.AvatarRiggedHands, _lHandRef.transform.localPosition, _lHandRef.transform.localRotation);

                    _streamQueue.SendNext(pose);
                }
                else
                    _streamQueue.SendNext(new HandPose { IsLeft = true, IsActive = false });

                if (right)
                {
                    _rHandRef.transform.position = right.AvatarRiggedHands.Root.position;
                    _rHandRef.transform.rotation = right.AvatarRiggedHands.Root.rotation;
                    HandPose pose = new HandPose(right.AvatarRiggedHands, _rHandRef.transform.localPosition, _rHandRef.transform.localRotation);
                    _streamQueue.SendNext(pose);
                }
                else
                    _streamQueue.SendNext(new HandPose { IsLeft = false, IsActive = false });
            }
            else
            {
                //Always send a hand pose even if we don't have hands active
                _streamQueue.SendNext(new HandPose { IsLeft = true, IsActive = false });
                _streamQueue.SendNext(new HandPose { IsLeft = false, IsActive = false });
            }
        }

        public override void DeserializeData()
        {
            //HEAD
            _head.transform.localPosition = (Vector3)_streamQueue.ReceiveNext();
            _head.transform.localRotation = (Quaternion)_streamQueue.ReceiveNext();

            //HANDS
            HandPose leftPose = (HandPose)_streamQueue.ReceiveNext();
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

            HandPose rightPose = (HandPose)_streamQueue.ReceiveNext();
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


        public override void Destroy()
        {
            //Clean up all listeners
            OnPropertyChanged.RemoveAllListeners();
            //We have to check isRemote here AmController won't work in this
            //instance since controller switches to another player once connection
            //is lost from the leaving player
            //FIXME: Come up with a better solution for the above (but _isRemote works for now)
            if (!_isRemote)
            {
                Destroy(_headRef.gameObject);
                Destroy(_lHandRef.gameObject);
                Destroy(_rHandRef.gameObject);
            }
            Destroy(gameObject);
        }

        #region RPCs


        [PunRPC]
        protected override void PropertyChangedRPC(string property, object value)
        {
            //Todo use reflection to get prop names
            switch (property)
            {
                case nameof(Color):
                    Vector3 hah = (Vector3)value;
                    Color = new Color(hah.x, hah.y, hah.z);
                    _rightHand.MeshRenderer.material.color = Color;
                    _leftHand.MeshRenderer.material.color = Color;
                    _head.GetComponentInChildren<MeshRenderer>().material.color = Color;
                    break;
                case nameof(Name):
                    Name = (string)value;
                    _head.Name = Name;
                    break;
            }
        }


        #endregion

    }


}