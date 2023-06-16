using RPG.Core;
using RPG.Saving;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace RPG.Movement
{
   public class Mover : MonoBehaviour, IAction, ISaveable
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
         public SerializableVector3 position;
         public SerializableVector3 rotation;
      }
      public object CaptureState()
      {
         // capture multiple values in the form of a dictionary
         // Dictionary<string, object> data = new Dictionary<string, object>();
         // data["position"] = new SerializableVector3(transform.position);
         // data["rotation"] = new SerializableVector3(transform.eulerAngles);

         // capture multiple values in the form of a struct
         MoverSaveData data = new MoverSaveData();
         data.position = new SerializableVector3(transform.position);
         data.rotation = new SerializableVector3(transform.eulerAngles);

         return data;
      }

      public void RestoreState(object state)
      {
         // read in multiple values in the form of a dictionary
         //Dictionary<string, object> data = (Dictionary<string, object>)state;

         // read in multiple values in the form of a struct
         MoverSaveData data = (MoverSaveData)state;

         GetComponent<NavMeshAgent>().enabled = false;
         //transform.position = ((SerializableVector3)data["position"]).ToVector();
         //transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
         transform.position = data.position.ToVector();
         transform.eulerAngles = data.rotation.ToVector();

         GetComponent<NavMeshAgent>().enabled = true;
      }
   }
}