using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
   [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
   public class WeaponConfig : ScriptableObject
   {
      [SerializeField] Weapon equippedPrefab = null;
      [SerializeField] AnimatorOverrideController animatorOverride = null;

      [SerializeField] float weaponRange = 0f;
      [SerializeField] float weaponDamage = 0f;
      [SerializeField] float percentageBonus = 0f;
      [SerializeField] bool isRightHanded = true;

      [SerializeField] Projectile projectile = null;

      public float Range { get { return weaponRange; } }
      public float Damage { get { return weaponDamage; } }
      public float PercentageBonus { get { return percentageBonus; } }
      public bool HasProjectile { get { return projectile != null; } }

      const string weaponName = "Weapon";

      public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
      {
         DestroyOldWeapon(rightHand, leftHand);

         Weapon weapon = null;

         if (equippedPrefab != null)
         {
            Transform handTransform = GetTransform(rightHand, leftHand);
            weapon = Instantiate(equippedPrefab, handTransform);
            weapon.gameObject.name = weaponName;
         }

         // cast will return null if it fails
         var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

         if (animatorOverride != null)
         {
            animator.runtimeAnimatorController = animatorOverride;
         }
         else if (overrideController != null)
         {
            animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
         }

         return weapon;
      }

      private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
      {
         Transform oldWeapon = rightHand.Find(weaponName);

         // if old weapon isn't in right hand...
         if (oldWeapon == null)
         {
            // ...check the left hand
            oldWeapon = leftHand.Find(weaponName);
         }

         // if old weapon wasn't in left hand either, simply return ... nothing to destroy
         if (oldWeapon == null) return;

         // otherwise, destroy the old weapon
         oldWeapon.name = "DESTROYING"; // prevent confusion with new weapon picked up
         Destroy(oldWeapon.gameObject);
      }

      public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
      {
         Projectile projectileInstance =
            Instantiate(
               projectile,                                  // what we're instantiating
               GetTransform(rightHand, leftHand).position,  // where we're instantiating it
               Quaternion.identity                          // in what direction we're instantiating it
            );

         // set where it's going
         projectileInstance.SetTarget(target, instigator, calculatedDamage);
      }

      private Transform GetTransform(Transform rightHand, Transform leftHand)
      {
         return isRightHanded ? rightHand : leftHand;
      }
   }
}
