using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
   public class Fader : MonoBehaviour
   {
      CanvasGroup canvasGroup;
      Coroutine currentlyActiveFade = null;
      private void Awake()
      {
         canvasGroup = GetComponent<CanvasGroup>();
      }

      public void FadeOutImmediate()
      {
         canvasGroup.alpha = 1;
      }

      public Coroutine FadeIn(float time)
      {
         return Fade(0, time);
      }

      public Coroutine FadeOut(float time)
      {
         return Fade(1, time);
      }

      public Coroutine Fade(float targetAlpha, float time)
      {
         // cancel any running coroutines
         if (currentlyActiveFade != null)
         {
            StopCoroutine(currentlyActiveFade);
         }

         // run the fade coroutine
         currentlyActiveFade = StartCoroutine(FadeRoutine(targetAlpha, time));

         return currentlyActiveFade;
      }

      private IEnumerator FadeRoutine(float targetAlpha, float time)
      {
         // do only while alpha <> the target
         while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
         {
            // move the alpha toward the target based on delta time / time
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Time.deltaTime / time);
            yield return null;
         }
      }
   }
}