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
                this.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
        }
    }
}