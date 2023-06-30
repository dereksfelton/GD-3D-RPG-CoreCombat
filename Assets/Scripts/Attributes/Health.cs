using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats; // NOTE: we'll want to remove this dependency
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
   public class Health : MonoBehaviour, IJsonSaveable
   {
      [SerializeField] float regnerationPercentage = 70;
      [SerializeField] UnityEvent<float> takeDamage;

      public float HP {
         get { return healthPoints.value; }
         set { healthPoints.value = value; }
      }
      public float MaxHP { get { return GetComponent<BaseStats>().GetStat(Stat.Health); } }
      public bool IsDead { get; private set; }

      LazyValue<float> healthPoints;

      private void Awake()
      {
         IsDead = false;

         // assign the delegate to be called when we fist need to access health points
         healthPoints = new LazyValue<float>(GetInitialHealth);
      }

      private float GetInitialHealth()
      {
         return GetComponent<BaseStats>().GetStat(Stat.Health);
      }

      private void Start()
      {
         healthPoints.ForceInit();
      }

      private void OnEnable()
      {
         GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
      }

      private void OnDisable()
      {
         GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
      }

      public void ApplyDamage(GameObject instigator, float damage)
      {
         HP = Mathf.Max(0, HP - damage);

         takeDamage.Invoke(damage);

         if (HP == 0 && !IsDead)
         {
            AwardExperience(instigator);
            Die();
         }
      }

      // return percent (0-100) of my max possible health is for my level and class
      public float GetPercentage1to100()
      {
         return 100 * GetPercentage0to1();
      }

      // return decimal (0-1) of my max possible health is for my level and class
      public float GetPercentage0to1()
      {
         return HP / GetComponent<BaseStats>().GetStat(Stat.Health);
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
         float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regnerationPercentage / 100);
         HP = Mathf.Max(HP, regenHealthPoints);
      }

      // implement IJsonSaveable interface_________________________________________________
      public JToken CaptureAsJToken()
      {
         return JToken.FromObject(HP);
      }

      public void RestoreFromJToken(JToken state)
      {
         HP = state.ToObject<float>();

         if (HP <= 0)
         {
            Die();
         }
      }
   }
}
