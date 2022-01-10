using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AvatarHead : MonoBehaviour
{

    private string _name;
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

}
