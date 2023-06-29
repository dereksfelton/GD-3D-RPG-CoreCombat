using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
         Combat,
         UI
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
         if (InteractWithUI()) return;
         if (health.IsDead)
         {
            SetCursor(CursorType.None);
            return;
         }
         if (InteractWithComponent()) return; // raycastables
         //if (InteractWithCombat()) return;
         if (InteractWithMovement()) return;

         SetCursor(CursorType.None);
      }

      private bool InteractWithComponent()
      {
         // raycast through world, get all hits
         RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
         
         // cycle through hit game objects
         foreach (RaycastHit hit in hits)
         {
            // get all IRaycastable components in this game object
            IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();

            // cycle through raycastable components
            foreach(IRaycastable raycastable in raycastables)
            {
               if (raycastable.HandleRaycast(this)) 
               {
                  SetCursor(CursorType.Combat); // just for now to show that we handled this raycast
                  return true;
               }
            }
         }
         // if nothing was hit, or if nothing handled the raycast, return false
         return false;
      }

      private bool InteractWithUI()
      {
         // returns t/f based on whether we're hovering over UI
         bool overUI = EventSystem.current.IsPointerOverGameObject();
         if (overUI)
         {
            SetCursor(CursorType.UI);
         }
         return overUI;
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