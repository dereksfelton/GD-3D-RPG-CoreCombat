using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
   public class Projectile : MonoBehaviour
   {
      [SerializeField] float speed = 1f;
      [SerializeField] bool isHoming = true;
      [SerializeField] GameObject hitEffect = null;

      Health target = null;
      float damage = 0;

      private void Start()
      {
         transform.LookAt(GetAimLocation());
      }

      void Update()
      {
         if (target == null) return;

         if (isHoming && !target.IsDead)
         {
            // point projectile at target
            transform.LookAt(GetAimLocation());
         }
         
         // move the arrow toward the target by the right amount per frame
         transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
      
      private void OnTriggerEnter(Collider other)
      {
         // if we hit something other than the target, just return (for now)
         if (other.GetComponent<Health>() != target) return;

         // if target is dead, just return
         if (target.IsDead) return;

         // otherwise, apply my damage to target
         target.TakeDamage(damage);

         // apply fireball impact effect
         if (hitEffect != null)
         {
            Instantiate(hitEffect, GetAimLocation(), transform.rotation);
         }

         // finally, destroy my (the projectile's) game object
         Destroy(gameObject);
      }
   }
}
