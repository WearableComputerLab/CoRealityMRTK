using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IVerticalLayout : MonoBehaviour
{
    private bool _isDirty = false;

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

    private List<RectTransform> _children = new List<RectTransform>();
    public List<RectTransform> Children
    {
        get => _children;
    }

    [SerializeField]
    private float _spacing = 0.5f;

    // Update is called once per frame
    void LateUpdate()
    {
        if (_isDirty == true)
        {
            Rebuild();
            _isDirty = false;
        }
    }

    public void AddItem(RectTransform item)
    {
        //Ensure it has a gameobject
        if (item.gameObject == null) return;
        //Parent it
        item.SetParent(this.transform, false);
        Children.Add(item);
        //Rebuild layout
        SetDirty();
    }

    public void RemoveItem(RectTransform item)
    {
        RectTransform childToRemove = null;
        for (int i = 0; i < Children.Count; i++)
        {
            if (Children[i] == item)
            {
                childToRemove = Children[i];
            }
        }
        if (childToRemove)
        {
            Children.Remove(childToRemove);
            Destroy(childToRemove.gameObject);
            SetDirty();
        }
    }

    public void SetDirty()
    {
        _isDirty = true;
    }

    private void Rebuild()
    {
        float vPos = 0f;
        foreach (RectTransform c in Children)
        {
            c.DOAnchorPos(new Vector3(c.anchoredPosition.x, -vPos), 0.3f);
            // c.anchoredPosition = new Vector3(c.anchoredPosition.x, -vPos);
            vPos += c.sizeDelta.y + _spacing;
        }
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, vPos);
    }
}
