using UnityEngine;

namespace RPG.Control
{
   public class PatrolPath : MonoBehaviour
   {
      const float waypointGizmoRadius = 0.3f;

      private void OnDrawGizmos()
      {
         Gizmos.color = Color.red;

         // loop through my (the PatrolPath's) children ... the waypoints
         // draw a gizmo for each
         for (int i = 0; i < transform.childCount; i++)
         {
            int j = GetNextWaypointIndex(i);
            Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
            Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
         }
      }

      private Vector3 GetWaypoint(int i)
      {
         return transform.GetChild(i).position;
      }

      private int GetNextWaypointIndex(int i)
      {
         return (i + 1 == transform.childCount) ? 0 : i + 1;
      }
   }
}