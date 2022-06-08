using CoReality.Avatars;
using Photon.Pun;
using UnityEngine;

namespace CoReality.Spectator
{
    public class SpectatorAvatar : AvatarBase
    {

        [SerializeField]
        private GameObject _cameraModel;

        public override AvatarBase Initalize(bool remote)
        {
            transform.SetParent(NetworkModule.NetworkOrigin);

            name = (remote ? "Remote" : "Local") + "Spectator";

            if (!remote)
            {
                //Disable camera model reference
                _cameraModel.gameObject.SetActive(false);
                //Just place where the spectator is
                transform.localPosition = SpectatorRig.Instance.transform.localPosition;
                transform.localRotation = SpectatorRig.Instance.transform.localRotation;
            }
            else
            {
                //Do nothing for now (Rig just sits in static position)
            }

            _isInitalized = true;

            return this;
        }

        public override void SerializeData()
        {
            _streamQueue.SendNext(transform.localPosition);
            _streamQueue.SendNext(transform.localRotation);
        }

        public override void DeserializeData()
        {
            transform.localPosition = (Vector3)_streamQueue.ReceiveNext();
            transform.localRotation = (Quaternion)_streamQueue.ReceiveNext();
        }

        public override void Destroy()
        {
            //Clean up all listeners
            OnPropertyChanged.RemoveAllListeners();
            Destroy(gameObject);
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