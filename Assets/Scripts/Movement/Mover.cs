using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.JsonSaving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
   public class Mover : MonoBehaviour, IAction, IJsonSaveable
   {
      [SerializeField] float maxSpeed = 6f;
      Transform target;
      NavMeshAgent navMeshAgent;
      Health health;

      private void Start()
      {
         navMeshAgent = GetComponent<NavMeshAgent>();
         health =  GetComponent<Health>();
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
         MoverSaveData data = state.ToObject<MoverSaveData>();

         GetComponent<NavMeshAgent>().enabled = false;
         transform.position = data.position.ToVector3();
         transform.eulerAngles = data.rotation.ToVector3();
         GetComponent<NavMeshAgent>().enabled = true;
      }
   }
}