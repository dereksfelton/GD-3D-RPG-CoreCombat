using UnityEngine;

namespace RPG.Combat
{
   public class Health : MonoBehaviour
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
      }
   }
}
