using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public class ButtonLongPress : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

    public UnityEvent onClick = new UnityEvent();
    public string description;
    
    public Sprite idleSprite;
    public Sprite hoverSprite;
    
 
    public Image backgroundImage;
    private float holdTime = .33f;
    private float timePressStarted;
    private bool isHeld = false;
    private bool longPressTriggered = false;
 
    private void Awake() {
        backgroundImage = GetComponent<Image>();

    }

    private void OnEnable() {
        if (idleSprite != null) { backgroundImage.sprite = idleSprite; }
    }

    private void Update() {
        if (isHeld && !longPressTriggered) {
            if (Time.time - timePressStarted > holdTime) {
                longPressTriggered = true;
                OnLongPress();
            }
        }
    }
 
    public void OnPointerDown(PointerEventData eventData) {
        timePressStarted = Time.time;
        isHeld = true;
    }
 
    public void OnPointerUp(PointerEventData eventData) {
        isHeld = false;
        if (longPressTriggered) {
            OnRelease();
            longPressTriggered = false;
        } else {
            onClick.Invoke();
        }
    }
 
    public void OnPointerExit(PointerEventData eventData) {
        if (idleSprite != null) { backgroundImage.sprite = idleSprite; }
        isHeld = false;
    }
 
    private void OnLongPress() {
        isHeld = true;
        description = description.Replace("\\n", "\n");
        if (GameMenu.instance != null) {
            GameMenu.instance.ShowTooltip(description);
        } else {
            PartyCreation.instance.ShowTooltip(description);
        }
    }

    private void OnRelease() {
        if (idleSprite != null) { backgroundImage.sprite = idleSprite; }
        isHeld = false;
        if (GameMenu.instance != null) {
            GameMenu.instance.HideTooltip();
        } else {
            PartyCreation.instance.HideTooltip();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (hoverSprite != null) {
            backgroundImage.sprite = hoverSprite; }
    }
}