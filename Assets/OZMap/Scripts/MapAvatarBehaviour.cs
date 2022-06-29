using CoReality.Avatars;
using Mapbox.Utils;
using Photon.Pun;
using UnityEngine;

namespace OZMap.Networking
{
    public class MapAvatarBehaviour : AvatarNetBehaviour
    {


        // Scale to display remote avatar at relative to their local scale
        private Vector3 remoteRelativeAvatarScale;

        // Converted (unscaled) world pos of remote user's map center
        private Vector3 remoteMapRelativeAvatarPos;

        public override void InitLocal(HoloAvatar avatar)
        {
            base.InitLocal(avatar);
        }

        public override void InitRemote(HoloAvatar avatar)
        {
            base.InitRemote(avatar);
        }

        public override void SerializeData(PhotonStreamQueue _stream)
        {
            AbstractMapInstance mapInstance = OZMapAppState.instance.MapInstance;
            Vector2d mapCenter = mapInstance.GetCenterLatLong();

            _stream.SendNext(mapInstance.GetMapScale().x);
            _stream.SendNext(mapCenter.x);
            _stream.SendNext(mapCenter.y);
            _stream.SendNext(Avatar.HeadWorldPos - mapInstance.GetMapScenePosition());
        }

        public override void DeserializeData(PhotonStreamQueue _stream)
        {
            AbstractMapInstance mapInstance = OZMapAppState.instance.MapInstance;

            float remoteMapScale = (float)_stream.ReceiveNext();
            Vector2d mapCenter = new Vector2d((double)_stream.ReceiveNext(), (double)_stream.ReceiveNext());
            Vector3 remoteMapOffsetPos = (Vector3)_stream.ReceiveNext();

            // Avatar scale
            Vector3 avatarScale = mapInstance.GetMapScale() / remoteMapScale;
            Avatar.Head.transform.localScale = avatarScale;

            // Avatar position
            Vector3 avatarPosition;
            remoteMapRelativeAvatarPos = avatarPosition = mapInstance.GetWorldMapPositionFromLatLong(mapCenter);// + mapInstance.GetMapScenePosition();
            avatarPosition += remoteMapOffsetPos * avatarScale.x;
            Avatar.Head.transform.localPosition = avatarPosition;

            remoteRelativeAvatarScale = avatarScale;
        }

        // MapOffsetPos must be formatted locally as offset from local map origin
        public Vector3 GetRemoteMapRelativePosition(Vector3 mapOffsetPos)
        {
            Vector3 relativePos = remoteMapRelativeAvatarPos;
            relativePos += mapOffsetPos * remoteRelativeAvatarScale.x;
            return relativePos;
        }

    }
}