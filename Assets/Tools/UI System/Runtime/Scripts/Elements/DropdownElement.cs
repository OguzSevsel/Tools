using PrimeTween;
using System;
using System.Collections.Generic;
using TMPro;
using Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Interfaces;
using Extensions;

namespace Elements
{
    public class DropdownElement : UIElement, IMouseInteractable
    {
        [field: SerializeField] public Image Image { get; private set; }

        [Header("Captions")]
        [SerializeField] private Image captionArrow;
        [SerializeField] private Image captionImage;
        [SerializeField] private TextMeshProUGUI captionLabel;

        [Header("Objects")]
        [SerializeField] private GameObject optionPrefab;
        [SerializeField] private GameObject dropdownPanel;
        [SerializeField] private GameObject dropdownOptionPanel;
        [SerializeField] private Scrollbar scrollBar;
        [SerializeField] private ScrollRect scrollView;

        [Header("Settings")]
        [SerializeField] private float maxPanelHeight = 200f;
        [SerializeField] private float maxPanelWidth = 200f;
        [SerializeField] private bool toggleScrollBar = true;
        [SerializeField] private bool isBlockerAboveList = true;
        
        [Header("Options")]
        [SerializeField] private List<OptionData> options;
        private GameObject blocker;

        public event Action<OptionData> OnValueChanged;
        public event Action<PointerEventData> OnMouseEnter;
        public event Action<PointerEventData> OnMouseExit;
        public event Action<PointerEventData> OnMouseClick;
        public event Action<PointerEventData> OnMouseUp;
        public event Action<PointerEventData> OnMouseDown;
        public event Action<DropdownElement> OnClose;
        public event Action<DropdownElement> OnOpen;

        public List<OptionData> Options { get; private set; }
        public OptionData SelectedOption { get; private set; }
        public PanelElement dropdownAnimationPanel { get; private set; }

        #region Option Data and Dropdown Blocker

        [System.Serializable]
        public class OptionData
        {
            public object RefObject { get; private set; }
            [field: SerializeField] public string Text { get; private set; }
            [field: SerializeField] public Sprite Image { get; private set; }
            public PanelElement PanelElement { get; private set; }

            public event Action<OptionData> OnSelected;
            public event Action<PanelElement> OnMouseExit;
            public event Action<PanelElement> OnMouseEnter;

            public OptionData(string text, Sprite image, PanelElement panelElement, object refObject = default)
            {
                this.Text = text;
                this.Image = image;
                this.RefObject = refObject;
                this.PanelElement = panelElement;

                this.PanelElement.Submitted += OnOptionSelected;
                this.PanelElement.OnMouseClick += OnOptionSelected;
                this.PanelElement.OnMouseEnter += OnOptionMouseEnter;
                this.PanelElement.OnMouseExit += OnOptionMouseExit;
            }

            private void OnOptionMouseExit(PointerEventData obj)
            {
                this.OnMouseExit?.Invoke(PanelElement);
            }

            private void OnOptionMouseEnter(PointerEventData obj)
            {
                this.OnMouseEnter?.Invoke(PanelElement);
            }

            private void OnOptionSelected(BaseEventData data)
            {
                this.OnSelected?.Invoke(this);
            }
        }

        private class DropdownBlocker : MonoBehaviour, IPointerClickHandler, ICanvasRaycastFilter
        {
            public RectTransform dropdownList;
            public event Action OnCloseDropDown;

            public void Init(RectTransform parent, RectTransform dropdown, bool isLast)
            {
                dropdownList = dropdown;

                if (isLast)
                {
                    gameObject.transform.SetAsLastSibling();
                }
                else
                {
                    gameObject.transform.SetAsFirstSibling();
                }
            }

            public void OnPointerClick(PointerEventData eventData)
            {
                Destroy(gameObject);
                OnCloseDropDown?.Invoke();
            }

            public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
            {
                if (dropdownList == null) return true;

                return !RectTransformUtility.RectangleContainsScreenPoint(dropdownList, sp, eventCamera);
            }
        }

        #endregion

        #region Initialization

