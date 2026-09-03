using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Elements;

namespace Utilities
{
    public class MenuNavigation : MonoBehaviour
    {
        [SerializeField] private List<UIElement> elements;
        [SerializeField] private InputActionReference navigateReference;
        private UIElement lastSelected;

        public UIElement FirstSelected;

        private void Awake()
        {
            navigateReference.action.Enable();
            navigateReference.action.performed += OnNavigate;

            foreach (var element in elements)
            {
                if (element.gameObject.activeInHierarchy)
                {
                    Selectable selectable = element.GetComponent<Selectable>();

                    if (selectable != null)
                    {
                        if (element is PanelElement panelElement)
                        {
                            panelElement.Selected += Element_OnSelected;
                            panelElement.Deselected += Element_OnDeselected;
                            panelElement.OnMouseEnter += Element_OnMouseEnter;
                            panelElement.OnMouseExit += Element_OnMouseExit;
                            panelElement.Submitted += Element_OnSubmit;
                            continue;
                        }

                        if (element is TextElement textElement)
                        {
                            textElement.Selected += Element_OnSelected;
                            textElement.Deselected += Element_OnDeselected;
                            textElement.Submitted += Element_OnSubmit;
                            continue;
                        }

                        if (element is ButtonElement buttonElement)
                        {
                            buttonElement.Selected += Element_OnSelected;
                            buttonElement.Deselected += Element_OnDeselected;
                            buttonElement.OnMouseEnter += Element_OnMouseEnter;
                            buttonElement.OnMouseExit += Element_OnMouseExit;
                            buttonElement.Submitted += Element_OnSubmit;
                            continue;
                        }

                        if (element is DropdownElement dropdownElement)
                        {
                            dropdownElement.Selected += Element_OnSelected;
                            dropdownElement.Deselected += Element_OnDeselected;
                            dropdownElement.OnMouseEnter += Element_OnMouseEnter;
                            dropdownElement.OnMouseExit += Element_OnMouseExit;
                            dropdownElement.Submitted += Element_OnSubmit;
                            continue;
                        }

                        if (element is ToggleElement toggleElement)
                        {
                            toggleElement.Selected += Element_OnSelected;
                            toggleElement.Deselected += Element_OnDeselected;
                            toggleElement.Submitted += Element_OnSubmit;
                            continue;
                        }

                        if (element is InputFieldElement inputFieldElement)
                        {
                            inputFieldElement.Selected += Element_OnSelected;
                            inputFieldElement.Deselected += Element_OnDeselected;
                            inputFieldElement.Submitted += Element_OnSubmit;
                            continue;
                        }

                        if (element is SliderElement sliderElement)
                        {
                            sliderElement.Selected += Element_OnSelected;
                            sliderElement.Deselected += Element_OnDeselected;
                            sliderElement.Submitted += Element_OnSubmit;
                            continue;
                        }
                    }
                }
            }
        }

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(FirstSelected.gameObject);
        }

        private void Element_OnSubmit(BaseEventData data)
        {
            UIElement element = data.selectedObject.GetComponent<UIElement>();
            element.ScaleDown(duration: 0.1f);
        }

        private void Element_OnMouseExit(PointerEventData data)
        {
            data.selectedObject = null;
        }

        private void Element_OnMouseEnter(PointerEventData data)
        {
            data.selectedObject = data.pointerEnter;
        }

        private void Element_OnDeselected(BaseEventData data)
        {
            UIElement element = data.selectedObject.GetComponent<UIElement>();
            element.ScaleDown();
        }

        private void Element_OnSelected(BaseEventData obj)
        {
            lastSelected = obj.selectedObject.GetComponent<UIElement>();
            UIElement element = obj.selectedObject.GetComponent<UIElement>();

            Debug.Log($"Last selected: {lastSelected?.name}");

            element.ScaleUp();
        }

        private void OnNavigate(InputAction.CallbackContext context)
        {
            if (EventSystem.current.currentSelectedGameObject == null && lastSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelected.gameObject);
                return;
            }
        }

        private void OnDestroy()
        {
            foreach (var element in elements)
            {
                if (element.gameObject.activeInHierarchy)
                {
                    Selectable selectable = element.GetComponent<Selectable>();

                    if (selectable != null)
                    {
                        if (element is PanelElement panelElement)
                        {
                            panelElement.Selected -= Element_OnSelected;
                            panelElement.Deselected -= Element_OnDeselected;
                            panelElement.OnMouseEnter -= Element_OnMouseEnter;
                            panelElement.OnMouseExit -= Element_OnMouseExit;
                            panelElement.Submitted -= Element_OnSubmit;
                            return;
                        }

                        if (element is TextElement textElement)
                        {
                            textElement.Selected -= Element_OnSelected;
                            textElement.Deselected -= Element_OnDeselected;
                            textElement.Submitted -= Element_OnSubmit;
                            return;
                        }

                        if (element is ButtonElement buttonElement)
                        {
                            buttonElement.Selected -= Element_OnSelected;
                            buttonElement.Deselected -= Element_OnDeselected;
                            buttonElement.OnMouseEnter -= Element_OnMouseEnter;
                            buttonElement.OnMouseExit -= Element_OnMouseExit;
                            buttonElement.Submitted -= Element_OnSubmit;
                            return;
                        }

                        if (element is DropdownElement dropdownElement)
                        {
                            dropdownElement.Selected -= Element_OnSelected;
                            dropdownElement.Deselected -= Element_OnDeselected;
                            dropdownElement.OnMouseEnter -= Element_OnMouseEnter;
                            dropdownElement.OnMouseExit -= Element_OnMouseExit;
                            dropdownElement.Submitted -= Element_OnSubmit;
                            return;
                        }

                        if (element is ToggleElement toggleElement)
                        {
                            toggleElement.Selected -= Element_OnSelected;
                            toggleElement.Deselected -= Element_OnDeselected;
                            toggleElement.Submitted -= Element_OnSubmit;
                            return;
                        }

                        if (element is InputFieldElement inputFieldElement)
                        {
                            inputFieldElement.Selected -= Element_OnSelected;
                            inputFieldElement.Deselected -= Element_OnDeselected;
                            inputFieldElement.Submitted -= Element_OnSubmit;
                            return;
                        }

                        if (element is SliderElement sliderElement)
                        {
                            sliderElement.Selected -= Element_OnSelected;
                            sliderElement.Deselected -= Element_OnDeselected;
                            sliderElement.Submitted -= Element_OnSubmit;
                            return;
                        }
                    }
                }
            }
        }
    } 
}
