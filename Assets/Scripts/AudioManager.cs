using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioSource[] bgms;
    public AudioSource[] sfxs;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Start() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void PlayBgm(string name) {
        if (name == "dungeon") {
            PlayBgmId(0);
        } else if (name == "battle") {
            PlayBgmId(1);
        } else if (name == "victory") {
            PlayBgmId(2);
        }
    }

    public void PlayBgmId(int id) {
        if (!bgms[id].isPlaying) {
            StopMusic();
            if (id < bgms.Length) {
                bgms[id].volume = 0.15f;
                bgms[id].Play();
            } else {
                Debug.LogError("BGM id '" + id + "' does not exist!");
            }
        }
    }

    public void PlaySfx(string name) {
        // var sfx = Array.Find(sfxs, s => s.name == name);
         if (name == "grass") {
            sfxs[2].Play();
         } else if (name == "blocked") {
            sfxs[1].Play();
         } else if (name == "door") {
            sfxs[4].Play();
         } else if (name == "chest") {
            sfxs[5].Play();
         } else if (name == "loot") {
            sfxs[6].Play();
         }
        // } else {
        //     Debug.LogError("SFX named '" + name + "' does not exist!");
        // }
    }

    public void StopMusic() {
        for (var i = 0; i < bgms.Length; i++) {
            bgms[i].Stop();
        }
    }

    public void Mute(string name) {
        if (name == "blocked") {
            sfxs[1].mute = true;
        }
    }

    public void UnMute(string name) {
        if (name == "blocked") {
            sfxs[1].Stop();
            sfxs[1].mute = false;
        }
    }
}
