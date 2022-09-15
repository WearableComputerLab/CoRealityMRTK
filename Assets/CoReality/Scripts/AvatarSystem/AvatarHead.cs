using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CoReality.Avatars
{

    /// <summary>
    /// TODO: Gaze implementation and eye direction
    /// </summary>
    public class AvatarHead : MonoBehaviour
    {

        [SerializeField, Tooltip("The MeshRenderer attached to this AvatarHead")]
        private MeshRenderer _meshRenderer;

        private string _name;

        /// <summary>
        /// Sets the name label for this Avatar
        /// </summary>
        /// <value></value>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                _nameText.text = value;
            }
        }

        [SerializeField]
        private TextMeshPro _nameText;

        public void SetColor(Color color)
        {
            _meshRenderer.material.SetColor("_RimColor", color);
        }
    }

}