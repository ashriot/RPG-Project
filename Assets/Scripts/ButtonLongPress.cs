using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
 
public class ButtonLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
    [SerializeField, Tooltip("How long must pointer be down on this object to trigger a long press")]
    public UnityEvent onClick = new UnityEvent();
    
    public string description;

    // public UnityEvent onLongPress = new UnityEvent();
    // public UnityEvent onRelease = new UnityEvent();
 
    private float holdTime = .33f;
    private float timePressStarted;
    private bool isHeld = false;
    private bool longPressTriggered = false;
 
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
        isHeld = false;
    }
 
    private void OnLongPress() {
        isHeld = true;
        description = description.Replace("\\n", "\n");
        GameMenu.instance.ShowTooltip(description);

    }

    private void OnRelease() {
        isHeld = false;
        GameMenu.instance.HideTooltip();
    }
}