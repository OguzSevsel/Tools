using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Elements;
using Extensions;

public class AnimationController : MonoBehaviour
{
    public enum Element
    {
        Button,
        Camera,
        World,
        Toggle,
        Text,
        Dropdown,
        Slider,
        InputField,
        Panel,
        ProgressBar
    }

    public enum Animation
    {
        Rotate,
        RollIn,
        RollFadeIn,
        RollOut,
        RollFadeOut,
        GrowHorizontal,
        ShrinkHorizontal,
        GrowVertical,
        ShrinkVertical,
        RotateBack,
        SlideIn,
        SlideOut,
        FadeSlideIn,
        FadeSlideOut,
        FadeIn,
        FadeOut,
        RotateSlideIn,
        RotateSlideOut,
        Grow,
        Shrink,
        GrowSlideIn,
        ShrinkSlideOut,
        ScaleUp,
        ScaleDown,
        BreatheOnce,
        BreatheLoop,
        ShakeCameraPos,
        ShakeCameraRot,
        ChangeCameraAspect,
        PunchScale,
        TypeText,
        CustomEffect1,
    }

    public bool IsRollIn = true;

    public Element element;
    public Animation anim;
    public event Action<Animation, Element> OnAnimationChanged;

    [SerializeField] private ButtonElement tabButtonButton;
    [SerializeField] private ButtonElement tabCameraButton;
    [SerializeField] private ButtonElement tabWorldButton;
    [SerializeField] private ButtonElement tabToggleButton;
    [SerializeField] private ButtonElement tabTextButton;
    [SerializeField] private ButtonElement tabDropdownButton;
    [SerializeField] private ButtonElement tabSliderButton;
    [SerializeField] private ButtonElement tabInputFieldButton;
    [SerializeField] private ButtonElement tabPanelButton;
    [SerializeField] private ButtonElement tabProgressBarButton;

    [SerializeField] private PanelElement panelButton;
    [SerializeField] private PanelElement panelCamera;
    [SerializeField] private PanelElement panelWorld;
    [SerializeField] private PanelElement panelToggle;
    [SerializeField] private PanelElement panelText;
    [SerializeField] private PanelElement panelDropdown;
    [SerializeField] private PanelElement panelSlider;
    [SerializeField] private PanelElement panelInputField;
    [SerializeField] private PanelElement panelPanel;
    [SerializeField] private PanelElement panelProgressBar;

    [SerializeField] private PanelElement panel;
    [SerializeField] private CameraElement mainCamera;
    [SerializeField] private SliderElement slider;
    [SerializeField] private ToggleElement toggle;
    [SerializeField] private DropdownElement dropdown;
    [SerializeField] private TextElement text;
    [SerializeField] private SpriteElement sprite;
    [SerializeField] private InputFieldElement inputField;
    [SerializeField] private ButtonElement button;
    [SerializeField] private ProgressBarElement progressBar;

    [SerializeField] private GameObject cameraDummy;
    [SerializeField] private TextElement debugText;

    #region Initialization

    public void Start()
    {
        tabButtonButton.OnMouseClick += TabButtonHandler;
        tabCameraButton.OnMouseClick += TabCameraHandler;
        tabWorldButton.OnMouseClick += TabWorldHandler;
        tabToggleButton.OnMouseClick += TabToggleHandler;
        tabTextButton.OnMouseClick += TabTextHandler;
        tabDropdownButton.OnMouseClick += TabDropdownHandler;
        tabSliderButton.OnMouseClick += TabSliderHandler;
        tabInputFieldButton.OnMouseClick += TabInputHandler;
        tabPanelButton.OnMouseClick += TabPanelHandler;
        tabProgressBarButton.OnMouseClick += TabProgressBarHandler;
        OnAnimationChanged += SwitchAnimation;

        panel.OnMouseEnter += Panel_OnMouseEnter;
        button.OnMouseEnter += Button_OnMouseEnter;
        dropdown.OnMouseEnter += Dropdown_OnMouseEnter;

        panel.OnMouseExit += Panel_OnMouseExit;
        button.OnMouseExit += Button_OnMouseExit;
        dropdown.OnMouseExit += Dropdown_OnMouseExit;
    }

    private void Dropdown_OnMouseExit(PointerEventData data)
    {
        dropdown.ScaleDown();
    }

    private void Button_OnMouseExit(PointerEventData data)
    {
        button.ScaleDown();
    }

    private void Panel_OnMouseExit(PointerEventData data)
    {
        panel.ScaleDown();
    }

    private void Dropdown_OnMouseEnter(PointerEventData data)
    {
        dropdown.ScaleUp();
    }

    private void Button_OnMouseEnter(PointerEventData data)
    {
        button.JiggleGrow();
    }

    private void Panel_OnMouseEnter(PointerEventData data)
    {
        panel.ScaleUp();
    }

    private void TogglePanel(PanelElement element)
    {
        if (element.gameObject.activeInHierarchy)
        {
            element.SetActive(false);
            return;
        }
        element.SetActive(true);
    }

