using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
   public class PlayerController : MonoBehaviour
   {
      Health health;

      private void Start() {
         health = GetComponent<Health>();
      }

      void Update()
      {
         if (health.IsDead) return; // don't do ANYTHING if you're dead!
         
         if (InteractWithCombat()) return;
         if (InteractWithMovement()) return;
      }

      private bool InteractWithCombat()
      {
         RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
         foreach (RaycastHit hit in hits)
         {
            // see if this hit is a CombatTarget that isn't dead
            CombatTarget target = hit.transform.GetComponent<CombatTarget>();
            if (target == null) continue;

            if (!GetComponent<Fighter>().CanAttack(target.gameObject)) continue;

            // if so, and if we clicked the left mouse button...
            if (Input.GetMouseButtonDown(0))
            {
               GetComponent<Fighter>().Attack(target.gameObject);
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
               GetComponent<Mover>().StartMoveAction(hit.point, 1f);
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