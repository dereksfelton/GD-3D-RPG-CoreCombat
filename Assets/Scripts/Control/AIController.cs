using System;
using GameDevTV.Utils;
using RPG.Attributes;
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
      [SerializeField] float waypointDistanceTolerance = 1f; // in meters
      [SerializeField] float waypointDwellTime = 3f; // in seconds

      [Range(0, 1)]
      [SerializeField] float patrolSpeedFraction = 0.2f; // as a percentage of Mover.maxSpeed
      [SerializeField] PatrolPath patrolPath;

      private Vector3 GuardPosition {
         get { return guardPosition.value; }
         set { guardPosition.value = value; }
      }

      // cached references
      Fighter fighter;
      Health health;
      Mover mover;
      GameObject player;

      LazyValue<Vector3> guardPosition;
      float timeSinceLastSawPlayer = Mathf.Infinity;
      float timeSinceArrivedAtWaypoint = Mathf.Infinity; // in seconds
      int currentWaypointIndex = 0; // note irrelevant if a PatrolPath isn't assigned

      private void Awake()
      {
         fighter = GetComponent<Fighter>();
         health = GetComponent<Health>();
         mover = GetComponent<Mover>();
         player = GameObject.FindWithTag("Player");

         guardPosition = new LazyValue<Vector3>(GetGuardPosition);
      }

      private void Start()
      {
         guardPosition.ForceInit();
      }

      private Vector3 GetGuardPosition()
      {
         return transform.position;
      }

      private void Update()
      {
         if (health.IsDead) return; // don't do ANYTHING if you're dead!

         // if we're in range of the player and can attack, do so
         if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
         {
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

         UpdateTimers();
      }

      private void UpdateTimers()
      {
         timeSinceLastSawPlayer += Time.deltaTime;
         timeSinceArrivedAtWaypoint += Time.deltaTime;
      }

      private void AttackBehavior()
      {
         timeSinceLastSawPlayer = 0;
         fighter.Attack(player);
      }

      private void SuspicionBehavior()
      {
         GetComponent<ActionScheduler>().CancelCurrentAction();
      }

      private void PatrolBehavior()
      {
         Vector3 nextPosition = GuardPosition; // default when no patrol path is assigned

         if (patrolPath != null)
         {
            if (AtWaypoint())
            {
               timeSinceArrivedAtWaypoint = 0;
               CycleWaypoint();
            }
            nextPosition = GetCurrentWaypoint();
         }

         if (timeSinceArrivedAtWaypoint > waypointDwellTime)
         {
            mover.StartMoveAction(nextPosition, patrolSpeedFraction);
         }
      }

      private bool AtWaypoint()
      {
         float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
         return distanceToWaypoint <= waypointDistanceTolerance;
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
      private void OnDrawGizmosSelected()
      {
         Gizmos.color = Color.blue;
         Gizmos.DrawWireSphere(transform.position, chaseDistance);
      }
   }
}
