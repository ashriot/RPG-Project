using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioSource[] bgms;
    public AudioSource[] sfxs;

    public static AudioManager instance;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBgm(string name) {
        Debug.Log(name);
        if (name == "dungeon") {
            PlayBgmId(0);
        } else if (name == "battle") {
            PlayBgmId(1);
        } else if (name == "victory") {
            PlayBgmId(2);
        } else if (name == "town") {
            PlayBgmId(3);
        } else if (name == "epilogue") {
            PlayBgmId(4);
        } else if (name == "prologue") {
            PlayBgmId(5);
        }
    }

    public void PlayBgmId(int id) {
        if (!bgms[id].isPlaying) {
            StopMusic();
            if (id < bgms.Length) {
                bgms[id].volume = 0.2f;
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
         } else if (name == "click") {
            sfxs[7].Play();
         } else if (name == "coin") {
            sfxs[8].Play();
         } else if (name == "gold") {
            sfxs[9].Play();
         } else if (name == "select_a") {
            sfxs[10].Play();
         } else if (name == "error") {
            sfxs[11].Play();
         } else if (name == "score") {
            sfxs[12].Play();
         } else if (name == "impact_a") {
            sfxs[13].Play();
         } else if (name == "impact_b") {
            sfxs[14].Play();
         } else if (name == "swing") {
            sfxs[15].Play();
         } else if (name == "rebound") {
            sfxs[16].Play();
         } else if (name == "fire_c") {
            sfxs[17].Play();
         } else if (name == "end_turn") {
            sfxs[18].Play();
         } else if (name == "special_a") {
            sfxs[19].Play();
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
