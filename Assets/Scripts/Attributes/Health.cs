using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats; // NOTE: we'll want to remove this dependency
using UnityEngine;

namespace RPG.Attributes
{
   public class Health : MonoBehaviour, IJsonSaveable
   {
      public bool IsDead { get; private set; }

      [SerializeField] float healthPoints = 100f;

      private void Awake()
      {
         IsDead = false;
      }

      private void Start()
      {
         // Note: Sam makes the point that this might sometimes get called AFTER save is
         //       restored, thus overwriting what we read on load. We'll fix this in a few
         //       lectures, but we'll live with it for now.
         healthPoints = GetComponent<BaseStats>().GetHealth();
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
         print("Restoring Health data");

         healthPoints = state.ToObject<float>();

         if (healthPoints <= 0)
         {
            Die();
         }
      }
   }
}