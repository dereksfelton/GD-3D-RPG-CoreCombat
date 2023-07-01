using RPG.Attributes;
using RPG.Movement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
   public class PlayerController : MonoBehaviour
   {
      [System.Serializable]
      struct CursorMapping
      {
         public CursorType type;
         public Texture2D texture;
         public Vector2 hotspot;
      }

      [SerializeField] CursorMapping[] cursorMappings = null;
      [SerializeField] float maxNavMeshProjectionDistance = 1.0f;

      // cached references
      Health health;

      // cached cursor mapping optimization credit: Brandon Anderson,
      // https://community.gamedev.tv/t/cursor-flicker-and-fps-bug-fix/172245
      private CursorMapping _cachedCursorMapping; 

      private BufferedRaycast bufferedRaycaster;

      private void Awake() {
         health = GetComponent<Health>();
         bufferedRaycaster = new BufferedRaycast(10);
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
         if (InteractWithMovement()) return;

         SetCursor(CursorType.None);
      }

      private bool InteractWithComponent()
      {
         // FILTERED RAYCAST
         // raycast through world, get all hits sorted by distance
         IEnumerable<IRaycastable> raycastables = 
            bufferedRaycaster.FilteredRaycast<IRaycastable>(GetMouseRay(), true);

         // cycle through raycastable components
         foreach (IRaycastable raycastable in raycastables)
         {
            if (raycastable.HandleRaycast(this))
            {
               SetCursor(raycastable.GetCursorType());
               return true;
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
         Vector3 target;
         bool hasHit = RaycastNavMesh(out target);

         if (hasHit)
         {
            if (Input.GetMouseButton(0))
            {
               // if we can't get to the target, return false
               if (!GetComponent<Mover>().CanMoveTo(target)) return false;

               GetComponent<Mover>().StartMoveAction(target, 1f);
            }

            // set cursor to custom Movement cursor
            SetCursor(CursorType.Movement);

            // return that we DID interact with movement,
            // even if we just hovered over a movement target
            return true;
         }
         return false;
      }

      private bool RaycastNavMesh(out Vector3 target)
      {
         // init target to dummy in case of early exit
         target = Vector3.zero;

         // raycast to terrain
         RaycastHit hit;
         bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

         // if we didn't hit terrain, simply return false
         if (!hasHit) return false;

         // otherwise, find nearest navmesh point
         NavMeshHit navMeshHit;
         bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, 
                                                        out navMeshHit, 
                                                        maxNavMeshProjectionDistance, 
                                                        NavMesh.AllAreas);
         if (!hasCastToNavMesh) return false;

         // set target and return true if we found a nearby navmesh point
         target = navMeshHit.position;

         return true;
      }

      private void SetCursor(CursorType type)
      {
         if (_cachedCursorMapping.type == type) return;
         _cachedCursorMapping = GetCursorMapping(type);
         Cursor.SetCursor(_cachedCursorMapping.texture, _cachedCursorMapping.hotspot, CursorMode.Auto);
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