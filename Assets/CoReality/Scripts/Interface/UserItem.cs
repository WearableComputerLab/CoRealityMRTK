using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CoReality
{

    public class UserItem : MonoBehaviour
    {

        private RectTransform _rect;
        public RectTransform RectTransform
        {
            get
            {
                if (_rect == null)
                    _rect = GetComponent<RectTransform>();
                return _rect;
            }
        }

        public string Name
        {
            get => _nameText.text;
            set => _nameText.text = value;
        }

        [SerializeField]
        private TextMeshProUGUI _nameText;

        public Color Color
        {
            get => _colorImage.color;
            set => _colorImage.color = value;
        }

        [SerializeField]
        private Image _colorImage;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}