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
            SceneManager.LoadScene(sceneToLoad);
         }
      }
   }
}