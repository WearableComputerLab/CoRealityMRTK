using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace CoReality.Avatars
{
    public abstract class AvatarBase : MonoBehaviourPun, IPunObservable
    {
        protected bool _isInitalized = false;


        /// <summary>
        /// Is this HoloAvatar local or remote?
        /// </summary>
        /// <value></value>
        public bool AmController
        {
            get => photonView.AmController;
        }

        protected string _name;

        /// <summary>
        /// The name of the avatar (dont confuse with GameObject.name)
        /// </summary>
        /// <value></value>
        public string Name
        {
            get => _name;
            set
            {
                _name = value + (AmController ? " (you)" : "");
                PropertyChanged(nameof(Name), value);
            }
        }

        protected Color _color;

        /// <summary>
        /// Sets the color of this Avatar
        /// </summary>
        /// <value></value>
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;

                //
                //Have to convert color to Vector3 so it can be sent over photonNetwork
                PropertyChanged(nameof(Color), new Vector3(value.r, value.g, value.b));
            }
        }

        //---------------------------------

        protected PhotonStreamQueue _streamQueue = new PhotonStreamQueue(120);

        protected PropertyChangedEvent _onPropertyChanged = new PropertyChangedEvent();

        public PropertyChangedEvent OnPropertyChanged
        {
            get => _onPropertyChanged;
        }

        /// <summary>
        /// Invokes onPropertyChanged event for local updates
        /// And Invokes PropertyChangedRPC for remote updates
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        protected void PropertyChanged(string property, object value)
        {
            //Update locally this will call OnAvatarPropertyChanged in AvatarModule
            _onPropertyChanged?.Invoke(property, value);
            //Invoke property change RPC if we are controller
            if (AmController)
            {
                photonView.RPC(
                    nameof(PropertyChangedRPC),
                    RpcTarget.OthersBuffered,
                    property,
                    value
                );
            }
        }

        void Update()
        {
            if (!_isInitalized) return;

            //Ensure in photon room else reset and return
            if (!PhotonNetwork.InRoom)
            {
                _streamQueue.Reset();
                return;
            }

            //Serialize data if owner, else deserialize it
            if (AmController && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                this.SerializeData();
            }
            else if (_streamQueue.HasQueuedObjects())
                this.DeserializeData();

        }

        #region Photon

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting == true)
            {
                this._streamQueue.Serialize(stream);
            }
            else
            {
                this._streamQueue.Deserialize(stream);
            }
        }

        /// <summary>
        /// Ensure you call base if this is overriden
        /// </summary>
        protected abstract void PropertyChangedRPC(string property, object value);

        #endregion

        public abstract AvatarBase Initalize(bool remote);

        public abstract void SerializeData();

        public abstract void DeserializeData();

        public abstract void Destroy();
    }

    public class PropertyChangedEvent : UnityEvent<string, object> { }

}