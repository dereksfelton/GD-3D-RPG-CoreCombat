using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
   public class Fighter : MonoBehaviour, IAction, IJsonSaveable
   {
      [SerializeField] float timeBetweenAttacks = 1f;
      [SerializeField] Transform rightHandTransform = null;
      [SerializeField] Transform leftHandTransform = null;
      [SerializeField] Weapon defaultWeapon = null;

      private Health target;
      private Mover mover;
      private BaseStats baseStats;
      private float timeSinceLastAttack = Mathf.Infinity;
      private Weapon currentWeapon = null;

      private void Start()
      {
         mover = GetComponent<Mover>();
         baseStats = GetComponent<BaseStats>();

         if (currentWeapon == null)
         {
            EquipWeapon(defaultWeapon);
         }
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

         if (currentWeapon.HasProjectile)
         {
            currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
         }
         else
         {
            target.TakeDamage(gameObject, damage);
         }
      }

      private bool TargetIsInRange()
      {
         return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.Range;
      }

      public bool CanAttack(GameObject combatTarget)
      {
         if (combatTarget == null) return false;
         
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

      public void EquipWeapon(Weapon weapon)
      {
         currentWeapon = weapon;
         weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
      }

      public Health GetTarget()
      {
         return target;
      }

      // Implement IJsonSaveable____________________________________________________________________
      public JToken CaptureAsJToken()
      {
         return JToken.FromObject(currentWeapon.name);
      }

      public void RestoreFromJToken(JToken state)
      {
         print("Restoring weapon: " + state.ToObject<string>());
         Weapon weapon = Resources.Load<Weapon>(state.ToObject<string>());
         EquipWeapon(weapon);
      }
   }
}
