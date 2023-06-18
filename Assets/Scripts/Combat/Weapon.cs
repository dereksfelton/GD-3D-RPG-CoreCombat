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

      public float Range { get { return weaponRange; } }
      public float Damage { get { return weaponDamage; } }


      public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
      {
         if (equippedPrefab != null)
         {
            Transform handTransform = isRightHanded ? rightHand : leftHand;
            Instantiate(equippedPrefab, handTransform);
         }
         if (animatorOverride != null)
         {
            animator.runtimeAnimatorController = animatorOverride;
         }
      }
   }
}
