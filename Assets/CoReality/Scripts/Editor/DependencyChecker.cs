#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.IO;

namespace CoReality
{
    public class DependencyChecker : EditorWindow
    {
        static bool _hasMRTK;
        static bool _hasVuforia;
        static bool _hasPhoton;

        [MenuItem("CoReality/DependencyChecker")]
        public static void InitPopup()
        {
            DependencyChecker window = (DependencyChecker)EditorWindow.GetWindow(typeof(DependencyChecker));
            window.position = new Rect((Screen.width / 2) - 150, (Screen.height / 2) - 110, 300, 220);
            window.ShowPopup();

            Refresh();
        }

        static void Refresh()
        {
            //Do Check
            string mrtkVersion = Application.dataPath + "/MRTK/SDK/Version.txt";
            string vuforiaPath = Application.dataPath + "../Packages/Vuforia";
            string photonPath = Application.dataPath + "/Photon";
            _hasMRTK = File.Exists(mrtkVersion);
            _hasPhoton = Directory.Exists(photonPath);
            _hasVuforia = Directory.Exists(vuforiaPath);
        }

        void OnGUI()
        {
            EditorGUILayout.Separator();

            GUIStyle headerStyle = EditorStyles.label;
            headerStyle.fontSize = 18;
            headerStyle.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.LabelField("CoReality Dependencies", headerStyle);

            GUIStyle labelStyle = EditorStyles.label;
            labelStyle.fontSize = 14;
            headerStyle.alignment = TextAnchor.MiddleLeft;

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Mixed Reality Toolkit >=2.7.2", labelStyle);
            if (!_hasMRTK)
            {
                EditorGUILayoutExtensions.LinkLabel("Download there", Color.blue, Vector2.zero, 14, "https://github.com/microsoft/MixedRealityToolkit-Unity/releases");
            }
            else
            {
                EditorGUILayout.LabelField("Installation found ✓", labelStyle);
            }

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Vuforia >=10.1.4", labelStyle);
            if (!_hasVuforia)
            {
                EditorGUILayoutExtensions.LinkLabel("Download here", Color.blue, Vector2.zero, 14, "https://developer.vuforia.com/downloads/sdk");
            }
            else
            {
                EditorGUILayout.LabelField("Installation found ✓", labelStyle);
            }

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Photon PUN 2 Unity", labelStyle);
            if (!_hasPhoton)
            {
                EditorGUILayoutExtensions.LinkLabel("Download here", Color.blue, Vector2.zero, 14, "https://assetstore.unity.com/packages/tools/network/pun-2-free-119922");
            }
            else
            {
                EditorGUILayout.LabelField("Installation found ✓", labelStyle);
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Refresh")) Refresh();
            if (GUILayout.Button("Close")) this.Close();
        }
    }

}

#endif