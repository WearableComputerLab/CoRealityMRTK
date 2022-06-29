using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace OZMap
{

    public class AnnotationModule : MonoBehaviour
    {

        public UnityEvent OnDrawFinished;
        public AnnotationSegmentEvent OnSegmentDeleted;

        [SerializeField] private GameObject _drawCursorLeft;
        [SerializeField] private GameObject _drawCursorRight;
        [SerializeField] private MapAnnotationSegment _drawSegmentPrefab;
        [SerializeField] private LayerMask _segmentLayers;

        [Header("Input Settings")]
        [SerializeField] private InputAction _leftHandDrawAction;
        [SerializeField] private InputAction _leftHandDeleteAction;
        [SerializeField] private InputAction _rightHandDrawAction;
        [SerializeField] private InputAction _rightHandDeleteAction;

        [Header("Draw Settings")]
        [SerializeField] private float _deleteRadius;
        [SerializeField] private float _minDrawDistance;
        [SerializeField] private float _minDrawTime;
        [SerializeField] private float _minDrawTimeDistance;

        [Header("Visual Settings")]
        [SerializeField] private Color _drawingColor;
        [SerializeField] private Color _deletingColor;
        [SerializeField] private Color _passiveColor;

        private Dictionary<int, MapAnnotationSegment> _segmentRegister = new Dictionary<int, MapAnnotationSegment>();
        private int _currentSegmentId = 0;
        private bool _isDrawing = false;
        private bool _isDeleting = false;
        private MapAnnotationSegment _currentSegment;

        // Drawing
        private GameObject _currentDrawCursor;
        private Vector3 _lastDrawPos;
        private float _drawTime;
        private bool _drewThisFrame;
        private int _drawnVertCount;

        public MapAnnotationSegment CurrentOrLastSegment { get => _currentSegment; }
        public bool IsDrawing { get => _isDrawing; }
        public bool DrewThisFrame { get => _drewThisFrame; }
        public Vector3 LastDrawPos { get => _lastDrawPos; }
        public int CurrentSegmentID { get => _currentSegmentId; }
        public int DrawnVertCount { get => _drawnVertCount; }

        private void Awake()
        {
            _rightHandDrawAction.started += (ctx) =>
            {
                SetHandedness(Handedness.Right);
                StartDrawing();
            };
            _rightHandDrawAction.canceled += (ctx) => EndDrawing();
            _rightHandDeleteAction.started += (ctx) =>
            {
                SetHandedness(Handedness.Right);
                StartDeleting();
            };
            _rightHandDeleteAction.canceled += (ctx) => EndDeleting();

            _leftHandDrawAction.started += (ctx) =>
            {
                SetHandedness(Handedness.Left);
                StartDrawing();
            };
            _leftHandDrawAction.canceled += (ctx) => EndDrawing();
            _leftHandDeleteAction.started += (ctx) =>
            {
                SetHandedness(Handedness.Left);
                StartDeleting();
            };
            _leftHandDeleteAction.canceled += (ctx) => EndDeleting();
        }

        private void Start()
        {
            _currentDrawCursor = _drawCursorLeft;
            SetCursorColor(_passiveColor);
        }

        private void Update()
        {
            // Drawing ----------------------------------------------------------------------------------------------------
            _drewThisFrame = false;
            Vector3 drawPos = _currentDrawCursor.transform.position;
            if (_isDrawing && _currentSegment != null)
            {
                _drawTime += Time.deltaTime;

                float distanceDelta = Vector3.Distance(drawPos, _lastDrawPos);

                // Check if we should create a new point from our drawing (two possibilities)
                if (distanceDelta >= _minDrawDistance)
                {
                    // We've moved cursor far enough to draw a new point
                    AddNewPoint();
                }
                else if (_drawTime >= _minDrawTime && distanceDelta >= _minDrawTimeDistance)
                {
                    // Enough time has elapsed and a "time min distance" value has been passed
                    AddNewPoint();
                }
            }

            // Deleting ---------------------------------------------------------------------------------------------------
            if (_isDeleting)
            {
                // Do raycast to check if we are overlapping any segments
                Collider[] hitColliders = Physics.OverlapSphere(_currentDrawCursor.transform.position, _deleteRadius, _segmentLayers);
                List<MapAnnotationSegment> toDelete = new List<MapAnnotationSegment>();
                foreach (Collider col in hitColliders)
                {
                    MapAnnotationSegment segment = col.GetComponent<MapAnnotationSegment>();
                    if (segment != null)
                    {
                        // We don't want segments from other users
                        if (!segment.IsLocal) break;

                        // We are overlapping a segment, sweep vertices and delete if overlapping a vertex
                        foreach (Vector3 vert in segment.Verts)
                        {
                            if (Vector3.Distance(segment.transform.TransformPoint(vert), _currentDrawCursor.transform.position) <= _deleteRadius)
                            {
                                // We are overlapping a vertex, delete the segment
                                toDelete.Add(segment);
                                break;
                            }
                        }
                    }
                }

                for (int i = toDelete.Count - 1; i >= 0; i--)
                {
                    DeleteSegment(toDelete[i]);
                }

            }

        }

        public void StartDrawing()
        {
            _currentSegmentId++;
            _currentSegment = Instantiate(_drawSegmentPrefab, transform);
            _segmentRegister[_currentSegmentId] = _currentSegment;
            _currentSegment.SegmentID = _currentSegmentId;
            _currentSegment.IsLocal = true;
            _currentSegment.LineColor = OZMapAppState.instance.PlayerColor;

            // Generate initial anchor point for segment and register with map
            Vector2d anchor = OZMapAppState.instance.MapInstance.GetCoordinatesFromWorldPos(_currentDrawCursor.transform.position);
            _currentSegment.SetLatLong(anchor);
            OZMapAppState.instance.MapInstance.AddMapFeature(_currentSegment, false);

            // Initialise segment at correct map position so first point will be placed correctly
            _currentSegment.transform.position = OZMapAppState.instance.MapInstance.GetWorldMapPositionFromLatLong(anchor, false);
            _currentSegment.transform.localScale = OZMapAppState.instance.MapInstance.GetMapScale();

            // Place line initial point
            AddNewPoint();

            SetCursorColor(_drawingColor);
            _currentDrawCursor.transform.DOScale(1.4f, 0.5f).SetEase(Ease.OutBack);
            _isDrawing = true;
        }

        public void EndDrawing()
        {
            if (_currentSegment.VertCount > 2)
            {
                // Add final point to segment
                AddNewPoint();
            }

            OnDrawFinished?.Invoke();
            _isDrawing = false;
            _drawnVertCount = 0;
            SetCursorColor(_passiveColor);
            _currentDrawCursor.transform.DOScale(1.0f, 0.5f).SetEase(Ease.OutBack);
        }

        private void DeleteSegment(MapAnnotationSegment mapAnnotationSegment)
        {
            if (_segmentRegister.ContainsKey(mapAnnotationSegment.SegmentID))
            {
                _segmentRegister.Remove(mapAnnotationSegment.SegmentID);
                OZMapAppState.instance.MapInstance.RemoveMapFeature(mapAnnotationSegment);
                Destroy(mapAnnotationSegment.gameObject);
                OnSegmentDeleted?.Invoke(mapAnnotationSegment);
            }
        }

        private void AddNewPoint()
        {
            _drawTime = 0f;
            _drawnVertCount++;
            _lastDrawPos = _currentDrawCursor.transform.position;
            _currentSegment.AddVert(_currentDrawCursor.transform.position);
            _drewThisFrame = true;
        }

        private void StartDeleting()
        {
            _isDeleting = true;
            SetCursorColor(_deletingColor);
            _currentDrawCursor.transform.DOScale(1.4f, 0.5f).SetEase(Ease.OutBack);
        }

        private void EndDeleting()
        {
            _isDeleting = false;
            SetCursorColor(_passiveColor);
            _currentDrawCursor.transform.DOScale(1.0f, 0.5f).SetEase(Ease.OutBack);
        }

        private void SetCursorColor(Color color)
        {
            foreach (MeshRenderer renderer in _currentDrawCursor.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material.color = color;
            }
        }

        private void SetHandedness(Handedness handedness)
        {
            if (handedness == Handedness.Left)
            {
                _currentDrawCursor = _drawCursorLeft;
                _drawCursorLeft.transform.localScale = Vector3.one;
                _drawCursorRight.transform.localScale = Vector3.zero;
            }
            else
            {
                _currentDrawCursor = _drawCursorRight;
                _drawCursorLeft.transform.localScale = Vector3.zero;
                _drawCursorRight.transform.localScale = Vector3.one;
            }
        }

        private void OnEnable()
        {
            _rightHandDrawAction.Enable();
            _rightHandDeleteAction.Enable();
            _leftHandDrawAction.Enable();
            _leftHandDeleteAction.Enable();
        }

        private void OnDisable()
        {
            _rightHandDrawAction.Disable();
            _rightHandDeleteAction.Disable();
            _leftHandDrawAction.Disable();
            _leftHandDeleteAction.Disable();
        }

    }

    [System.Serializable]
    public class AnnotationSegmentEvent : UnityEvent<MapAnnotationSegment> { }

}
