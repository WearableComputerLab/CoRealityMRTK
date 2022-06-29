using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OZMap
{
    [RequireComponent(typeof(LineRenderer))]
    public class MapAnnotationSegment : MapFeature
    {

        private LineRenderer _lineRenderer;
        private LineRendererColourSetter _colourSetter;
        private int _lineId;
        private List<Vector3> _lineVerts = new List<Vector3>();
        private BoxCollider _collider;
        private bool _isLocal;
        private bool _manualVerticesSet = false;

        public Color LineColor { set => _colourSetter.SetColor(value); }
        public List<Vector3> Verts { get => _lineVerts; }
        public int SegmentID { get => _lineId; set => _lineId = value; }
        public bool IsLocal { get => _isLocal; set => _isLocal = value; }
        public int VertCount { get => _lineVerts.Count; }
        public bool ManualVerticesSet { get => _manualVerticesSet; set => _manualVerticesSet = value; }

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _colourSetter = GetComponent<LineRendererColourSetter>();
            _collider = gameObject.AddComponent<BoxCollider>();
        }

        public override void InitFeature()
        {
        }

        public override void UpdatePosition(Vector3 worldPos, Quaternion mapRot, Vector3 mapScale, float zoomLevel)
        {
            transform.position = worldPos;
            transform.localScale = mapScale;
        }

        public void AddVert(Vector3 vert, bool worldSpace = true)
        {
            if (worldSpace) vert = transform.InverseTransformPoint(vert);
            _lineVerts.Add(vert);
            _lineRenderer.positionCount = _lineVerts.Count;
            _lineRenderer.SetPositions(_lineVerts.ToArray());
            UpdateColliderBounds();
        }

        public void SetVerts(Vector3[] verts, bool worldSpace = true)
        {
            if (worldSpace)
            {
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i] = transform.InverseTransformPoint(verts[i]);
                }
            }
            _lineVerts = verts.ToList();
            _lineRenderer.positionCount = _lineVerts.Count;
            _lineRenderer.SetPositions(_lineVerts.ToArray());
            UpdateColliderBounds();
            _manualVerticesSet = true;
        }

        public void UpdateColliderBounds()
        {
            Bounds segmentBounds = _lineRenderer.bounds;
            _collider.center = transform.InverseTransformPoint(segmentBounds.center);
            _collider.size = segmentBounds.size;
        }



    }

}
