using RPG.Combat;
using RPG.Movement;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace RPG.Control
{
   public class PlayerController : MonoBehaviour
   {
      void Update()
      {
         if (InteractWithCombat()) return;
         if (InteractWithMovement()) return;
         print("Nothing to do...");
      }

      private bool InteractWithCombat()
      {
         RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
         foreach (RaycastHit hit in hits)
         {
            // see if this hit is a CombatTarget
            CombatTarget target = hit.transform.GetComponent<CombatTarget>();
            if (target == null) continue;

            // if so, and if we clicked the left mouse button...
            if (Input.GetMouseButtonDown(0))
            {
               GetComponent<Fighter>().Attack(target);
            }

            // return that we DID interact with combat,
            // even if we just hovered over a combat target
            return true;
         }
         return false;
      }

      private bool InteractWithMovement()
      {
         RaycastHit hit;
         bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

         if (hasHit)
         {
            if (Input.GetMouseButton(0))
            {
               GetComponent<Mover>().MoveTo(hit.point);
            }
            // return that we DID interact with movement,
            // even if we just hovered over a movement target
            return true;
         }
         return false;
      }

      private static Ray GetMouseRay()
      {
         return Camera.main.ScreenPointToRay(Input.mousePosition);
      }
   }
}