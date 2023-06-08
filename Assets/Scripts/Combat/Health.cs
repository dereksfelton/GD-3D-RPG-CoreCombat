using UnityEngine;

namespace RPG.Combat
{
   public class Health : MonoBehaviour
   {
      private bool isDead = false;

      [SerializeField] float healthPoints = 100f;

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
         if (isDead) return;

         //  trigger the death animation
         isDead = true;
         GetComponent<Animator>().SetTrigger("die");
      }
   }
}
