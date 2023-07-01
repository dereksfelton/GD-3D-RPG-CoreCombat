using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
   public class Fighter : MonoBehaviour, IAction, IJsonSaveable, IModifierProvider
   {
      [SerializeField] float timeBetweenAttacks = 1f;
      [SerializeField] Transform rightHandTransform = null;
      [SerializeField] Transform leftHandTransform = null;
      [SerializeField] WeaponConfig defaultWeapon = null;

      private Weapon CurrentWeapon {
         get { return currentWeapon.value; }
         set { currentWeapon.value = value; }
      }

      // cached references
      Mover mover;
      BaseStats baseStats;

      Health target;
      float timeSinceLastAttack = Mathf.Infinity;
      
      WeaponConfig currentWeaponConfig;
      LazyValue<Weapon> currentWeapon;

      private void Awake()
      {
         mover = GetComponent<Mover>();
         baseStats = GetComponent<BaseStats>();

         currentWeaponConfig = defaultWeapon;
         currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
      }

      private Weapon SetupDefaultWeapon()
      {
         return AttachWeapon(defaultWeapon);
      }

      private void Start()
      {
         currentWeapon.ForceInit();
      }

      private void Update()
      {
         // increment time since last attack
         timeSinceLastAttack += Time.deltaTime;

         if (target == null || target.IsDead) return;

         if (!TargetIsInRange())
         {
            // 0.9f for speedFraction, because we want chasing to be at almost full speed
            mover.MoveTo(target.transform.position, 0.9f);
         }
         else
         {
            mover.Cancel();
            AttackBehavior();
         }
      }

      private void AttackBehavior()
      {
         // make sure we're facing the target
         transform.LookAt(target.transform);
         
         // throttle attacks
         if (timeSinceLastAttack >= timeBetweenAttacks)
         {
            TriggerAttack();
         }
      }

      private void TriggerAttack()
      {
         // this triggers the Hit() event
         GetComponent<Animator>().ResetTrigger("stopAttack");
         GetComponent<Animator>().SetTrigger("attack");
         timeSinceLastAttack = 0f;
      }

      // Animation event...
      // this method is called from animation in exactly the frame when the punch "lands"
      void Hit()
      {
         HandleAttack();
      }

      // Animation event...
      // this method is called from animation in exactly the frame when an arrow is shot
      void Shoot()
      {
         HandleAttack();
      }

      private void HandleAttack()
      {
         if (target == null) return;

         float damage = baseStats.GetStat(Stat.Damage);

         // check to see if my current weapon isn't null
         if (CurrentWeapon != null)
         {
            CurrentWeapon.OnHit(); // if not, call its OnHit method
         }

         if (currentWeaponConfig.HasProjectile)
         {
            currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
         }
         else
         {
            target.ApplyDamage(gameObject, damage);
         }
      }

      private bool TargetIsInRange()
      {
         return Vector3.Distance(transform.position, target.transform.position) <= currentWeaponConfig.Range;
      }

      public bool CanAttack(GameObject combatTarget)
      {
         if (combatTarget == null) return false;

         // if we can't get to the target, return false
         if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position)) return false;

         Health targetToTest = combatTarget.GetComponent<Health>();
         return targetToTest != null && !targetToTest.IsDead;
      }

      public void Attack(GameObject combatTarget)
      {
         GetComponent<ActionScheduler>().StartAction(this);
         target = combatTarget.GetComponent<Health>();
      }

      public void Cancel()
      {
         StopAttack();
         mover.Cancel();
      }

      private void StopAttack()
      {
         GetComponent<Animator>().ResetTrigger("attack");
         GetComponent<Animator>().SetTrigger("stopAttack");
         target = null;
      }

      public void EquipWeapon(WeaponConfig weaponConfig)
      {
         currentWeaponConfig = weaponConfig;
         CurrentWeapon = AttachWeapon(weaponConfig);
      }

      public Weapon AttachWeapon(WeaponConfig weaponConfig)
      {
         Animator animator = GetComponent<Animator>();
         return weaponConfig.Spawn(rightHandTransform, leftHandTransform, animator);
      }

      public Health GetTarget()
      {
         return target;
      }

      // Implement IJsonSaveable____________________________________________________________________
      public JToken CaptureAsJToken()
      {
         return JToken.FromObject(currentWeaponConfig.name);
      }

      public void RestoreFromJToken(JToken state)
      {
         print("Restoring weapon: " + state.ToObject<string>());
         WeaponConfig weapon = Resources.Load<WeaponConfig>(state.ToObject<string>());
         EquipWeapon(weapon);
      }

      // Implement IModifierProvider____________________________________________________________________
      public IEnumerable<float> GetAdditiveModifiers(Stat stat)
      {
         if (stat == Stat.Damage)
         {
            // note we could yield return more than one value here, such as damage from a off-handed
            // weapon, since our return type is IEnumerable
            yield return currentWeaponConfig.Damage;
         }
      }

      public IEnumerable<float> GetPercentageModifiers(Stat stat)
      {
         if (stat == Stat.Damage)
         {
            yield return currentWeaponConfig.PercentageBonus;
         }
      }
   }
}
