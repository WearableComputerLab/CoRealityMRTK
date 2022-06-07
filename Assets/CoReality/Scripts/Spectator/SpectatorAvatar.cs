using CoReality.Avatars;
using Photon.Pun;
using UnityEngine;

namespace CoReality.Spectator
{
    public class SpectatorAvatar : AvatarBase
    {

        private GameObject _cameraModel;

        public override AvatarBase Initalize(bool remote)
        {
            transform.SetParent(NetworkModule.NetworkOrigin);

            //Just place where the spectator is
            transform.localPosition = SpectatorRig.Instance.transform.localPosition;
            transform.localRotation = SpectatorRig.Instance.transform.localRotation;

            if (remote)
            {
                //For now just create a primative to indicate the location
                _cameraModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                _cameraModel.name = "Spectator Indicator";
                _cameraModel.transform.parent = NetworkModule.NetworkOrigin;
                _cameraModel.transform.localPosition = transform.localPosition;
            }
            else { }

            _isInitalized = true;

            return this;
        }

        public override void SerializeData()
        {

        }

        public override void DeserializeData()
        {

        }

        public override void Destroy()
        {

        }

        [PunRPC]
        protected new void PropertyChangedRPC(string property, object value)
        {
            base.PropertyChangedRPC(property, value);

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