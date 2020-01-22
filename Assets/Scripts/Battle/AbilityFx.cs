using UnityEngine;

public class AbilityFx : MonoBehaviour {

    public float effectDuration;
    public int sfxId;

    // Start is called before the first frame update
    void Start() {
        AudioManager.instance.PlaySfx("blocked");
    }

    // Update is called once per frame
    void Update() {
        Destroy(gameObject, effectDuration);
    }
}
