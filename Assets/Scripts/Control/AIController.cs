using RPG.Combat;
using RPG.Core;
using UnityEngine;

namespace RPG.Control
{
   public class AIController : MonoBehaviour
   {
      [SerializeField] float chaseDistance = 5f;

      // cached references
      Fighter fighter;
      GameObject player;
      Health health;

      private void Start() {
         fighter = GetComponent<Fighter>();
         health = GetComponent<Health>();
         player = GameObject.FindWithTag("Player");
      }

      private void Update()
      {
         if (health.IsDead) return; // don't do ANYTHING if you're dead!

         if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
         {
            fighter.Attack(player);
         }
         else
         {
            fighter.Cancel();
         }
      }

      private bool InAttackRangeOfPlayer()
      {
         float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
         return distanceToPlayer <= chaseDistance;
      }
   }
}
