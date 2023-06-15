using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        private void Start() {
            canvasGroup = GetComponent<CanvasGroup>();
            //StartCoroutine(FadeOutThenIn()); // uncomment to see this in action
        }

        // only used to demo in this video ... won't be used elsewhere
        IEnumerator FadeOutThenIn() {
            yield return FadeOut(3f);
            print("Faded out");
            yield return FadeIn(1.5f);
            print("Faded in");
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