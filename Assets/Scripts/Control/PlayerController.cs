using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace RPG.Control
{
   public class PlayerController : MonoBehaviour
   {
      Health health;

      enum CursorType
      {
         None,
         Movement,
         Combat
      }

      [System.Serializable]
      struct CursorMapping
      {
         public CursorType type;
         public Texture2D texture;
         public Vector2 hotspot;
      }

      [SerializeField] CursorMapping[] cursorMappings = null;

      private void Awake() {
         health = GetComponent<Health>();
      }

      void Update()
      {
         if (health.IsDead) return; // don't do ANYTHING if you're dead!
         
         if (InteractWithCombat()) return;
         if (InteractWithMovement()) return;

         SetCursor(CursorType.None);
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

            // set cursor to custom Combat cursor
            SetCursor(CursorType.Combat);

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

            // set cursor to custom Movement cursor
            SetCursor(CursorType.Movement);

            // return that we DID interact with movement,
            // even if we just hovered over a movement target
            return true;
         }
         return false;
      }

      private void SetCursor(CursorType type)
      {
         CursorMapping mapping = GetCursorMapping(type);
         Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
      }

      private CursorMapping GetCursorMapping(CursorType type)
      {
         foreach (CursorMapping mapping in cursorMappings)
         {
            if (mapping.type == type)
            {
               return mapping;
            }
         }
         return cursorMappings[0]; // not ideal, but will work for now
      }

      private static Ray GetMouseRay()
      {
         return Camera.main.ScreenPointToRay(Input.mousePosition);
      }
   }
}