    private void SetDebugText(string text)
    {
        debugText.TypeText(text, text.Length);
        debugText.FlashTextColor(Color.red, cycles: 3);
    }

    private void DeactivateElements()
    {
        button.SetActive(false);
        sprite.SetActive(false);
        toggle.SetActive(false);
        text.SetActive(false);
        dropdown.SetActive(false);
        slider.SetActive(false);
        inputField.SetActive(false);
        panel.SetActive(false);
        cameraDummy.SetActive(false);
    }

    private void DeactivatePanels()
    {
        panelButton.SetActive(false);
        panelCamera.SetActive(false);
        panelWorld.SetActive(false);
        panelToggle.SetActive(false);
        panelText.SetActive(false);
        panelDropdown.SetActive(false);
        panelSlider.SetActive(false);
        panelInputField.SetActive(false);
        panelPanel.SetActive(false);
    }

    private void TabButtonHandler(PointerEventData data)
    {
        DeactivatePanels();
        TogglePanel(panelButton);
        element = Element.Button;
        DeactivateElements();
        button.SetActive(true);
    }

    private void TabCameraHandler(PointerEventData data)
    {
        DeactivatePanels();
        TogglePanel(panelCamera);
        element = Element.Camera;
        DeactivateElements();
        cameraDummy.SetActive(true);
    }

    private void TabWorldHandler(PointerEventData data)
    {
        DeactivatePanels();
        TogglePanel(panelWorld);
        element = Element.World;
        DeactivateElements();
        sprite.SetActive(true);
    }

    private void TabToggleHandler(PointerEventData data)
    {
        DeactivatePanels();
        TogglePanel(panelToggle);
        element = Element.Toggle;
        DeactivateElements();
        toggle.SetActive(true);
        panelToggle.SetActive(true);
    }

    private void TabTextHandler(PointerEventData data)
    {
        DeactivatePanels();
        TogglePanel(panelText);
        element = Element.Text;
        DeactivateElements();
        text.SetActive(true);
    }

    private void TabDropdownHandler(PointerEventData data)
    {
        DeactivatePanels();
        TogglePanel(panelDropdown);
        element = Element.Dropdown;
        DeactivateElements();
        dropdown.SetActive(true);
    }

    private void TabSliderHandler(PointerEventData data)
    {
        DeactivatePanels();
        TogglePanel(panelSlider);
        element = Element.Slider;
        DeactivateElements();
        slider.SetActive(true);
    }

    private void TabInputHandler(PointerEventData data)
    {
        DeactivatePanels();
        TogglePanel(panelInputField);
        element = Element.InputField;
        DeactivateElements();
        inputField.SetActive(true);
    }

    private void TabPanelHandler(PointerEventData data)
    {
        DeactivatePanels();
        TogglePanel(panelPanel);
        element = Element.Panel;
        DeactivateElements();
        panel.SetActive(true);
    }

    private void TabProgressBarHandler(PointerEventData data)
    {
        DeactivatePanels();
        TogglePanel(panelProgressBar);
        element = Element.ProgressBar;
        DeactivateElements();
        progressBar.SetActive(true);
    }

    #endregion

    #region Animations

