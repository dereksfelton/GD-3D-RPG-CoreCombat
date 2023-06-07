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

      private Transform target;
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

         if (target == null) return;

         if (!TargetIsInRange())
         {
            mover.MoveTo(target.position);
         }
         else
         {
            mover.Cancel();
            AttackBehavior();
         }
      }

      private void AttackBehavior()
      {
         // throttle attacks
         if (timeSinceLastAttack >= timeBetweenAttacks)
         {
            GetComponent<Animator>().SetTrigger("attack");
            timeSinceLastAttack = 0f;

            Health targetHealth = target.GetComponent<Health>();
            targetHealth.TakeDamage(weaponDamage);
         }
      }

      private bool TargetIsInRange()
      {
         return Vector3.Distance(transform.position, target.position) <= weaponRange;
      }

      public void Attack(CombatTarget combatTarget)
      {
         GetComponent<ActionScheduler>().StartAction(this);
         target = combatTarget.transform;
      }

      public void Cancel()
      {
         target = null;
      }

      // Animation event ... called from animation
      void Hit()
      {

      }
   }
}
