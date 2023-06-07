using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
   public class Mover : MonoBehaviour
   {
      [SerializeField] Transform target;

      NavMeshAgent navMeshAgent;

      private void Start()
      {
         navMeshAgent = GetComponent<NavMeshAgent>();
      }

      void Update()
      {
         UpdateAnimator();
      }

      // make this public because we want it to be called from outside
      public void MoveTo(Vector3 destination)
      {
         navMeshAgent.destination = destination;
         navMeshAgent.isStopped = false;
      }

      public void Stop()
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
   }
}