    public void ScaleUp()
    {
        anim = Animation.ScaleUp;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void ScaleDown()
    {
        anim = Animation.ScaleDown;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void Grow()
    {
        anim = Animation.Grow;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void Shrink()
    {
        anim = Animation.Shrink;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void SlideIn()
    {
        anim = Animation.SlideIn;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void SlideOut()
    {
        anim = Animation.SlideOut;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void FadeSlideIn()
    {
        anim = Animation.FadeSlideIn;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void FadeSlideOut()
    {
        anim = Animation.FadeSlideOut;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void GrowSlideIn()
    {
        anim = Animation.GrowSlideIn;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void ShrinkSlideOut()
    {
        anim = Animation.ShrinkSlideOut;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void RotateSlideIn()
    {
        anim = Animation.RotateSlideIn;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void RotateSlideOut()
    {
        anim = Animation.RotateSlideOut;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void BreatheOnce()
    {
        anim = Animation.BreatheOnce;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void BreatheLoop()
    {
        anim = Animation.BreatheLoop;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void FadeIn()
    {
        anim = Animation.FadeIn;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void FadeOut()
    {
        anim = Animation.FadeOut;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void ShakeCameraPos()
    {
        anim = Animation.ShakeCameraPos;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void ShakeCameraRot()
    {
        anim = Animation.ShakeCameraRot;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void ChangeCameraAspect()
    {
        anim = Animation.ChangeCameraAspect;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void Rotate()
    {
        anim = Animation.Rotate;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void RotateReverse()
    {
        anim = Animation.RotateBack;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void TypeText()
    {
        anim = Animation.TypeText;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void GrowHorizontal()
    {
        anim = Animation.GrowHorizontal;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void ShrinkHorizontal()
    {
        anim = Animation.ShrinkHorizontal;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void GrowVertical()
    {
        anim = Animation.GrowVertical;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void ShrinkVertical()
    {
        anim = Animation.ShrinkVertical;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void RollIn()
    {
        anim = Animation.RollIn;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void RollOut()
    {
        anim = Animation.RollOut;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void RollFadeIn()
    {
        anim = Animation.RollFadeIn;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void RollFadeOut()
    {
        anim = Animation.RollFadeOut;
        OnAnimationChanged?.Invoke(anim, element);
    }

    public void CustomAnimation1()
    {
        anim = Animation.CustomEffect1;
        OnAnimationChanged?.Invoke(anim, element);
    }

    #endregion

    #region Utils

    private void SwitchAnimation(Animation animation, Element element)
    {
        switch (element)
        {
            case Element.Button:

                switch (animation)
                {
                    case Animation.Rotate:
                        button.Rotation();
                        break;
                    case Animation.RotateBack:
                        button.Rotation(new Vector3(0,0,0));
                        break;
                    case Animation.SlideIn:
                        button.SlideIn();
                        break;
                    case Animation.SlideOut:
                        button.SlideOut();
                        break;
                    case Animation.FadeSlideIn:
                        button.FadeSlideIn();
                        break;
                    case Animation.FadeSlideOut:
                        button.FadeSlideOut();
                        break;
                    case Animation.FadeIn:
                        button.FadeIn();
                        break;
                    case Animation.FadeOut:
                        button.FadeOut();
                        break;
                    case Animation.RotateSlideIn:
                        button.RotateSlideIn(new Vector3(0, 0, 0));
                        break;
                    case Animation.RotateSlideOut:
                        button.RotateSlideOut();
                        break;
                    case Animation.Grow:
                        button.Grow();
                        break;
                    case Animation.Shrink:
                        button.Shrink();
                        break;
                    case Animation.GrowSlideIn:
                        button.GrowSlideIn();
                        break;
                    case Animation.ShrinkSlideOut:
                        button.ShrinkSlideOut();
                        break;
                    case Animation.ScaleUp:
                        button.ScaleUp();
                        break;
                    case Animation.ScaleDown:
                        button.ScaleDown();
                        break;
                    case Animation.BreatheOnce:
                        SetDebugText("You cant use this Animation on Button Element");
                        break;
                    case Animation.BreatheLoop:
                        SetDebugText("You cant use this Animation on Button Element");
                        break;
                    case Animation.ShakeCameraPos:
                        SetDebugText("You cant use this Animation on Button Element");
                        break;
                    case Animation.ShakeCameraRot:
                        SetDebugText("You cant use this Animation on Button Element");
                        break;
                    case Animation.ChangeCameraAspect:
                        SetDebugText("You cant use this Animation on Button Element");
                        break;
                    case Animation.PunchScale:
                        button.PunchScale();
                        break;
                    case Animation.TypeText:
                        button.TypeText();
                        break;
                    case Animation.GrowHorizontal:
                        button.GrowHorizontal();
                        break;
                    case Animation.ShrinkHorizontal:
                        button.ShrinkHorizontal();
                        break;
                    case Animation.GrowVertical:
                        button.GrowVertical();
                        break;
                    case Animation.ShrinkVertical:
                        button.ShrinkVertical();
                        break;
                    case Animation.RollIn:
                        button.Show();
                        button.RollIn();
                        break;
                    case Animation.RollOut:
                        button.RollOut();
                        break;
                    case Animation.RollFadeIn:
                        button.RollFadeIn();
                        break;
                    case Animation.RollFadeOut:
                        button.RollFadeOut();
                        break;
                    default:
                        break;
                }

                break;

            case Element.Camera:

                switch (animation)
                {
                    case Animation.Rotate:
                        mainCamera.Rotation();
                        break;
                    case Animation.RotateBack:
                        mainCamera.Rotation(new Vector3(0, 0, 0));
                        break;
                    case Animation.SlideIn:
                        mainCamera.SlideIn();
                        break;
                    case Animation.SlideOut:
                        mainCamera.SlideOut();
                        break;
                    case Animation.FadeSlideIn:
                        SetDebugText("You cant use this Animation on Camera Element");
                        break;
                    case Animation.FadeSlideOut:
                        SetDebugText("You cant use this Animation on Camera Element");
                        break;
                    case Animation.FadeIn:
                        SetDebugText("You cant use this Animation on Camera Element");
                        break;
                    case Animation.FadeOut:
                        SetDebugText("You cant use this Animation on Camera Element");
                        break;
                    case Animation.RotateSlideIn:
                        mainCamera.RotateSlideIn(new Vector3(0, 0, 0));
                        break;
                    case Animation.RotateSlideOut:
                        mainCamera.RotateSlideOut();
                        break;
                    case Animation.Grow:
                        mainCamera.Grow();
                        break;
                    case Animation.Shrink:
                        mainCamera.Shrink();
                        break;
                    case Animation.GrowSlideIn:
                        mainCamera.GrowSlideIn();
                        break;
                    case Animation.ShrinkSlideOut:
                        mainCamera.ShrinkSlideOut();
                        break;
                    case Animation.ScaleUp:
                        mainCamera.ScaleUp();
                        break;
                    case Animation.ScaleDown:
                        mainCamera.ScaleDown();
                        break;
                    case Animation.BreatheOnce:
                        SetDebugText("You cant use this Animation on Camera Element");
                        break;
                    case Animation.BreatheLoop:
                        SetDebugText("You cant use this Animation on Camera Element");
                        break;
                    case Animation.ShakeCameraPos:
                        mainCamera.ShakePosition();
                        break;
                    case Animation.ShakeCameraRot:
                        mainCamera.ShakeRotation();
                        break;
                    case Animation.ChangeCameraAspect:
                        mainCamera.ChangeAspect(aspect: 2f);
                        break;
                    case Animation.PunchScale:
                        mainCamera.PunchScale();
                        break;
                    case Animation.TypeText:
                        SetDebugText("You cant use this Animation on Camera Element");
                        break;
                    default:
                        break;
                }

                break;

            case Element.World:

                switch (animation)
                {
                    case Animation.Rotate:
                        sprite.Rotation();
                        break;
                    case Animation.RotateBack:
                        sprite.Rotation(new Vector3(0, 0, 0));
                        break;
                    case Animation.SlideIn:
                        sprite.SlideIn();
                        break;
                    case Animation.SlideOut:
                        sprite.SlideOut();
                        break;
                    case Animation.FadeSlideIn:
                        sprite.FadeSlideIn();
                        break;
                    case Animation.FadeSlideOut:
                        sprite.FadeSlideOut();
                        break;
                    case Animation.FadeIn:
                        sprite.FadeIn();
                        break;
                    case Animation.FadeOut:
                        sprite.FadeOut();
                        break;
                    case Animation.RotateSlideIn:
                        sprite.RotateSlideIn(new Vector3(0, 0, 0));
                        break;
                    case Animation.RotateSlideOut:
                        sprite.RotateSlideOut();
                        break;
                    case Animation.Grow:
                        sprite.Grow();
                        break;
                    case Animation.Shrink:
                        sprite.Shrink();
                        break;
                    case Animation.GrowSlideIn:
                        sprite.GrowSlideIn();
                        break;
                    case Animation.ShrinkSlideOut:
                        sprite.ShrinkSlideOut();
                        break;
                    case Animation.ScaleUp:
                        sprite.ScaleUp();
                        break;
                    case Animation.ScaleDown:
                        sprite.ScaleDown();
                        break;
                    case Animation.BreatheOnce:
                        sprite.Breathe().SetLoops(1);
                        break;
                    case Animation.BreatheLoop:
                        sprite.Breathe().SetLoops(5).OnComplete(sprite, (world) => SetDebugText("Limited with 5 loops"));
                        break;
                    case Animation.ShakeCameraPos:
                        SetDebugText("You cant use this Animation on World Element");
                        break;
                    case Animation.ShakeCameraRot:
                        SetDebugText("You cant use this Animation on World Element");
                        break;
                    case Animation.ChangeCameraAspect:
                        SetDebugText("You cant use this Animation on World Element");
                        break;
                    case Animation.PunchScale:
                        sprite.PunchScale();
                        break;
                    case Animation.TypeText:
                        SetDebugText("You cant use this Animation on World Element");
                        break;
                    default:
                        break;
                }

                break;

            case Element.Toggle:

                switch (animation)
                {
                    case Animation.Rotate:
                        toggle.Rotation();
                        break;
                    case Animation.RotateBack:
                        toggle.Rotation(new Vector3(0, 0, 0));
                        break;
                    case Animation.SlideIn:
                        toggle.SlideIn();
                        break;
                    case Animation.SlideOut:
                        toggle.SlideOut();
                        break;
                    case Animation.FadeSlideIn:
                        toggle.FadeSlideIn();
                        break;
                    case Animation.FadeSlideOut:
                        toggle.FadeSlideOut();
                        break;
                    case Animation.FadeIn:
                        toggle.FadeIn();
                        break;
                    case Animation.FadeOut:
                        toggle.FadeOut();
                        break;
                    case Animation.RotateSlideIn:
                        toggle.RotateSlideIn(new Vector3(0, 0, 0));
                        break;
                    case Animation.RotateSlideOut:
                        toggle.RotateSlideOut();
                        break;
                    case Animation.Grow:
                        toggle.Grow();
                        break;
                    case Animation.Shrink:
                        toggle.Shrink();
                        break;
                    case Animation.GrowSlideIn:
                        toggle.GrowSlideIn();
                        break;
                    case Animation.ShrinkSlideOut:
                        toggle.ShrinkSlideOut();
                        break;
                    case Animation.ScaleUp:
                        toggle.ScaleUp();
                        break;
                    case Animation.ScaleDown:
                        toggle.ScaleDown();
                        break;
                    case Animation.BreatheOnce:
                        toggle.Breathe().SetLoops(1);
                        break;
                    case Animation.BreatheLoop:
                        toggle.Breathe().SetLoops(5).OnComplete(toggle, (toggle) => SetDebugText("Limited with 5 loops"));
                        break;
                    case Animation.ShakeCameraPos:
                        SetDebugText("You cant use this Animation on Toggle Element");
                        break;
                    case Animation.ShakeCameraRot:
                        SetDebugText("You cant use this Animation on Toggle Element");
                        break;
                    case Animation.ChangeCameraAspect:
                        SetDebugText("You cant use this Animation on Toggle Element");
                        break;
                    case Animation.PunchScale:
                        toggle.PunchScale();
                        break;
                    case Animation.TypeText:
                        toggle.TypeText();
                        break;
                    case Animation.GrowHorizontal:
                        toggle.GrowHorizontal();
                        break;
                    case Animation.ShrinkHorizontal:
                        toggle.ShrinkHorizontal();
                        break;
                    case Animation.GrowVertical:
                        toggle.GrowVertical();
                        break;
                    case Animation.ShrinkVertical:
                        toggle.ShrinkVertical();
                        break;
                    case Animation.RollIn:
                        toggle.Show();
                        toggle.RollIn();
                        break;
                    case Animation.RollOut:
                        toggle.RollOut();
                        break;
                    case Animation.RollFadeIn:
                        toggle.RollFadeIn();
                        break;
                    case Animation.RollFadeOut:
                        toggle.RollFadeOut();
                        break;
                    default:
                        break;
                }

                break;

            case Element.Text:

                switch (animation)
                {
                    case Animation.Rotate:
                        text.Rotation();
                        break;
                    case Animation.RotateBack:
                        text.Rotation(new Vector3(0, 0, 0));
                        break;
                    case Animation.SlideIn:
                        text.SlideIn();
                        break;
                    case Animation.SlideOut:
                        text.SlideOut();
                        break;
                    case Animation.FadeSlideIn:
                        text.FadeSlideIn();
                        break;
                    case Animation.FadeSlideOut:
                        text.FadeSlideOut();
                        break;
                    case Animation.FadeIn:
                        text.FadeIn();
                        break;
                    case Animation.FadeOut:
                        text.FadeOut();
                        break;
                    case Animation.RotateSlideIn:
                        text.RotateSlideIn(new Vector3(0, 0, 0));
                        break;
                    case Animation.RotateSlideOut:
                        text.RotateSlideOut();
                        break;
                    case Animation.Grow:
                        text.Grow();
                        break;
                    case Animation.Shrink:
                        text.Shrink();
                        break;
                    case Animation.GrowSlideIn:
                        text.GrowSlideIn();
                        break;
                    case Animation.ShrinkSlideOut:
                        text.ShrinkSlideOut();
                        break;
                    case Animation.ScaleUp:
                        text.ScaleUp();
                        break;
                    case Animation.ScaleDown:
                        text.ScaleDown();
                        break;
                    case Animation.BreatheOnce:
                        text.Breathe().SetLoops(1);
                        break;
                    case Animation.BreatheLoop:
                        text.Breathe().SetLoops(5).OnComplete(text, (text) => SetDebugText("Limited with 5 loops"));
                        break;
                    case Animation.ShakeCameraPos:
                        SetDebugText("You cant use this Animation on Text Element");
                        break;
                    case Animation.ShakeCameraRot:
                        SetDebugText("You cant use this Animation on Text Element");
                        break;
                    case Animation.ChangeCameraAspect:
                        SetDebugText("You cant use this Animation on Text Element");
                        break;
                    case Animation.PunchScale:
                        text.PunchScale();
                        break;
                    case Animation.TypeText:
                        text.TypeText(text.Text.text);
                        break;
                    case Animation.GrowHorizontal:
                        text.GrowHorizontal();
                        break;
                    case Animation.ShrinkHorizontal:
                        text.ShrinkHorizontal();
                        break;
                    case Animation.GrowVertical:
                        text.GrowVertical();
                        break;
                    case Animation.ShrinkVertical:
                        text.ShrinkVertical();
                        break;
                    case Animation.RollIn:
                        text.Show();
                        text.RollIn();
                        break;
                    case Animation.RollOut:
                        text.RollOut();
                        break;
                    case Animation.RollFadeIn:
                        text.RollFadeIn();
                        break;
                    case Animation.RollFadeOut:
                        text.RollFadeOut();
                        break;
                    case Animation.CustomEffect1:
                        text.Animate().Begin(text.SlideIn()).Also(text.GrowVertical());
                        break;
                    default:
                        break;
                }

                break;

            case Element.Dropdown:

                switch (animation)
                {
                    case Animation.Rotate:
                        dropdown.Rotation();
                        break;
                    case Animation.RotateBack:
                        dropdown.Rotation(new Vector3(0, 0, 0));
                        break;
                    case Animation.SlideIn:
                        dropdown.SlideIn();
                        break;
                    case Animation.SlideOut:
                        dropdown.SlideOut();
                        break;
                    case Animation.FadeSlideIn:
                        dropdown.FadeSlideIn();
                        break;
                    case Animation.FadeSlideOut:
                        dropdown.FadeSlideOut();
                        break;
                    case Animation.FadeIn:
                        dropdown.FadeIn();
                        break;
                    case Animation.FadeOut:
                        dropdown.FadeOut();
                        break;
                    case Animation.RotateSlideIn:
                        dropdown.RotateSlideIn(new Vector3(0, 0, 0));
                        break;
                    case Animation.RotateSlideOut:
                        dropdown.RotateSlideOut();
                        break;
                    case Animation.Grow:
                        dropdown.Grow();
                        break;
                    case Animation.Shrink:
                        dropdown.Shrink();
                        break;
                    case Animation.GrowSlideIn:
                        dropdown.GrowSlideIn();
                        break;
                    case Animation.ShrinkSlideOut:
                        dropdown.ShrinkSlideOut();
                        break;
                    case Animation.ScaleUp:
                        dropdown.ScaleUp();
                        break;
                    case Animation.ScaleDown:
                        dropdown.ScaleDown();
                        break;
                    case Animation.BreatheOnce:
                        dropdown.Breathe().SetLoops(1);
                        break;
                    case Animation.BreatheLoop:
                        dropdown.Breathe().SetLoops(5).OnComplete(dropdown, (dropdown) => SetDebugText("Limited with 5 loops"));
                        break;
                    case Animation.ShakeCameraPos:
                        SetDebugText("You cant use this Animation on Dropdown Element");
                        break;
                    case Animation.ShakeCameraRot:
                        SetDebugText("You cant use this Animation on Dropdown Element");
                        break;
                    case Animation.ChangeCameraAspect:
                        SetDebugText("You cant use this Animation on Dropdown Element");
                        break;
                    case Animation.PunchScale:
                        dropdown.PunchScale();
                        break;
                    case Animation.TypeText:
                        SetDebugText("You cant use this Animation on Dropdown Element");
                        break;
                    case Animation.GrowHorizontal:
                        dropdown.GrowHorizontal();
                        break;
                    case Animation.ShrinkHorizontal:
                        dropdown.ShrinkHorizontal();
                        break;
                    case Animation.GrowVertical:
                        dropdown.GrowVertical();
                        break;
                    case Animation.ShrinkVertical:
                        dropdown.ShrinkVertical();
                        break;
                    case Animation.RollIn:
                        dropdown.Show();
                        dropdown.RollIn();
                        break;
                    case Animation.RollOut:
                        dropdown.RollOut();
                        break;
                    case Animation.RollFadeIn:
                        dropdown.RollFadeIn();
                        break;
                    case Animation.RollFadeOut:
                        dropdown.RollFadeOut();
                        break;
                    default:
                        break;
                }

                break;

            case Element.Slider:

                switch (animation)
                {
                    case Animation.Rotate:
                        slider.Rotation();
                        break;
                    case Animation.RotateBack:
                        slider.Rotation(new Vector3(0, 0, 0));
                        break;
                    case Animation.SlideIn:
                        slider.SlideIn();
                        break;
                    case Animation.SlideOut:
                        slider.SlideOut();
                        break;
                    case Animation.FadeSlideIn:
                        slider.FadeSlideIn();
                        break;
                    case Animation.FadeSlideOut:
                        slider.FadeSlideOut();
                        break;
                    case Animation.FadeIn:
                        slider.FadeIn();
                        break;
                    case Animation.FadeOut:
                        slider.FadeOut();
                        break;
                    case Animation.RotateSlideIn:
                        slider.RotateSlideIn(new Vector3(0, 0, 0));
                        break;
                    case Animation.RotateSlideOut:
                        slider.RotateSlideOut();
                        break;
                    case Animation.Grow:
                        slider.Grow();
                        break;
                    case Animation.Shrink:
                        slider.Shrink();
                        break;
                    case Animation.GrowSlideIn:
                        slider.GrowSlideIn();
                        break;
                    case Animation.ShrinkSlideOut:
                        slider.ShrinkSlideOut();
                        break;
                    case Animation.ScaleUp:
                        slider.ScaleUp();
                        break;
                    case Animation.ScaleDown:
                        slider.ScaleDown();
                        break;
                    case Animation.BreatheOnce:
                        slider.Breathe().SetLoops(1);
                        break;
                    case Animation.BreatheLoop:
                        slider.Breathe().SetLoops(5).OnComplete(slider, (slider) => SetDebugText("Limited with 5 loops"));
                        break;
                    case Animation.ShakeCameraPos:
                        SetDebugText("You cant use this Animation on Slider Element");
                        break;
                    case Animation.ShakeCameraRot:
                        SetDebugText("You cant use this Animation on Slider Element");
                        break;
                    case Animation.ChangeCameraAspect:
                        SetDebugText("You cant use this Animation on Slider Element");
                        break;
                    case Animation.PunchScale:
                        slider.PunchScale();
                        break;
                    case Animation.TypeText:
                        SetDebugText("You cant use this Animation on Slider Element");
                        break;
                    case Animation.GrowHorizontal:
                        slider.GrowHorizontal();
                        break;
                    case Animation.ShrinkHorizontal:
                        slider.ShrinkHorizontal();
                        break;
                    case Animation.GrowVertical:
                        slider.GrowVertical();
                        break;
                    case Animation.ShrinkVertical:
                        slider.ShrinkVertical();
                        break;
                    case Animation.RollIn:
                        slider.Show();
                        slider.RollIn();
                        break;
                    case Animation.RollOut:
                        slider.RollOut();
                        break;
                    case Animation.RollFadeIn:
                        slider.RollFadeIn();
                        break;
                    case Animation.RollFadeOut:
                        slider.RollFadeOut();
                        break;
                    default:
                        break;
                }

                break;

            case Element.InputField:

                switch (animation)
                {
                    case Animation.Rotate:
                        inputField.Rotation();
                        break;
                    case Animation.RotateBack:
                        inputField.Rotation(new Vector3(0, 0, 0));
                        break;
                    case Animation.SlideIn:
                        inputField.SlideIn();
                        break;
                    case Animation.SlideOut:
                        inputField.SlideOut();
                        break;
                    case Animation.FadeSlideIn:
                        inputField.FadeSlideIn();
                        break;
                    case Animation.FadeSlideOut:
                        inputField.FadeSlideOut();
                        break;
                    case Animation.FadeIn:
                        inputField.FadeIn();
                        break;
                    case Animation.FadeOut:
                        inputField.FadeOut();
                        break;
                    case Animation.RotateSlideIn:
                        inputField.RotateSlideIn(new Vector3(0, 0, 0));
                        break;
                    case Animation.RotateSlideOut:
                        inputField.RotateSlideOut();
                        break;
                    case Animation.Grow:
                        inputField.Grow();
                        break;
                    case Animation.Shrink:
                        inputField.Shrink();
                        break;
                    case Animation.GrowSlideIn:
                        inputField.GrowSlideIn();
                        break;
                    case Animation.ShrinkSlideOut:
                        inputField.ShrinkSlideOut();
                        break;
                    case Animation.ScaleUp:
                        inputField.ScaleUp();
                        break;
                    case Animation.ScaleDown:
                        inputField.ScaleDown();
                        break;
                    case Animation.BreatheOnce:
                        inputField.Breathe().SetLoops(1);
                        break;
                    case Animation.BreatheLoop:
                        inputField.Breathe().SetLoops(5).OnComplete(inputField, (inputfield) => SetDebugText("Limited with 5 loops"));
                        break;
                    case Animation.ShakeCameraPos:
                        SetDebugText("You cant use this Animation on Input Field Element");
                        break;
                    case Animation.ShakeCameraRot:
                        SetDebugText("You cant use this Animation on Input Field Element");
                        break;
                    case Animation.ChangeCameraAspect:
                        SetDebugText("You cant use this Animation on Input Field Element");
                        break;
                    case Animation.PunchScale:
                        inputField.PunchScale();
                        break;
                    case Animation.TypeText:
                        inputField.TypeText();
                        break;
                    case Animation.GrowHorizontal:
                        inputField.GrowHorizontal();
                        break;
                    case Animation.ShrinkHorizontal:
                        inputField.ShrinkHorizontal();
                        break;
                    case Animation.GrowVertical:
                        inputField.GrowVertical();
                        break;
                    case Animation.ShrinkVertical:
                        inputField.ShrinkVertical();
                        break;
                    case Animation.RollIn:
                        inputField.Show();
                        inputField.RollIn();
                        break;
                    case Animation.RollOut:
                        inputField.RollOut();
                        break;
                    case Animation.RollFadeIn:
                        inputField.RollFadeIn();
                        break;
                    case Animation.RollFadeOut:
                        inputField.RollFadeOut();
                        break;
                    default:
                        break;
                }

                break;

            case Element.ProgressBar:

                switch (animation)
                {
                    case Animation.Rotate:
                        progressBar.Rotation();
                        break;
                    case Animation.RotateBack:
                        progressBar.Rotation(new Vector3(0, 0, 0));
                        break;
                    case Animation.SlideIn:
                        progressBar.SlideIn();
                        break;
                    case Animation.SlideOut:
                        progressBar.SlideOut();
                        break;
                    case Animation.FadeSlideIn:
                        progressBar.FadeSlideIn();
                        break;
                    case Animation.FadeSlideOut:
                        progressBar.FadeSlideOut();
                        break;
                    case Animation.FadeIn:
                        progressBar.FadeIn();
                        break;
                    case Animation.FadeOut:
                        progressBar.FadeOut();
                        break;
                    case Animation.RotateSlideIn:
                        progressBar.RotateSlideIn(new Vector3(0, 0, 0));
                        break;
                    case Animation.RotateSlideOut:
                        progressBar.RotateSlideOut();
                        break;
                    case Animation.Grow:
                        progressBar.Grow();
                        break;
                    case Animation.Shrink:
                        progressBar.Shrink();
                        break;
                    case Animation.GrowSlideIn:
                        progressBar.GrowSlideIn();
                        break;
                    case Animation.ShrinkSlideOut:
                        progressBar.ShrinkSlideOut();
                        break;
                    case Animation.ScaleUp:
                        progressBar.ScaleUp();
                        break;
                    case Animation.ScaleDown:
                        progressBar.ScaleDown();
                        break;
                    case Animation.BreatheOnce:
                        progressBar.Breathe().SetLoops(1);
                        break;
                    case Animation.BreatheLoop:
                        progressBar.Breathe().SetLoops();
                        break;
                    case Animation.ShakeCameraPos:
                        SetDebugText("You cant use this Animation on Progress Bar Element");
                        break;
                    case Animation.ShakeCameraRot:
                        SetDebugText("You cant use this Animation on Progress Bar Element");
                        break;
                    case Animation.ChangeCameraAspect:
                        SetDebugText("You cant use this Animation on Progress Bar Element");
                        break;
                    case Animation.PunchScale:
                        progressBar.PunchScale();
                        break;
                    case Animation.TypeText:
                        SetDebugText("You cant use this Animation on Progress Bar Element");
                        break;
                    case Animation.GrowHorizontal:
                        progressBar.GrowHorizontal();
                        break;
                    case Animation.ShrinkHorizontal:
                        progressBar.ShrinkHorizontal();
                        break;
                    case Animation.GrowVertical:
                        progressBar.GrowVertical();
                        break;
                    case Animation.ShrinkVertical:
                        progressBar.ShrinkVertical();
                        break;
                    case Animation.RollIn:
                        progressBar.Show();
                        progressBar.RollIn();
                        break;
                    case Animation.RollOut:
                        progressBar.RollOut();
                        break;
                    case Animation.RollFadeIn:
                        progressBar.RollFadeIn();
                        break;
                    case Animation.RollFadeOut:
                        progressBar.RollFadeOut();
                        break;
                    default:
                        break;
                }

                break;

            case Element.Panel:

                switch (animation)
                {
                    case Animation.Rotate:
                        panel.Rotation();
                        break;
                    case Animation.RotateBack:
                        panel.Rotation(new Vector3(0, 0, 0));
                        break;
                    case Animation.SlideIn:
                        panel.SlideIn();
                        break;
                    case Animation.SlideOut:
                        panel.SlideOut();
                        break;
                    case Animation.FadeSlideIn:
                        panel.FadeSlideIn();
                        break;
                    case Animation.FadeSlideOut:
                        panel.FadeSlideOut();
                        break;
                    case Animation.FadeIn:
                        panel.FadeIn();
                        break;
                    case Animation.FadeOut:
                        panel.FadeOut();
                        break;
                    case Animation.RotateSlideIn:
                        panel.RotateSlideIn(new Vector3(0, 0, 0));
                        break;
                    case Animation.RotateSlideOut:
                        panel.RotateSlideOut();
                        break;
                    case Animation.Grow:
                        panel.Grow();
                        break;
                    case Animation.Shrink:
                        panel.Shrink();
                        break;
                    case Animation.GrowSlideIn:
                        panel.GrowSlideIn();
                        break;
                    case Animation.ShrinkSlideOut:
                        panel.ShrinkSlideOut();
                        break;
                    case Animation.ScaleUp:
                        panel.ScaleUp();
                        break;
                    case Animation.ScaleDown:
                        panel.ScaleDown();
                        break;
                    case Animation.BreatheOnce:
                        panel.Breathe().SetLoops(1);
                        break;
                    case Animation.BreatheLoop:
                        panel.Breathe().SetLoops(5).OnComplete(panel, (panel) => SetDebugText("Limited with 5 loops"));
                        break;
                    case Animation.ShakeCameraPos:
                        SetDebugText("You cant use this Animation on Panel Element");
                        break;
                    case Animation.ShakeCameraRot:
                        SetDebugText("You cant use this Animation on Panel Element");
                        break;
                    case Animation.ChangeCameraAspect:
                        SetDebugText("You cant use this Animation on Panel Element");
                        break;
                    case Animation.PunchScale:
                        panel.PunchScale();
                        break;
                    case Animation.GrowHorizontal:
                        panel.GrowHorizontal();
                        break;
                    case Animation.ShrinkHorizontal:
                        panel.ShrinkHorizontal();
                        break;
                    case Animation.GrowVertical:
                        panel.GrowVertical();
                        break;
                    case Animation.ShrinkVertical:
                        panel.ShrinkVertical();
                        break;
                    case Animation.RollIn:
                        panel.Show();
                        panel.RollIn();
                        break;
                    case Animation.RollOut:
                        panel.RollOut();
                        break;
                    case Animation.RollFadeIn:
                        panel.RollFadeIn();
                        break;
                    case Animation.RollFadeOut:
                        panel.RollFadeOut();
                        break;
                    case Animation.TypeText:
                        SetDebugText("You cant use this Animation on Panel Element");
                        break;
                    case Animation.CustomEffect1:
                        panel.Animate().Begin(panel.SlideIn()).Also(panel.GrowVertical());
                        break;
                    default:
                        break;
                }

                break;
            default:
                break;
        }
    }

    #endregion
}






