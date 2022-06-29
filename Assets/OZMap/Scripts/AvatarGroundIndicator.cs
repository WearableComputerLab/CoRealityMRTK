using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mapbox.Utils;
using UnityEngine;

namespace OZMap.Networking
{
    /// <summary>
    /// Simple indicator component designed to show a line from the avatar's position to the ground.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class AvatarGroundIndicator : MonoBehaviour
    {

        [SerializeField] private GameObject _groundObject;
        [SerializeField] private bool _useParentAsScaleRef = false;
        [SerializeField] private int _parentLevels = 1;
        [SerializeField] private Transform _scaleReference;
        [SerializeField] private Transform _avatarReference;

        private LineRenderer _lineRenderer;
        private bool _minimised = false;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (_useParentAsScaleRef)
            {
                Transform parent = transform.parent;
                for (int i = 0; i < _parentLevels; i++)
                {
                    parent = parent.parent;
                }
                _scaleReference = parent;
            }
        }

        void Update()
        {
            // Set position of ground based on Mapbox
            Vector2d mapCoords = OZMapAppState.instance.MapInstance.GetCoordinatesFromWorldPos(_avatarReference.position);
            Vector3 groundPos = OZMapAppState.instance.MapInstance.GetWorldMapPositionFromLatLong(mapCoords, true);
            _groundObject.transform.position = groundPos;

            // Clamp ground object rotation to prevent it from rotating
            _groundObject.transform.rotation = Quaternion.identity;

            // Keep ground object world scale to 1 regardless of avatar scale
            _groundObject.transform.localScale = Vector3.one / _scaleReference.localScale.x;

            // Set line vertex positions based on avatar's position and groundObject's position
            // Only set line if line is greater than minimum size
            if (Vector3.Distance(_avatarReference.position, groundPos) > 0.1f)
            {
                if (_minimised)
                {
                    _minimised = false;
                }
                _lineRenderer.SetPositions(new Vector3[] { groundPos, _avatarReference.position });
            }
            else
            {
                if (!_minimised)
                {
                    _minimised = true;
                }
                _lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
            }
        }

    }
}
