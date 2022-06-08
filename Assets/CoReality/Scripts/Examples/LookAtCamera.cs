using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoReality.Examples
{

    public class LookAtCamera : MonoBehaviour
    {
        void Update()
        {
            if (Camera.main != null)
                transform.LookAt(transform.position - Camera.main.transform.position);
        }
    }
}