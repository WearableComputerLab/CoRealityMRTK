using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoReality.Examples
{

    /// <summary>
    /// Choose Hololens / Desktop scene based on build target
    /// </summary>
    public class SceneChooser : MonoBehaviour
    {
        void Awake()
        {
#if !WINDOWS_UWP
            print("Loading Desktop Scene");
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
#else
            print("Loading Hololens Scene");
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
#endif
        }
    }
}