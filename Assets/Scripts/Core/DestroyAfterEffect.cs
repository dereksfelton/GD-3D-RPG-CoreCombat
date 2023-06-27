using UnityEngine;

namespace RPG.Core
{
   public class DestroyAfterEffect : MonoBehaviour
   {
      [SerializeField] private GameObject targetToDestroy = null;

      void Update()
      {
         if (!GetComponent<ParticleSystem>().IsAlive())
         {
            // if we specified a target to destroy, destroy it.
            // otherwise just destroy my current game object
            Destroy(targetToDestroy ? targetToDestroy : gameObject);
         }
      }
   }
}