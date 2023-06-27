using System;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
   public class HealthDisplay : MonoBehaviour
   {
      Health health;

      private void Awake()
      {
         health = GameObject.FindWithTag("Player").GetComponent<Health>();
      }

      private void Update()
      {
         GetComponent<TMP_Text>().text = String.Format("{0:0}%", health.GetPercentage());
      }
   }
}
