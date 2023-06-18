using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
   public class Projectile : MonoBehaviour
   {
      [SerializeField] float speed = 1f;
      Health target = null;
      float damage = 0;

      void Update()
      {
         if (target == null) return;

         // point projectile at target
         transform.LookAt(GetAimLocation());
         
         // move the arrow toward the target by the right amount per frame
         transform.Translate(Vector3.forward * speed * Time.deltaTime);
      }

      private void OnTriggerEnter(Collider other)
      {
         // if we hit something other than the target, just return (for now)
         if (other.GetComponent<Health>() != target) return;
         
         // otherwise, apply my damage to target
         target.TakeDamage(damage);

         // finally, destroy my (the projectile's) game object
         Destroy(gameObject);
      }

      public void SetTarget(Health target, float damage)
      {
         this.target = target;
         this.damage = damage;
      }

      private Vector3 GetAimLocation()
      {
         CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
         if (targetCapsule == null) return target.transform.position;
         return target.transform.position + Vector3.up * targetCapsule.height / 2;
      }
   }
}
