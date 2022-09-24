using CoReality.Avatars;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using UnityEngine;

namespace CoReality.Spectator
{
    public class SpectatorAvatar : AvatarBase
    {

        [SerializeField]
        private GameObject _cameraModel;

        private BoxCollider _moveCollider;

        private ObjectManipulator _objectManipulator;

        private bool _manipStarted = false;

        public override AvatarBase Initalize(bool remote)
        {
            transform.SetParent(NetworkModule.NetworkOrigin);

            name = (remote ? "Remote" : "Local") + "Spectator";

            //Is local
            if (!remote)
            {
                //Disable camera model reference
                _cameraModel.gameObject.SetActive(false);
                //Just place where the spectator is
                transform.localPosition = SpectatorRig.Instance.transform.localPosition;
                transform.localRotation = SpectatorRig.Instance.transform.localRotation;

                //Todo: move into OnEnable/OnDisable (maybe)
                SpectatorRig.Instance.SpectatorMove.onSpectatorMove.AddListener(OnLocalSpectatorMove);
            }
            //Is remote
            else
            {
                //Create the Objectmanipulator so that this object can be moved by a
                //hololens user
                _moveCollider = gameObject.AddComponent<BoxCollider>();
                _moveCollider.size = new Vector3(0.2f, 0.2f, 0.2f);

                _objectManipulator = gameObject.AddComponent<ObjectManipulator>();
                _objectManipulator.TwoHandedManipulationType = 
                    Microsoft.MixedReality.Toolkit.Utilities.TransformFlags.Move |
                    Microsoft.MixedReality.Toolkit.Utilities.TransformFlags.Rotate;

                //Event listeners
                //Todo: move to OnEnable/OnDisable
                _objectManipulator.OnManipulationStarted.AddListener(HandleManipulationStarted);
                _objectManipulator.OnManipulationEnded.AddListener(HandleManipulationEnded);

                gameObject.AddComponent<NearInteractionGrabbable>();
            }

            _isInitalized = true;

            return this;
        }

        protected override void Update()
        {
            base.Update();

            if(_manipStarted)
            {
                HandleManipulation(_objectManipulator.HostTransform);
            }
        }

        /// <summary>
        /// Handle local transformation of the spectator rig
        /// </summary>
        private void OnLocalSpectatorMove(Vector3 position, Quaternion rotation)
        {
            photonView.RPC(
              nameof(TransformRPC),
              RpcTarget.Others,
              position,
              rotation,
              false
          );
        }

        private void HandleManipulationStarted(ManipulationEventData args)
        {
            _manipStarted = true;
        }

        /// <summary>
        /// Handle remote transformation of the spectator rig postion
        /// allowing hololens users to move the cameras
        /// </summary>
        private void HandleManipulation(Transform hostTransform)
        {
            photonView.RPC(
                nameof(TransformRPC),
                RpcTarget.Others,
                hostTransform.localPosition,
                hostTransform.localRotation,
                true
            );
        }

        private void HandleManipulationEnded(ManipulationEventData args)
        {
            _manipStarted = false;
        }

        /// <summary>
        /// Gets the Head Transform of this spectator
        /// </summary>
        /// <returns></returns>
        public override Transform GetHeadTransform()
        {
            return transform;
        }

        public override void SerializeData()
        {

        }

        public override void DeserializeData()
        {

        }

        public override void Destroy()
        {
            //Clean up all listeners
            OnPropertyChanged.RemoveAllListeners();
            Destroy(gameObject);
        }

        /// <summary>
        /// Since we are allowing other users to move this camera we need
        /// control movement of this photonView through an RPC so that
        /// it is two-way.
        /// </summary>
        [PunRPC]
        private void TransformRPC(Vector3 position, Quaternion rotation, bool moveRig)
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
            //move the rig if moved remotely (by hololens user)
            if(moveRig)
            {
                SpectatorRig.Instance.transform.localPosition = position;
                SpectatorRig.Instance.transform.localRotation = rotation;
            }
        }

        [PunRPC]
        protected override void PropertyChangedRPC(string property, object value)
        {
            switch (property)
            {
                case nameof(Color):
                    Vector3 col = (Vector3)value;
                    Color = new Color(col.x, col.y, col.z);
                    _cameraModel.GetComponent<MeshRenderer>().material.color = Color;
                    break;
                case nameof(Name):
                    Name = (string)value;
                    break;
            }
        }
    }
}