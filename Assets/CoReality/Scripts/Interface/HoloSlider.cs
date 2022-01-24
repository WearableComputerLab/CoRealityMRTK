using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoReality.Examples
{
    public class HoloSlider : MonoBehaviour
    {
        [SerializeField]
        private HoloSliderHandle _handle;

        [SerializeField]
        private float _min = -5;

        [SerializeField]
        private float _max = 5;

        [SerializeField]
        private float _current = 0f;

        private float _minValue = 2.125f;
        private float _maxValue = -2.125f;
        private Vector3 _delta; //The delta offset
        private float _initZ;
        void Start()
        {
            _handle.OnDragStart((pos) =>
            {
                _delta = transform.InverseTransformPoint(pos);
                _initZ = _handle.transform.localPosition.z;
            }).OnDrag((pos) =>
            {
                Vector3 p = transform.InverseTransformPoint(pos) - _delta;
                float z = _initZ + p.z;
                if (z < _maxValue)
                    z = _maxValue;
                if (z > _minValue)
                    z = _minValue;
                _handle.transform.localPosition = new Vector3(0, 0, z);

                //Calculate map
                var val = MapValue(z, _minValue, _maxValue, _min, _max);
                _current = val;
                _handle.SetValue(_current);

            }).OnDragEnd((pos) =>
            {
            });
        }


        public static float MapValue(float val, float fromMin, float fromMax, float toMin, float toMax)
        {
            return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, val));
        }


    }
}
