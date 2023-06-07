using RPG.Movement;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Combat
{
   public class Fighter : MonoBehaviour
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
         if (target != null && Vector3.Distance(transform.position, target.position) > weaponRange)
         {
            mover.MoveTo(target.position);
         }
         else
         {
            mover.Stop();
         }
      }

      public void Attack(CombatTarget combatTarget)
      {
         target = combatTarget.transform;
      }
   }
}
