using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
   public class Portal : MonoBehaviour
   {
      [SerializeField] int sceneToLoad = -1;
      [SerializeField] Transform spawnPoint;

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

         Portal otherPortal = GetOtherPortal();
         UpdatePlayer(otherPortal);

         Destroy(gameObject);
      }

      private void UpdatePlayer(Portal otherPortal)
      {
         GameObject player = GameObject.FindWithTag("Player");
         player.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
         player.transform.rotation = otherPortal.spawnPoint.rotation;
      }

      private Portal GetOtherPortal()
      {
         foreach (Portal portal in FindObjectsOfType<Portal>())
         {
            if (portal == this) continue;
            return portal;
         }
         return null;
      }
   }
}