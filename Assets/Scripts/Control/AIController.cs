using RPG.Combat;
using UnityEngine;

namespace RPG.Control
{
   public class AIController : MonoBehaviour
   {
      [SerializeField] float chaseDistance = 5f;

      // cached references
      Fighter fighter;
      GameObject player;

      private void Start() {
         fighter = GetComponent<Fighter>();
         player = GameObject.FindWithTag("Player");
      }

      private void Update()
      {
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
