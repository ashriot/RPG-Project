using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioSource[] bgm;
    public AudioSource[] sfx;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Start() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void PlayBgm(int id) {
        if (!bgm[id].isPlaying) {
            StopMusic();
            if (id < bgm.Length) {
                bgm[id].Play();
            } else {
                Debug.LogError("BGM id '" + id + "' does not exist!");
            }
        }
    }

    public void PlaySfx(int id) {
        if (id < sfx.Length) {
            sfx[id].Play();
        } else {
            Debug.LogError("SFX id '" + id + "' does not exist!");
        }
    }

    public void StopMusic() {
        for (var i = 0; i < bgm.Length; i++) {
            bgm[i].Stop();
        }
    }
}
