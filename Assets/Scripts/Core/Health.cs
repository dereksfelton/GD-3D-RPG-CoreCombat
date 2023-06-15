using RPG.Saving;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Core
{
   public class Health : MonoBehaviour, ISaveable
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

      // implement ISaveable interface_________________________________________________
      public object CaptureState()
      {
         return healthPoints;
      }

      public void RestoreState(object state)
      {
         healthPoints = (float)state;

         if (healthPoints == 0)
         {
            Die();
         }
      }
   }
}
