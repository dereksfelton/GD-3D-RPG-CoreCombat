using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate() {
            canvasGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time) {
            // do only while alpha < 1
            while (canvasGroup.alpha < 1)  {
                // increment the alpha by delta time / time
                canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time) {
            // do only while alpha > 0
            while (canvasGroup.alpha > 0)  {
                // increment the alpha by delta time / time
                canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}