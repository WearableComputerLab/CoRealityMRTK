using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoReality.Examples
{

    /// <summary>
    /// Simple camera controller for desktop mode
    /// </summary>
    public class CameraController : MonoBehaviour
    {

        [SerializeField, Range(0.0f, 1.0f)]
        float _lerpRate = .5f;

        [SerializeField]
        float _speedMultiplier = 2f;

        float _xRotation;
        float _yRotation;

        public void Rotate(float x, float y)
        {
            _xRotation = x * _speedMultiplier;
            _yRotation = y * _speedMultiplier;
        }

        public void Position(Vector3 pos)
        {
            transform.position = pos;
        }

        public void Zoom(float amount)
        {

        }

        void Update()
        {
            if (Input.GetMouseButton(0)) { }
            else if (Input.GetMouseButton(1)) { }
            else if (Input.GetMouseButton(2))
                Rotate(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        void LateUpdate()
        {
            _xRotation = Mathf.Lerp(_xRotation, 0, _lerpRate);
            _yRotation = Mathf.Lerp(_yRotation, 0, _lerpRate);
            transform.eulerAngles += new Vector3(-_yRotation, _xRotation, 0);
        }


        void OnDestroy()
        {

        }

    }
}