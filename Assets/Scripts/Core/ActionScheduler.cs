using UnityEngine;

namespace RPG.Core
{
   public class ActionScheduler : MonoBehaviour
   {
      MonoBehaviour currentAction;

      public void StartAction(MonoBehaviour action)
      {
         // don't do anything here if we're still doing the same thing
         if (currentAction == action) return;

         if (currentAction != null)
         {
            print("Canceling " + currentAction);
         }
         currentAction = action;
      }
   }   
}
