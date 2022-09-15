using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CoReality.Examples
{
    public class HideMenu : MonoBehaviour
    {

        [SerializeField]
        private KeyCode _hideKey = KeyCode.Tilde;

        [SerializeField]
        private GameObject[] _objectsToHide;

        private bool _areActive = true;

        void Update()
        {
            if (Input.GetKeyDown(_hideKey))
            {
                _areActive = !_areActive;
                for (int i = 0; i < _objectsToHide.Length; i++)
                {
                    _objectsToHide[i].SetActive(_areActive);
                }
            }
        }
    }
}