using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
   public class Portal : MonoBehaviour
   {
      [SerializeField] int sceneToLoad = -1;

      void OnTriggerEnter(Collider other) {
         if (other.gameObject.CompareTag("Player"))
         {
            StartCoroutine(Transition());
         }
      }

      private IEnumerator Transition()
      {
         // NOTE: DontDestroyOnLoad only works when gameObject is at scene root!
         DontDestroyOnLoad(gameObject); 
         AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneToLoad);
         while (!loadingScene.isDone) yield return null;
         print("Scene loaded");
         Destroy(gameObject);
      }
   }
}