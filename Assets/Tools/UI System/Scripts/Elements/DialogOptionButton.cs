using Tools.UISystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogOptionButton : ButtonElement
{
    [SerializeField] private GameObject _indicatorObject;

    public override void Awake()
    {
        base.Awake();
        this.OnMouseEnter += DialogOptionButton_OnMouseEnter;
        this.OnMouseExit += DialogOptionButton_OnMouseExit;

        if (_indicatorObject != null)
            _indicatorObject.SetActive(false);
    }

    public void DialogOptionButton_OnMouseEnter(PointerEventData eventData)
    {
        if (_indicatorObject != null)
            _indicatorObject.SetActive(true);
    }

    public void DialogOptionButton_OnMouseExit(PointerEventData eventData)
    {
        if (_indicatorObject != null)
            _indicatorObject.SetActive(false);
    }
}
