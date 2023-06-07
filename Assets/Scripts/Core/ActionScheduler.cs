using UnityEngine;

namespace RPG.Core
{
   public class ActionScheduler : MonoBehaviour
   {
      IAction currentAction;

      public void StartAction(IAction action)
      {
         // don't do anything here if we're still doing the same thing
         if (currentAction == action) return;

         if (currentAction != null)
         {
            currentAction.Cancel();
         }
         currentAction = action;
      }
   }   
}
