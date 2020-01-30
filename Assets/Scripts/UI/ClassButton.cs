using UnityEngine;

public class ClassButton : MonoBehaviour {

    public TabButton tabButton;
    public Hero hero;

    public void Click() {
        PartyCreation.instance.ClickClass(this);
    }
}
