using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
    public TabGroup tabGroup;
    public Image backgroundImage;
    
    public UnityEvent onClick = new UnityEvent();
    public string description;
 
    private float timePressStarted;
    private bool isHeld = false;
    private bool longPressTriggered = false;
    private float holdTime = .33f;

    void Awake() {
        backgroundImage = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }
    
    private void Update() {
        if (isHeld && !longPressTriggered) {
            if (Time.time - timePressStarted > holdTime) {
                longPressTriggered = true;
                OnLongPress();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        tabGroup.OnTabExit(this);
        isHeld = false;
    }

    public void OnPointerDown(PointerEventData eventData) {
        timePressStarted = Time.time;
        isHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isHeld = false;
        if (longPressTriggered) {
            OnLongRelease();
            longPressTriggered = false;
        } else {
            if (tabGroup.selectedTab == this) { return; }
            tabGroup.OnTabSelected(this);
            onClick.Invoke();
        }
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

    private void OnLongRelease() {
        isHeld = false;
        if (GameMenu.instance != null) {
            GameMenu.instance.HideTooltip();
        } else {
            PartyCreation.instance.HideTooltip();
        }
    }
}
