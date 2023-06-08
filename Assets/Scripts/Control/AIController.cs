using System;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
   public class AIController : MonoBehaviour
   {
      [SerializeField] float chaseDistance = 5f;
      [SerializeField] float suspicionDuration = 5f; // in seconds
      [SerializeField] float waypointTolerance = 1f; // in meters

      [SerializeField] PatrolPath patrolPath;

      // cached references
      Fighter fighter;
      Health health;
      Mover mover;
      GameObject player;

      Vector3 guardPosition;
      float timeSinceLastSawPlayer = Mathf.Infinity;
      int currentWaypointIndex = 0; // note irrelevant if a PatrolPath isn't assigned

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

         // if we're in range of the player and can attack, do so
         if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
         {
            timeSinceLastSawPlayer = 0;
            AttackBehavior();
         }
         // otherwise, if we're still suspicious, cancel other actions
         else if (timeSinceLastSawPlayer < suspicionDuration)
         {
            SuspicionBehavior();
         }
         // otherwise, return to my guard position
         else
         {
            PatrolBehavior();
         }

         timeSinceLastSawPlayer += Time.deltaTime;
      }

      private void AttackBehavior()
      {
         fighter.Attack(player);
      }

      private void SuspicionBehavior()
      {
         GetComponent<ActionScheduler>().CancelCurrentAction();
      }

      private void PatrolBehavior()
      {
         Vector3 nextPosition = guardPosition; // default when no patrol path is assigned

         if (patrolPath != null)
         {
            if (AtWaypoint())
            {
               CycleWaypoint();
            }
            nextPosition = GetCurrentWaypoint();
         }

         mover.StartMoveAction(nextPosition);
      }

      private bool AtWaypoint()
      {
         float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
         print($"{gameObject.name} is {distanceToWaypoint} from waypoint");
         return distanceToWaypoint <= waypointTolerance;
      }

      private void CycleWaypoint()
      {
         currentWaypointIndex = patrolPath.GetNextWaypointIndex(currentWaypointIndex);
      }

      private Vector3 GetCurrentWaypoint()
      {
         return patrolPath.GetWaypoint(currentWaypointIndex);
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
