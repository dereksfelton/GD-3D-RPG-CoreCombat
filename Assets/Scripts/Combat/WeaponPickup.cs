using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
   public class WeaponPickup : MonoBehaviour
   {
      [SerializeField] Weapon weapon = null;
      [SerializeField] float respawnTime = 5;

      private void OnTriggerEnter(Collider other)
      {
         if (other.gameObject.CompareTag("Player"))
         {
            other.GetComponent<Fighter>().EquipWeapon(weapon);
            StartCoroutine(HideForSeconds(respawnTime));
         }
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
   }
}
