using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoReality.Examples
{

    public class LookAtCamera : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(transform.position - Camera.main.transform.position);
        }
    }
}