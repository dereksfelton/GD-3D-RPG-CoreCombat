using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats; // NOTE: we'll want to remove this dependency
using System;
using UnityEngine;

namespace RPG.Attributes
{
   public class Health : MonoBehaviour, IJsonSaveable
   {
      [SerializeField] float regnerationPercentage = 70;
      public bool IsDead { get; private set; }

      private BaseStats baseStats = null;
      private float healthPoints = -1f;


      private void Awake()
      {
         baseStats = GetComponent<BaseStats>();
         IsDead = false;
      }

      private void Start()
      {
         // subscribe to the onLevelUp event
         baseStats.onLevelUp += RegenerateHealth;

         // only set health points here if they haven't been restored yet
         if (healthPoints < 0)
         {
            healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
         }
      }

      public void TakeDamage(GameObject instigator, float damage)
      {
         healthPoints = Mathf.Max(0, healthPoints - damage);
         
         if (healthPoints == 0)
         {
            Die();
            AwardExperience(instigator);
         }
      }

      // return what percent of my max possible health is for my level and class
      public float GetPercentage()
      {
         return 100 * healthPoints / GetComponent<BaseStats>().GetStat(Stat.Health);
      }

      private void Die()
      {
         if (IsDead) return;

         //  trigger the death animation
         IsDead = true;
         GetComponent<Animator>().SetTrigger("die");

         // cancel currently running actions
         GetComponent<ActionScheduler>().CancelCurrentAction();
      }

      private void AwardExperience(GameObject instigator)
      {
         Experience instigatorExperience = instigator.GetComponent<Experience>();
         if (instigatorExperience == null) return;

         instigatorExperience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
      }

      private void RegenerateHealth()
      {
         // regenerate to the HIGHER of regen percentage at our current level's max HP, or our current HP
         // i.e., don't penalize if they're already above this percentager of the new level's HP
         float regenHealthPoints = baseStats.GetStat(Stat.Health) * (regnerationPercentage / 100);
         healthPoints = Mathf.Max(healthPoints, regenHealthPoints);
      }

      // implement IJsonSaveable interface_________________________________________________
      public JToken CaptureAsJToken()
      {
         return JToken.FromObject(healthPoints);
      }

      public void RestoreFromJToken(JToken state)
      {
         healthPoints = state.ToObject<float>();

         if (healthPoints <= 0)
         {
            Die();
         }
      }
   }
}
