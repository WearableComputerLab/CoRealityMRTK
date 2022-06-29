using System.Collections.Generic;
using CoReality.Avatars;
using Mapbox.Utils;
using Photon.Pun;
using UnityEngine;

namespace OZMap.Networking
{
    [RequireComponent(typeof(MapAvatarBehaviour))]
    public class MapAnnotationAvatarBehaviour : AvatarNetBehaviour
    {

        [SerializeField] private MapAnnotationSegment _lineSegmentPrefab;

        private AnnotationModule _annotationModule;
        private Dictionary<int, MapAnnotationSegment> _segmentRegister = new Dictionary<int, MapAnnotationSegment>();
        private Color _playerColor;
        private PhotonView _photonView;
        private MapAvatarBehaviour _mapBehaviour;

        public override void InitLocal(HoloAvatar avatar)
        {
            _photonView = GetComponent<PhotonView>();
            _annotationModule = FindObjectOfType<AnnotationModule>();
            _annotationModule.OnDrawFinished.AddListener(OnDrawFinished);
            _annotationModule.OnSegmentDeleted.AddListener(OnSegmentDeleted);
            base.InitLocal(avatar);
        }

        public override void InitRemote(HoloAvatar avatar)
        {
            _annotationModule = FindObjectOfType<AnnotationModule>();
            _mapBehaviour = GetComponent<MapAvatarBehaviour>();
            base.InitRemote(avatar);
        }

        public override void SerializeData(PhotonStreamQueue _stream)
        {
            _stream.SendNext(_annotationModule.IsDrawing);
            _stream.SendNext(_annotationModule.DrewThisFrame);

            _stream.SendNext(_annotationModule.LastDrawPos - OZMapAppState.instance.MapInstance.GetMapScenePosition());
            _stream.SendNext(_annotationModule.CurrentSegmentID);
        }

        public override void DeserializeData(PhotonStreamQueue _stream)
        {
            bool isDrawing = (bool)_stream.ReceiveNext();
            bool drewThisFrame = (bool)_stream.ReceiveNext();
            Vector3 drawPos = (Vector3)_stream.ReceiveNext();
            int segmentId = (int)_stream.ReceiveNext();

            if (!isDrawing || !drewThisFrame) return;

            MapAnnotationSegment currentSegment = null;
            Vector3 relativeDrawPos = _mapBehaviour.GetRemoteMapRelativePosition(drawPos);

            // If segment not exist yet, create new segment or otherwise use registered line
            if (!_segmentRegister.ContainsKey(segmentId))
            {
                Vector2d anchor = OZMapAppState.instance.MapInstance.GetCoordinatesFromWorldPos(relativeDrawPos);
                currentSegment = CreateSegment(anchor, segmentId);
            }
            else
            {
                currentSegment = _segmentRegister[segmentId];
            }

            if (!currentSegment.ManualVerticesSet) currentSegment.AddVert(relativeDrawPos);
        }

        public override void SetAvatarColor(Color color)
        {
            _playerColor = color;
        }

        /// <summary>
        /// Will fire on local avatar when a line segment has been draw, triggering a sync across
        /// all remote avatars for that segment to ensure their version of the segment is accurate.
        /// </summary>
        private void OnDrawFinished()
        {
            int segmentId = _annotationModule.CurrentSegmentID;
            Vector2d anchor = _annotationModule.CurrentOrLastSegment.LatitudeLongitude;
            Vector3[] verts = _annotationModule.CurrentOrLastSegment.Verts.ToArray();
            _photonView.RPC(nameof(SyncLine), RpcTarget.Others, segmentId, anchor.x, anchor.y, verts);
        }

        private void OnSegmentDeleted(MapAnnotationSegment segment)
        {
            _photonView.RPC(nameof(DeleteLine), RpcTarget.Others, segment.SegmentID);
        }

        [PunRPC]
        private void SyncLine(int segmentId, double anchorLat, double anchorLong, Vector3[] verts)
        {
            MapAnnotationSegment segment;
            Vector2d anchor = new Vector2d(anchorLat, anchorLong);

            if (!_segmentRegister.ContainsKey(segmentId))
            {
                // Create new segment if not created yet
                segment = CreateSegment(anchor, segmentId);
            }
            else
            {
                // If already exists replace anchor with sync anchor
                segment = _segmentRegister[segmentId];
                segment.SetLatLong(anchor);
            }

            // Update verts with synced verts, which are already in local segment space
            segment.SetVerts(verts, false);
        }

        [PunRPC]
        private void DeleteLine(int segmentId)
        {
            if (!_segmentRegister.ContainsKey(segmentId)) return;
            MapAnnotationSegment mapAnnotationSegment = _segmentRegister[segmentId];
            _segmentRegister.Remove(mapAnnotationSegment.SegmentID);
            OZMapAppState.instance.MapInstance.RemoveMapFeature(mapAnnotationSegment);
            Destroy(mapAnnotationSegment.gameObject);
        }

        private MapAnnotationSegment CreateSegment(Vector2d anchor, int segmentId)
        {
            MapAnnotationSegment segment = Instantiate(_lineSegmentPrefab, transform);
            segment.GetComponent<LineRendererColourSetter>().SetColor(_playerColor);
            segment.SegmentID = segmentId;
            segment.IsLocal = false;
            _segmentRegister[segmentId] = segment;

            // Generate initial anchor point for segment and register with map
            segment.SetLatLong(anchor);
            OZMapAppState.instance.MapInstance.AddMapFeature(segment, false);

            // Initialise segment at correct map position so first point will be placed correctly
            segment.transform.position = OZMapAppState.instance.MapInstance.GetWorldMapPositionFromLatLong(anchor, false);
            segment.transform.localScale = OZMapAppState.instance.MapInstance.GetMapScale();

            return segment;
        }

    }
}