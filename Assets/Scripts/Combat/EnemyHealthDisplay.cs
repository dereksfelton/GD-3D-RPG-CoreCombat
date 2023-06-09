﻿using RPG.Attributes;
using System;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
   public class EnemyHealthDisplay : MonoBehaviour
   {
      Fighter fighter;

      private void Awake()
      {
         fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
      }

      private void Update()
      {
         if (fighter.GetTarget() == null)
         {
            GetComponent<TMP_Text>().text = "N/A";
            return;
         }
         Health health = fighter.GetTarget();
         GetComponent<TMP_Text>().text = string.Format("{0:0} / {1:0}", health.HP, health.MaxHP);
      }
   }
}
