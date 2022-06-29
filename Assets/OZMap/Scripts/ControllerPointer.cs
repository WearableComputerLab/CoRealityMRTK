using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace OZMap
{

    /// <summary>
    /// Handles a visual ray/discrete point pointer for a single controller. The pointer is 
    /// for attention directing and highlighting locations and is likely used in tandem with
    /// networking modules to transmit each user's pointer state. 
    /// 
    /// Currently used in tandem with the MRTK pointer, so this "overlays" over the top, which
    /// isn't the best solution. 
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class ControllerPointer : MonoBehaviour
    {

        [SerializeField] private GameObject _activeFocusObject;
        [SerializeField] private GameObject _passiveFocusObject;
        [SerializeField] private LayerMask _rayLayerMask;
        [SerializeField] private float _minLength;
        [SerializeField] private float _maxLength;

        [Tooltip("Button action to activate the pointer's active state.")]
        [SerializeField] private InputAction _enableAction;

        [Tooltip("Enabler held button action to enable focus length modification via GrowAxisAction.")]
        [SerializeField] private InputAction _growModifierAction;

        [Tooltip("Vector2 action to control the pointer's length.")]
        [SerializeField] private InputAction _growAxisAction;


        [Header("Focus Settings")]
        [SerializeField] private float _focusLength = 1.5f;

        [Header("Line Settings")]
        [SerializeField] private float _activeLineWidth;
        [SerializeField] private Color _activeLineColor;
        [SerializeField] private float _passiveLineWidth;
        [SerializeField] private Color _passiveLineColor;

        private bool _pointerActive = false;
        private bool _growModActive = false;
        private Vector3 _focusPos;
        private bool _manualFocusThisFrame = false;
        private LineRenderer _lineRenderer;

        public bool IsPointerActive { get { return _pointerActive; } }
        public Vector3 FocusWorldPosition { get { return _focusPos; } }
        public Vector3 ManualFocusPosition
        {
            set
            {
                _focusPos = value;
                _manualFocusThisFrame = true;
            }
        }

        private void OnEnable()
        {
            _enableAction.Enable();
            _growModifierAction.Enable();
            _growAxisAction.Enable();
        }

        private void OnDisable()
        {
            _enableAction.Disable();
            _growModifierAction.Disable();
            _growAxisAction.Disable();
        }

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _enableAction.started += (ctx) => ActivatePointer();
            _enableAction.canceled += (ctx) => DeactivatePointer();
            _growModifierAction.started += (ctx) => _growModActive = true;
            _growModifierAction.canceled += (ctx) => _growModActive = false;
        }

        private void Start()
        {
            DeactivatePointer();
        }

        private void Update()
        {
            // Keep focus object zero'd in scene space
            _activeFocusObject.transform.rotation = Quaternion.identity;
            _passiveFocusObject.transform.rotation = Quaternion.identity;

            if (!_manualFocusThisFrame)
            {
                _focusPos = transform.position + (transform.forward * _focusLength);

                // Raycast to check for map terrain layer
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, _focusLength, _rayLayerMask))
                {
                    _focusPos = hit.point;
                }
            }

            _activeFocusObject.transform.position = _focusPos;
            _passiveFocusObject.transform.position = _focusPos;
            _lineRenderer.SetPositions(new Vector3[2] { Vector3.zero, transform.InverseTransformPoint(_focusPos) });

            // Ray length modification input 
            if (_growModActive)
            {
                _focusLength = Mathf.Clamp(_focusLength + _growAxisAction.ReadValue<float>(), _minLength, _maxLength);
            }

            _manualFocusThisFrame = false;
        }

        public void ActivatePointer()
        {
            _pointerActive = true;
            _activeFocusObject.SetActive(true);
            _passiveFocusObject.SetActive(false);

            _lineRenderer.widthCurve = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, _activeLineWidth) });
            // _lineRenderer.startColor = _activeLineColor;
            // _lineRenderer.endColor = _activeLineColor;
        }

        public void DeactivatePointer()
        {
            _pointerActive = false;
            _activeFocusObject.SetActive(false);
            _passiveFocusObject.SetActive(true);

            _lineRenderer.widthCurve = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, _passiveLineWidth) });
            // _lineRenderer.startColor = _passiveLineColor;
            // _lineRenderer.endColor = _passiveLineColor;
        }

        public void SetPointerColor(Color color)
        {
            foreach (IColourSetter setter in GetComponents<IColourSetter>())
            {
                setter.SetColor(color);
            }

            foreach (IColourSetter setter in GetComponentsInChildren<IColourSetter>(true))
            {
                setter.SetColor(color);
            }
        }

    }
}
