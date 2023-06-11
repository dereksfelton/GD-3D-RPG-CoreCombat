using UnityEngine;

namespace RPG.SceneManagement
{
   public class Portal : MonoBehaviour
   {
      void OnTriggerEnter(Collider other) {
         if (other.gameObject.CompareTag("Player"))
         {
            print ("Portal entered");
         }
      }
   }
}