using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
   public class Mover : MonoBehaviour, IAction, IJsonSaveable
   {
      [SerializeField] float maxSpeed = 6f;
      [SerializeField] float maxPathLength = 40f;

      Transform target;
      NavMeshAgent navMeshAgent;
      Health health;

      private void Awake()
      {
         navMeshAgent = GetComponent<NavMeshAgent>();
         health = GetComponent<Health>();
      }

      void Update()
      {
         // disable the nav mesh agent if I'm dead
         navMeshAgent.enabled = !health.IsDead;

         UpdateAnimator();
      }

      // only called from non-combat movement
      public void StartMoveAction(Vector3 destination, float speedFraction)
      {
         GetComponent<ActionScheduler>().StartAction(this);
         MoveTo(destination, speedFraction);
      }

      // returns whether a point is reachable (physically, or just distance-wise) from my position
      public bool CanMoveTo(Vector3 destination)
      {
         // calculate a path to target
         NavMeshPath path = new NavMeshPath();
         bool foundPathToTarget = NavMesh.CalculatePath(transform.position,
                                                        destination,
                                                        NavMesh.AllAreas,
                                                        path);
         // if no path was found, return false
         if (!foundPathToTarget) return false;

         // if path to target isn't complete, return false
         if (path.status != NavMeshPathStatus.PathComplete) return false;

         // if complete path to target is too long, return false
         if (PathIsTooLong(path)) return false;

         return true;
      }

      // make this public because we want it to be called from outside
      public void MoveTo(Vector3 destination, float speedFraction)
      {
         navMeshAgent.destination = destination;
         navMeshAgent.speed = 
            maxSpeed * Mathf.Clamp01(speedFraction); // ensure value is between 0 and 1
         navMeshAgent.isStopped = false;
      }

      public void Cancel()
      {
         navMeshAgent.isStopped = true;
      }

      private void UpdateAnimator()
      {
         Vector3 velocity = navMeshAgent.velocity;

         // convert to local value relative to character...
         // lets you convert global values into local values the animator needs to know
         Vector3 localVelocity = transform.InverseTransformDirection(velocity);

         // find how fast I should be moving in a forward direction
         float speed = localVelocity.z;

         // set animator's forwardSpeed value to the speed we calculated...
         // this effectively does what sliding the blend graph to that value would do
         GetComponent<Animator>().SetFloat("forwardSpeed", speed);
      }

      private bool PathIsTooLong(NavMeshPath path)
      {
         // start calculating path length from my current position
         Vector3 currentWaypoint = transform.position;
         float length = 0;

         // cycle through path corners
         foreach (Vector3 nextCorner in path.corners)
         {
            // add the distance from my current position to the next corner
            length += Vector3.Distance(currentWaypoint, nextCorner);

            // if length is longer than the max path length, don't bother caclculating further
            if (length > maxPathLength) return true;

            // otherwise, update my current waypoint to the corner
            currentWaypoint = nextCorner;
         }
         // if I've made it this far, the path is NOT too long
         return false;
      }

      // implement ISaveable interface_________________________________________________
      [System.Serializable]
      struct MoverSaveData
      {
         public JToken position;
         public JToken rotation;
      }

      public JToken CaptureAsJToken()
      {
         MoverSaveData data = new MoverSaveData();
         data.position = transform.position.ToToken();
         data.rotation = transform.eulerAngles.ToToken();
         return JToken.FromObject(data);
      }

      public void RestoreFromJToken(JToken state)
      {
         print("Restoring Mover data");

         MoverSaveData data = state.ToObject<MoverSaveData>();

         navMeshAgent.enabled = false;
         transform.position = data.position.ToVector3();
         transform.eulerAngles = data.rotation.ToVector3();
         navMeshAgent.enabled = true;
      }
   }
}