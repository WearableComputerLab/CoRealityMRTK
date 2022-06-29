using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace OZMap
{
    /// <summary>
    /// Animates an "eye" object that is attatched to a multiuser avatar. Humans are 
    /// used to focusing on eyes while talking, but if those eyes are lifeless then uncanny
    /// valley comes knocking. Movement should be just enough to feel natural and alive,
    /// but subtle enough that it does not convey unintended emotion.
    /// </summary>
    public class EyeAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject _eyeObject;

        [Header("Blink Settings")]
        [SerializeField] private bool _useBlink;
        [SerializeField] private float _blinkDuration;
        [SerializeField] private float _minBlinkTime;
        [SerializeField] private float _maxBlinkTime;

        [Header("Movement Settings")]
        [SerializeField] private bool _useMovement;
        [SerializeField] private float _moveDuration;
        [SerializeField] private float _moveDistance;
        [SerializeField] private float _minMoveTime;
        [SerializeField] private float _maxMoveTime;

        [Header("Squint Settings")]
        [SerializeField] private bool _useSquint;
        [SerializeField] private float _minSquintRange;
        [SerializeField] private float _maxSquintRange;
        [SerializeField] private float _squintDuration;
        [SerializeField] private float _minSquintTime;
        [SerializeField] private float _maxSquintTime;

        private float _blinkTimer;
        private float _moveTimer;
        private float _squintTimer;
        private Sequence _blinkSequence;
        private Sequence _lookSequence;
        private Sequence _squintSequence;
        private Vector3 _basePosition;
        private Vector3 _currentScale;

        void Start()
        {
            _basePosition = _eyeObject.transform.localPosition;
            _currentScale = Vector3.one;
        }

        private void Update()
        {
            if (_useBlink)
            {
                _blinkTimer -= Time.deltaTime;
                if (_blinkTimer <= 0f)
                {
                    _blinkTimer = Random.Range(_minBlinkTime, _maxBlinkTime);
                    DoBlink();
                }
            }

            if (_useMovement)
            {
                _moveTimer -= Time.deltaTime;
                if (_moveTimer <= 0f)
                {
                    Vector3 toPos = Vector3.zero;
                    if (Random.value >= 0.7f)
                    {
                        toPos = Vector3.zero;
                        _moveTimer = Random.Range(Mathf.RoundToInt(_maxMoveTime / 2f), _maxMoveTime);
                    }
                    else
                    {
                        toPos = new Vector3(
                            Random.Range(-_moveDistance, _moveDistance),
                            0f,
                            Random.Range(-_moveDistance, _moveDistance)
                        );
                        _moveTimer = Random.Range(_minMoveTime, _maxMoveTime);
                    }
                    DoEyeMovement(toPos + _basePosition);
                }
            }

            if (_useSquint)
            {
                _squintTimer -= Time.deltaTime;
                if (_squintTimer <= 0f)
                {
                    _squintTimer = Random.Range(_minSquintTime, _maxSquintTime);
                    _currentScale = new Vector3(
                        1f, 1f, Random.Range(_minSquintRange, _maxSquintRange)
                    );
                    if (!_blinkSequence.IsPlaying())
                    {
                        DoSquint(_currentScale.y);
                    }
                }
            }
        }

        public void DoBlink()
        {
            if (_blinkSequence != null && _blinkSequence.IsPlaying())
            {
                _blinkSequence.Kill();
            }

            _blinkSequence = DOTween.Sequence();
            _blinkSequence.Append(_eyeObject.transform.DOScale(new Vector3(1f, 1f, 0f), _blinkDuration / 2f));
            _blinkSequence.AppendInterval(0.1f);
            _blinkSequence.Append(_eyeObject.transform.DOScale(_currentScale, _blinkDuration / 2f));
            _blinkSequence.Play();
        }

        public void DoSquint(float scale)
        {
            if (_squintSequence != null && _squintSequence.IsPlaying())
            {
                _squintSequence.Kill();
            }

            _squintSequence = DOTween.Sequence();
            _squintSequence.Append(_eyeObject.transform.DOScale(new Vector3(1f, 1f, scale), _squintDuration));
            _squintSequence.Play();
        }


        public void DoEyeMovement(Vector3 toPos)
        {
            if (_lookSequence != null && _lookSequence.IsPlaying())
            {
                _lookSequence.Kill();
            }

            _lookSequence = DOTween.Sequence();
            _lookSequence.Append(_eyeObject.transform.DOLocalMove(toPos, _moveDuration));
            _lookSequence.Play();
        }

    }

}
