using Packages.Rider.Editor.UnitTesting;
using UnityEngine;

namespace RPG.Combat
{
   public class Projectile : MonoBehaviour
   {
      [SerializeField] Transform target = null;
      [SerializeField] float speed = 1f;

      void Update()
      {
         if (target == null) return;

         // point projectile at target
         transform.LookAt(GetAimLocation());
         
         // move the arrow toward the target by the right amount per frame
         transform.Translate(Vector3.forward * speed * Time.deltaTime);
      }

      private Vector3 GetAimLocation()
      {
         CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
         if (targetCapsule == null) return target.position;
         return target.position + Vector3.up * targetCapsule.height / 2;
      }
   }
}
