using Newtonsoft.Json.Linq;
using RPG.JsonSaving;
using UnityEngine;

namespace RPG.Core
{
   public class Health : MonoBehaviour, IJsonSaveable
   {
      public bool IsDead { get; private set; }

      [SerializeField] float healthPoints = 100f;

      private void Awake() {
         IsDead = false;
      }

      public void TakeDamage(float damage)
      {
         healthPoints = Mathf.Max(0, healthPoints - damage);
         
         if (healthPoints == 0)
         {
            Die();
         }
      }

      private void Die()
      {
         if (IsDead) return;

         //  trigger the death animation
         IsDead = true;
         GetComponent<Animator>().SetTrigger("die");

         // cancel currently running actions
         GetComponent<ActionScheduler>().CancelCurrentAction();
      }

      // implement IJsonSaveable interface_________________________________________________
      public JToken CaptureAsJToken()
      {
         return JToken.FromObject(healthPoints);
      }

      public void RestoreFromJToken(JToken state)
      {
         healthPoints = state.ToObject<float>();

         if (healthPoints <= 0)
         {
            Die();
         }
      }
   }
}
