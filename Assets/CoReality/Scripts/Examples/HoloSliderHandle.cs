using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
using System;

namespace CoReality.Examples
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class HoloSliderHandle : MonoBehaviour, IMixedRealityPointerHandler
    {

        [SerializeField]
        private TextMeshPro _valueText;
        private float _delta;

        private Action<Vector3> _dragStart, _drag, _dragEnd;

        public void SetValue(float value)
        {
            _valueText.text = String.Format("{0:0.0}", value);
        }

        public HoloSliderHandle OnDragStart(Action<Vector3> cb)
        {
            _dragStart = cb;
            return this;
        }

        public HoloSliderHandle OnDrag(Action<Vector3> cb)
        {
            _drag = cb;
            return this;
        }

        public HoloSliderHandle OnDragEnd(Action<Vector3> cb)
        {
            _dragEnd = cb;
            return this;
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer is SpherePointer p)
            {
                _dragStart?.Invoke(p.Position);
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer is SpherePointer p)
            {
                _drag?.Invoke(p.Position);
            }

        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer is SpherePointer p)
            {
                _dragEnd?.Invoke(p.Position);
            }
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        { }
    }
}