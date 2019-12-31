using UnityEngine;

public class MenuFade : MonoBehaviour {
    public CanvasGroup canvasGroup;

    public float fadeSpeed = 5f;
    private bool shouldFadeIn;
    private bool shouldFadeOut;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (shouldFadeIn) {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);
            if (canvasGroup.alpha == 1) {
                shouldFadeIn = false;
            }
        } else if (shouldFadeOut) {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeSpeed * Time.deltaTime);
            if (canvasGroup.alpha == 0) {
                shouldFadeOut = false;
                gameObject.SetActive(false);
            }}
    }

    public void FadeIn() {
        shouldFadeIn = true;
    }

    public void FadeOut() {
        shouldFadeOut = true;
    }
}
