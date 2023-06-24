﻿using RPG.Core;
using System;
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

      const string weaponName = "Weapon";

      public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
      {
         DestroyOldWeapon(rightHand, leftHand);

         if (equippedPrefab != null)
         {
            Transform handTransform = GetTransform(rightHand, leftHand);
            GameObject weapon = Instantiate(equippedPrefab, handTransform);
            weapon.name = weaponName;
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
