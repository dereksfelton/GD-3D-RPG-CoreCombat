using RPG.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Combat
{
   [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
   public class Weapon : ScriptableObject
   {
      [SerializeField] GameObject equippedPrefab = null;
      [SerializeField] AnimatorOverrideController animatorOverride = null;

      [SerializeField] float weaponRange = 0f;
      [SerializeField] float weaponDamage = 0f;
      [SerializeField] bool isRightHanded = true;

      [SerializeField] Projectile projectile = null;

      public float Range { get { return weaponRange; } }
      public float Damage { get { return weaponDamage; } }
      public bool HasProjectile { get { return projectile != null; } }


      public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
      {
         if (equippedPrefab != null)
         {
            Instantiate(equippedPrefab, GetTransform(rightHand, leftHand));
         }
         if (animatorOverride != null)
         {
            animator.runtimeAnimatorController = animatorOverride;
         }
      }

      public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
      {
         Projectile projectileInstance =
            Instantiate(
               projectile,                                  // what we're instantiating
               GetTransform(rightHand, leftHand).position,  // where we're instantiating it
               Quaternion.identity                          // in what direction we're instantiating it
            );

         // set where it's going
         projectileInstance.SetTarget(target, Damage);
      }

      private Transform GetTransform(Transform rightHand, Transform leftHand)
      {
         return isRightHanded ? rightHand : leftHand;
      }
   }
}
