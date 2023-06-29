using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
   [RequireComponent(typeof(Health))]
   public class CombatTarget : MonoBehaviour, IRaycastable
   {
      // implement IRaycastable interface________________________________________   
      public CursorType GetCursorType()
      {
         return CursorType.Combat;
      }

      public bool HandleRaycast(PlayerController callingController)
      {
         Fighter fighter = callingController.GetComponent<Fighter>();

         if (!fighter.CanAttack(gameObject)) return false;
         
         if (Input.GetMouseButtonDown(0))
         {
            fighter.Attack(gameObject);
         }
         return true;
      }
   }
}
