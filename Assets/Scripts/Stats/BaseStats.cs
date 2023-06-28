using GameDevTV.Utils;
using System;
using UnityEngine;

namespace RPG.Stats
{
   public class BaseStats : MonoBehaviour
   {
      [Range(1, 99)]
      [SerializeField] int startingLevel;
      [SerializeField] CharacterClass characterClass;
      [SerializeField] Progression progression = null;
      [SerializeField] GameObject levelUpParticleEffect = null;
      [SerializeField] bool shouldUseModifiers = false;

      public int Level
      {
         get { return currentLevel.value; }
         set { currentLevel.value = value; }
      }

      public event Action onLevelUp;

      LazyValue<int> currentLevel;

      Experience experience;

      private void Awake()
      {
         experience = GetComponent<Experience>();
         currentLevel = new LazyValue<int>(CalculateLevel);
      }

      private void Start()
      {
         currentLevel.ForceInit();
      }

      private void OnEnable()
      {
         if (experience != null)
         {
            experience.onExperienceGained += UpdateLevel;
         }
      }

      private void OnDisable()
      {
         if (experience != null)
         {
            experience.onExperienceGained -= UpdateLevel;
         }
      }

      public float GetStat(Stat stat)
      {
         return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
      }

      private float GetBaseStat(Stat stat)
      {
         return progression.GetStat(stat, characterClass, Level);
      }

      private float GetAdditiveModifier(Stat stat)
      {
         if (!shouldUseModifiers) return 0;

         float total = 0;

         // get all modifier providers
         var providers = GetComponents<IModifierProvider>();
         
         // loop over every provider
         foreach (var provider in providers)
         {
            foreach (float modifier in provider.GetAdditiveModifiers(stat))
            {
               total += modifier;
            }
         }
         return total;
      }

      private float GetPercentageModifier(Stat stat)
      {
         if (!shouldUseModifiers) return 0;

         float multiplier = 0;

         // get all modifier providers
         var providers = GetComponents<IModifierProvider>();

         // loop over every provider
         foreach (var provider in providers)
         {
            foreach (float modifier in provider.GetPercentageModifiers(stat))
            {
               multiplier += modifier;
            }
         }
         return multiplier;
      }

      private void UpdateLevel()
      {
         int newLevel = CalculateLevel();
         if (newLevel > Level)
         {
            Level = newLevel;
            LevelUpEffect();
            onLevelUp();
         }
      }

      private int CalculateLevel()
      {
         // get my experience component
         Experience experience = GetComponent<Experience>();

         // only proceed if this is non-null; otherwise simply return starting level
         if (experience == null) return startingLevel;

         // get current XP
         float currentXp = experience.XP;

         // find how many levels were defined for this stat and character
         int penultimateLevel = progression.GetLevelCount(Stat.ExperienceToLevelUp, characterClass);

         // loop over all levels available for this stat (ExperienceToLevelUp) and character
         for (int level = 1; level <= penultimateLevel; level++)
         {
            // find XP to level up to next level
            float XpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);

            // if this value is greater than current XP, this is the level we're now at
            if (XpToLevelUp > currentXp)
            {
               return level;
            }
         }

         // if we've run out of "levels to level up" values, we've hit the ULTIMATE level
         return penultimateLevel + 1;
      }

      private void LevelUpEffect()
      {
         Instantiate(levelUpParticleEffect, transform);
      }
   }
}