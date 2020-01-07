using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashImage : MonoBehaviour {
    public Image flashImage;
    Color startColor;

    void Start() {
        startColor = flashImage.color;
    }

    public void FlashWhite(int blinkCount, float duration) {
        StartCoroutine(Blink(Color.white, blinkCount, duration));
    }

    public IEnumerator ColorLerpTo(Color _color, float _duration) {
        float elapsedTime = 0.0f;
        while (elapsedTime < _duration) {
            flashImage.color = Color.Lerp(flashImage.color, _color, (elapsedTime / _duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator Blink(Color _blinkIn, int _blinkCount, float _totalBlinkDuration) {
        // We divide the whole duration for the ammount of blinks we will perform
        float fractionalBlinkDuration = _totalBlinkDuration / _blinkCount;

        for (int blinked = 0; blinked < _blinkCount; blinked++) {
            // Each blink needs 2 lerps: we give each lerp half of the duration allocated for 1 blink
            float halfFractionalDuration = fractionalBlinkDuration * 0.5f;

            // Lerp to the color
            yield return StartCoroutine(ColorLerpTo(_blinkIn, halfFractionalDuration));

            // Lerp to transparent
            StartCoroutine(ColorLerpTo(Color.clear, halfFractionalDuration));
        }
    }
}