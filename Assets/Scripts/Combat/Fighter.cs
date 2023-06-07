using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
   public class Fighter : MonoBehaviour, IAction
   {
      [SerializeField] float weaponRange;
      private Transform target;
      private Mover mover;

      private void Start()
      {
         mover = GetComponent<Mover>();
      }

      private void Update()
      {
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
         GetComponent<Animator>().SetTrigger("attack");
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