        public override void Awake()
        {
            base.Awake();
            Options = new List<OptionData>();
            dropdownAnimationPanel = dropdownPanel.GetComponent<PanelElement>();

            foreach (var option in options)
            {
                AddOption(option.Text, option.Image);
            }

            options.Clear();
            SelectFirstOption();

            SetHeightOfDropdown();
            SetWidthDropdown();
        }

        public virtual void OnValidate()
        {
            ToggleScrollBar();
        }

        #endregion

        #region Animations

        public Tween TextColor(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(captionLabel,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
        }

        public Tween Color(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(Image,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
        }

        #endregion

        #region Utils

        private GameObject CreateBlocker(Transform parent)
        {
            GameObject blocker = new GameObject("Blocker", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

            RectTransform rt = blocker.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image img = blocker.GetComponent<Image>();
            img.color = new Color(0, 0, 0, 0);
            img.raycastTarget = true;

            blocker.AddComponent<DropdownBlocker>().Init((RectTransform)parent, (RectTransform)dropdownPanel.transform, isBlockerAboveList);
            blocker.GetComponent<DropdownBlocker>().OnCloseDropDown += CloseDropdown;

            return blocker;
        }

        private bool IsPartOfLayoutGroup()
        {
            var horizontal = this.gameObject.GetComponentInParent<HorizontalLayoutGroup>();
            var vertical = this.gameObject.GetComponentInParent<VerticalLayoutGroup>();
            var grid = this.gameObject.GetComponentInParent<GridLayoutGroup>();

            if (horizontal == null && vertical == null && grid == null)
            {
                return false;
            }
            return true;
        }

        private void CloseDropdown()
        {
            ToggleDropdown();
        }

        private void SelectFirstOption()
        {
            if (Options.Count > 0)
            {
                OptionData data = Options[0];

                SetCaptions(data.Text, data.Image);
                SelectedOption = data;
                this.OnValueChanged?.Invoke(SelectedOption);
            }
        }

        private void ToggleScrollBar()
        {
            if (toggleScrollBar)
            {
                scrollView.verticalScrollbar = scrollBar;
                scrollBar.gameObject.SetActive(true);
                return;
            }
            
            scrollView.verticalScrollbar = null;
            scrollBar.gameObject.SetActive(false);
        }

        public void ToggleDropdown()
        {
            SetHeightOfDropdown();

            bool isOpen = dropdownAnimationPanel.IsVisible() && dropdownAnimationPanel.IsActive();

            if (isOpen)
            {
                if (blocker != null)
                {
                    Destroy(blocker);
                }

                if (OnClose == null)
                {
                    dropdownAnimationPanel.FadeSlideOut();
                    return;
                }

                OnClose.Invoke(this);
            }
            else
            {
                blocker = CreateBlocker(gameObject.GetComponentInParent<Canvas>().transform);

                if (OnOpen == null)
                {
                    dropdownAnimationPanel.FadeSlideIn();
                    return;
                }

                OnOpen.Invoke(this);
            }
        }

        public void SetWidthDropdown()
        {
            RectTransform dropdownRect = gameObject.GetComponent<RectTransform>();

            if (!IsPartOfLayoutGroup())
            {
                dropdownRect.SetWidth(maxPanelWidth);
            }
        }

        private void SetHeightOfDropdown()
        {
            float itemHeight = 20;

            if (Options.Count > 0)
            {
                itemHeight = Options[0].PanelElement.gameObject.GetComponent<RectTransform>().rect.height;
            }

            RectTransform dropdownRect = dropdownPanel.GetComponent<RectTransform>();

            if (Options.Count * itemHeight > maxPanelHeight)
            {
                dropdownRect.SetHeight(maxPanelHeight);
            }
            else if (Options.Count * itemHeight < maxPanelHeight && Options.Count * itemHeight > 0)
            {
                dropdownRect.SetHeight(Options.Count * itemHeight);
            }
            else
            {
                dropdownRect.SetHeight(itemHeight);
            }
        }

        public void SetWidth(RectTransform rt, float width)
        {
            Vector2 size = rt.sizeDelta;
            size.x = width;
            rt.sizeDelta = size;
        }

        public void SetHeight(RectTransform rt, float height)
        {
            Vector2 size = rt.sizeDelta;
            size.y = height;
            rt.sizeDelta = size;
        }

        public void SetCaptions(string captionLabel, Sprite captionImage)
        {
            this.captionLabel.text = captionLabel;
            this.captionImage.sprite = captionImage;
        }

        #endregion

        #region Option Utils

        public void AddOption(string text, Sprite image = null, object refObject = default)
        {
            GameObject dataGO = Instantiate(optionPrefab, dropdownOptionPanel.transform);
            PanelElement panelElement = dataGO.GetComponent<PanelElement>();
            DropdownOptionView view = dataGO.GetComponent<DropdownOptionView>();

            OptionData data = new(text, image, panelElement, refObject);
            data.OnSelected += OnValueChangedHandler;
            data.OnMouseEnter += OnOptionMouseEnter;
            data.OnMouseExit += OnOptionMouseExit;
            
            view.Text.text = data.Text;
            view.Icon.sprite = data.Image;

            Options.Add(data);
        }

        public void AddOptions(List<OptionData> options)
        {
            ClearOptions();

            foreach (var option in options)
            {
                if (option.RefObject != null)
                {
                    AddOption(option.Text, option.Image, option.RefObject);
                }
                else 
                {
                    AddOption(option.Text, option.Image);
                }
            }
        }

        public void ClearOptions()
        {
            Options.Clear();
            Utilities.Helpers.DeleteChildren(dropdownOptionPanel.transform);
        }

        public void SetOption(int value)
        {
            if (Options.Count <= 0) return;
            if(value < 0 || value >= Options.Count) return;

            OptionData data = Options[value];
            this.OnValueChanged?.Invoke(data);
            this.SelectedOption = data;
            SetCaptions(data.Text, data.Image);
        }
            
        public void SetOption(OptionData data)
        {
            if (!Options.Contains(data)) return;

            this.OnValueChanged?.Invoke(data);
            this.SelectedOption = data;
            SetCaptions(data.Text, data.Image);
        }

        public void SetOption(object refObject)
        {
            OptionData data = null;

            foreach (var option in Options)
            {
                if (option.RefObject == refObject)
                {
                    data = option;
                }
            }

            if (data != null)
            {
                this.OnValueChanged?.Invoke(data);
                this.SelectedOption = data;
                SetCaptions(data.Text, data.Image);
                return;
            }

            Debug.LogWarning(refObject + "Could not be found in the dropdown options");
        }

        public OptionData GetOption(object refObject)
        {
            foreach (var option in Options)
            {
                if (option.RefObject == refObject)
                {
                    return option;
                }
            }
            return null;
        }

        public OptionData GetOption(Image image)
        {
            foreach (var option in Options)
            {
                if (option.Image == image)
                {
                    return option;
                }
            }
            return null;
        }

        public OptionData GetOption(string text)
        {
            foreach (var option in Options)
            {
                if (option.Text == text)
                {
                    return option;
                }
            }
            return null;
        }

        public OptionData GetOption(int index)
        {
            if (Options.Count <= 0) return null;
            if (index < 0 || index >= Options.Count) return null;

            return Options[index];
        }

        #endregion

        #region Events

        private void OnOptionMouseExit(PanelElement element)
        {
            element.ScaleDown();
        }

        private void OnOptionMouseEnter(PanelElement element)
        {
            element.ScaleUp();
        }

        private void OnValueChangedHandler(OptionData data)
        {
            this.SelectedOption = data;
            SetCaptions(data.Text, data.Image);
            ToggleDropdown();

            if (!EventSettings.OnValueChanged) return;

            this.OnValueChanged?.Invoke(data);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            this.OnMouseEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            this.OnMouseExit?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            ToggleDropdown();

            this.OnMouseClick?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;

            this.OnMouseUp?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;

            this.OnMouseDown?.Invoke(eventData);
        }

        #endregion
    }
}

