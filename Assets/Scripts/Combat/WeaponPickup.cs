using RPG.Attributes;
using RPG.Control;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
   public class WeaponPickup : MonoBehaviour, IRaycastable
   {
      [SerializeField] WeaponConfig weapon = null;
      [SerializeField] float healthToRestore = 0; // 12-205 hack until we implement real inventory
      [SerializeField] float respawnTime = 5;

      private void OnTriggerEnter(Collider other)
      {
         if (other.gameObject.CompareTag("Player"))
         {
            Pickup(other.gameObject);
         }
      }

      private void Pickup(GameObject subject)
      {
         // equip the weapon if it's not null
         if (weapon != null)
         {
            subject.GetComponent<Fighter>().EquipWeapon(weapon);
         }
         // apply health boost if not zero
         if (healthToRestore > 0)
         {
            subject.GetComponent<Health>().Heal(healthToRestore);
         }
         
         StartCoroutine(HideForSeconds(respawnTime));
      }

      private IEnumerator HideForSeconds(float seconds)
      {
         ShowPickup(false);
         yield return new WaitForSeconds(seconds);
         ShowPickup(true);
      }

      private void ShowPickup(bool shouldShow)
      {
         // disable collider
         GetComponent<Collider>().enabled = shouldShow;

         // disable game objects of all my transform's children
         foreach (Transform child in transform)
         {
            child.gameObject.SetActive(shouldShow);
         }
      }

      // implement IRaycastable interface________________________________________
      public CursorType GetCursorType()
      {
         return CursorType.Pickup;
      }

      public bool HandleRaycast(PlayerController callingController)
      {
         if (Input.GetMouseButtonDown(0))
         {
            Pickup(callingController.gameObject);
         }
         return true;
      }
   }
}
