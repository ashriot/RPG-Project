using UnityEngine;
using UnityEngine.UI;

public class EquipmentWindow : MonoBehaviour {
    public ButtonWithImage headButton;
    public ButtonWithImage bodyButton;
    public ButtonWithImage armsButton;
    public ButtonWithImage feetButton;
    public ButtonWithImage ringLButton;
    public ButtonWithImage ringRButton;
    public ButtonWithImage mainHandButton;
    public ButtonWithImage offHandButton;
    public EquippableItem head;
    public EquippableItem body;
    public EquippableItem arms;
    public EquippableItem feet;
    public EquippableItem ringL;
    public EquippableItem ringR;
    public Hands mainHand;
    public Hands offHand;

    public Text mainHandAtkOrBlk;
    public Text mainHandDmgOrAmt;

    public Text offHandAtkOrBlk;
    public Text offHandDmgOrAmt;
}
