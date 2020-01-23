using UnityEngine;
using UnityEngine.UI;

public class EquipmentWindow : MonoBehaviour {
    public EquipmentButton headButton;
    public EquipmentButton bodyButton;
    public EquipmentButton armsButton;
    public EquipmentButton feetButton;
    public EquipmentButton ringLButton;
    public EquipmentButton ringRButton;
    public EquipmentButton mainHandButton;
    public EquipmentButton offHandButton;
    public EquippableItem head;
    public EquippableItem body;
    public EquippableItem arms;
    public EquippableItem feet;
    public EquippableItem ringL;
    public EquippableItem ringR;
    public Hands mainHand;
    public Hands offHand;

    public EquipmentButton[] buttons;
    public EquippableItem[] equipment;
    public Hands[] hands;

    public Text mainHandAtkOrBlk;
    public Text mainHandDmgOrAmt;

    public Text offHandAtkOrBlk;
    public Text offHandDmgOrAmt;
}
