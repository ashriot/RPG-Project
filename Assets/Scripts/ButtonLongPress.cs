using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
 
public class ButtonLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
    [SerializeField, Tooltip("How long must pointer be down on this object to trigger a long press")]
    // public UnityEvent onClick = new UnityEvent();
    public UnityEvent onLongPress = new UnityEvent();
 
    private float holdTime = .5f;
    // private bool held = false;
 
    public void OnPointerDown(PointerEventData eventData) {
        // held = false;
        Invoke("OnLongPress", holdTime);
    }
 
    public void OnPointerUp(PointerEventData eventData) {
        CancelInvoke("OnLongPress");
        BattleManager.instance.actionTooltip.description.text = string.Empty;
        // if (!held) {
        //    onClick.Invoke();
        // }
    }
 
    public void OnPointerExit(PointerEventData eventData) {
        CancelInvoke("OnLongPress");
    }
 
    private void OnLongPress() {
        // held = true;
        onLongPress.Invoke();
    }
}