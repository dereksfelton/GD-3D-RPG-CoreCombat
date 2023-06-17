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

      public float Range { get { return weaponRange; } }
      public float Damage { get { return weaponDamage; } }


      public void Spawn(Transform handTransform, Animator animator)
      {
         if (equippedPrefab != null)
         {
            Instantiate(equippedPrefab, handTransform);
         }
         if (animatorOverride != null)
         {
            animator.runtimeAnimatorController = animatorOverride;
         }
      }
   }
}
