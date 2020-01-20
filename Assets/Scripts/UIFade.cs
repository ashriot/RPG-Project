using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour {
    
    public static UIFade instance;

    public Image fadeScreen;
    public FlashImage fullScreen;
    public float fadeSpeed = 1f;
    
    private bool flashing;
    private int flashTimes;
    private bool shouldExpand;
    private bool shouldFadeIn;
    private bool shouldFadeOut;
    private bool shouldCollapse;

    // Start is called before the first frame update
    void Start() {
        instance = this;

        DontDestroyOnLoad(instance);
    }

    // Update is called once per frame
    void Update() {
        if (shouldExpand) {
            var yScale = Mathf.MoveTowards(fadeScreen.rectTransform.localScale.y, 1f, fadeSpeed * Time.deltaTime);

            fadeScreen.rectTransform.localScale = new Vector3(1f, yScale, 1f);

            if (fadeScreen.rectTransform.localScale.y == 1f) {
                shouldExpand = false;
            }
        }

        if (shouldFadeIn) {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
            
            if (fadeScreen.color.a == 0f) {
                shouldFadeIn = false;
                fadeScreen.gameObject.SetActive(false);
                fullScreen.gameObject.SetActive(false);
            }
        }

        if (shouldFadeOut) {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            
            if (fadeScreen.color.a == 1f) {
                shouldFadeOut = false;
            }
        }

        if (shouldCollapse) {
            var yScale = Mathf.MoveTowards(fadeScreen.rectTransform.localScale.y, 0f, fadeSpeed * Time.deltaTime);

            fadeScreen.rectTransform.localScale = new Vector3(1f, yScale, 1f);

            if (fadeScreen.rectTransform.localScale.y == 0f) {
                shouldCollapse = false;
                fadeScreen.gameObject.SetActive(false);
                fullScreen.gameObject.SetActive(false);
            }
        }
    }
    
    // public IEnumerator FadeToBlack(float _duration) {
    //     fadeScreen.gameObject.SetActive(true);
    //     float elapsedTime = 0.0f;
    //     while (elapsedTime < _duration) {
    //         fadeScreen.rectTransform.localScale = Vector3.Lerp(fadeScreen.rectTransform.localPosition, Vector3.one, (elapsedTime / _duration));
    //         elapsedTime += Time.deltaTime;
    //         yield return new WaitForEndOfFrame();
    //     }
    //     fadeScreen.gameObject.SetActive(false);
    // }
    
    // public IEnumerator FadeFromBlack(float _duration) {
    //     fadeScreen.gameObject.SetActive(true);
    //     float elapsedTime = 0.0f;
    //     while (elapsedTime < _duration) {
    //         fadeScreen.color = Color.Lerp(fadeScreen.color, Color.black, (elapsedTime / _duration));
    //         elapsedTime += Time.deltaTime;
    //         yield return new WaitForEndOfFrame();
    //     }
    //     fadeScreen.gameObject.SetActive(false);
    // }

    public void FlashScreen(int times) {
        fullScreen.gameObject.SetActive(true);
        fullScreen.FlashWhite(times, .5f);
    }

    public void Expand() {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.rectTransform.localScale = new Vector3(1f, 0f, 1f);
        shouldExpand = true;
        shouldFadeIn = false;
    }

    public void FadeIn() {
        shouldExpand = false;
        shouldFadeIn = true;
    }

    public void FadeOut() {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.color = Color.clear;
        fadeScreen.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        shouldFadeOut = true;
    }

    public void Collapse() {
        fadeScreen.gameObject.SetActive(true);
        shouldCollapse = true;
    }
}
