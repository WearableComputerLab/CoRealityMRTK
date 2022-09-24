using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CoReality.Spectator
{

    public class SpectatorMove : MonoBehaviour
    {
        public SpectatorMoveEvent onSpectatorMove = new SpectatorMoveEvent();

        /// <summary>
        /// Normal speed of camera movement.
        /// </summary>
        public float _movementSpeed = 10f;

        /// <summary>
        /// Speed of camera movement when shift is held down,
        /// </summary>
        public float _fastMovementSpeed = 100f;

        /// <summary>
        /// Sensitivity for free look.
        /// </summary>
        public float _freeLookSensitivity = 3f;

        /// <summary>
        /// Amount to zoom the camera when using the mouse wheel.
        /// </summary>
        public float _zoomSensitivity = 10f;

        /// <summary>
        /// Amount to zoom the camera when using the mouse wheel (fast mode).
        /// </summary>
        public float _fastZoomSensitivity = 50f;

        /// <summary>
        /// Set to true when free looking (on right mouse button).
        /// </summary>
        private bool _looking = false;

        /// <summary>
        /// Will Allow OnSpectatorMove to be invoked
        /// </summary>
        private bool _allowUpdate = false;

        void Update()
        {
            var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var movementSpeed = fastMode ? this._fastMovementSpeed : this._movementSpeed;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);
                _allowUpdate = true;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);
                _allowUpdate = true;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime);
                _allowUpdate = true;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime);
                _allowUpdate = true;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                transform.position = transform.position + (transform.up * movementSpeed * Time.deltaTime);
                _allowUpdate = true;
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.position = transform.position + (-transform.up * movementSpeed * Time.deltaTime);
                _allowUpdate = true;
            }

            if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp))
            {
                transform.position = transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
                _allowUpdate = true;
            }

            if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown))
            {
                transform.position = transform.position + (-Vector3.up * movementSpeed * Time.deltaTime);
                _allowUpdate = true;
            }

            if (_looking)
            {
                float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * _freeLookSensitivity;
                float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * _freeLookSensitivity;
                transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
                _allowUpdate = true;
            }

            float axis = Input.GetAxis("Mouse ScrollWheel");
            if (axis != 0)
            {
                var zoomSensitivity = fastMode ? this._fastZoomSensitivity : this._zoomSensitivity;
                transform.position = transform.position + transform.forward * axis * zoomSensitivity;
                _allowUpdate = true;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StartLooking();
                _allowUpdate = true;
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                StopLooking();
                _allowUpdate = true;
            }

            if(_allowUpdate)
            {
                _allowUpdate = false;
                onSpectatorMove.Invoke(transform.localPosition, transform.localRotation);
            }

        }

        void OnDisable()
        {
            StopLooking();
        }

        /// <summary>
        /// Enable free looking.
        /// </summary>
        public void StartLooking()
        {
            _looking = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Disable free looking.
        /// </summary>
        public void StopLooking()
        {
            _looking = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    [System.Serializable]
    public class SpectatorMoveEvent : UnityEvent<Vector3, Quaternion> {  }
}
