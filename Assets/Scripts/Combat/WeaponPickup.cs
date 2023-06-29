using RPG.Control;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
   public class WeaponPickup : MonoBehaviour, IRaycastable
   {
      [SerializeField] Weapon weapon = null;
      [SerializeField] float respawnTime = 5;

      private void OnTriggerEnter(Collider other)
      {
         if (other.gameObject.CompareTag("Player"))
         {
            Pickup(other.GetComponent<Fighter>());
         }
      }

      private void Pickup(Fighter fighter)
      {
         fighter.EquipWeapon(weapon);
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
            Pickup(callingController.GetComponent<Fighter>());
         }
         return true;
      }
   }
}
