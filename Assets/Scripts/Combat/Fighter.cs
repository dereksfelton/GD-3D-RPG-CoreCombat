using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
   public class Fighter : MonoBehaviour, IAction
   {
      [SerializeField] float weaponRange;
      [SerializeField] float timeBetweenAttacks;
      [SerializeField] float weaponDamage = 5f;

      private Health target;
      private Mover mover;
      private float timeSinceLastAttack = 0f;   

      private void Start()
      {
         mover = GetComponent<Mover>();
      }

      private void Update()
      {
         // increment time since last attack
         timeSinceLastAttack += Time.deltaTime;

         if (target == null || target.IsDead) return;

         if (!TargetIsInRange())
         {
            mover.MoveTo(target.transform.position);
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
            // this triggers the Hit() event
            GetComponent<Animator>().SetTrigger("attack");
            timeSinceLastAttack = 0f;
         }
      }

      // Animation event ... called from animation ... this is when the punch "lands"
      void Hit()
      {
         target.TakeDamage(weaponDamage);
      }

      private bool TargetIsInRange()
      {
         return Vector3.Distance(transform.position, target.transform.position) <= weaponRange;
      }

      public bool CanAttack(CombatTarget combatTarget)
      {
         if (combatTarget == null) return false;
         
         Health targetToTest = combatTarget.GetComponent<Health>();
         return targetToTest != null && !targetToTest.IsDead;
      }

      public void Attack(CombatTarget combatTarget)
      {
         GetComponent<ActionScheduler>().StartAction(this);
         target = combatTarget.GetComponent<Health>();
      }

      public void Cancel()
      {
         GetComponent<Animator>().SetTrigger("stopAttack");
         target = null;
      }
   }
}
