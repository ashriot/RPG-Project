using UnityEngine;

public class Clickable : MonoBehaviour {

    public new string name;
    public string description;

    public string GetInfo() {
        return name + '\n' + description;
    }
}
