using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
   public class AIController : MonoBehaviour
   {
      [SerializeField] float chaseDistance = 5f;

      // cached references
      Fighter fighter;
      Health health;
      Mover mover;
      GameObject player;

      Vector3 guardPosition;

      private void Start() {
         fighter = GetComponent<Fighter>();
         health = GetComponent<Health>();
         mover = GetComponent<Mover>();
         player = GameObject.FindWithTag("Player");

         // set my guard location as my startup position
         guardPosition = transform.position;
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
            mover.StartMoveAction(guardPosition);
         }
      }

      private bool InAttackRangeOfPlayer()
      {
         float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
         return distanceToPlayer <= chaseDistance;
      }

      // called by Unity...
      private void OnDrawGizmosSelected() {
         Gizmos.color = Color.blue;
         Gizmos.DrawWireSphere(transform.position, chaseDistance);
      }
   }
}
