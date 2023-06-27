using UnityEngine;

namespace RPG.Stats
{
   public class BaseStats : MonoBehaviour
   {
      [Range(1, 99)]
      [SerializeField] int startingLevel;
      [SerializeField] CharacterClass characterClass;
      [SerializeField] Progression progression;

      int currentLevel = 0;

      private void Start()
      {
         currentLevel = CalculateLevel();
         Experience experience = GetComponent<Experience>();
         if (experience != null)
         {
            // subscribe to the onExpereinceGained action
            experience.onExperienceGained += UpdateLevel;
         }
      }

      private void UpdateLevel()
      {
         int newLevel = CalculateLevel();
         if (newLevel > currentLevel)
         {
            currentLevel = newLevel;
            print("Levelled Up!");
         }
      }

      public float GetStat(Stat stat)
      {
         return progression.GetStat(stat, characterClass, GetLevel());
      }

      public int GetLevel()
      {
         if (currentLevel < 1)
         {
            currentLevel = CalculateLevel();
         }
         return currentLevel;
      }

      public int CalculateLevel()
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
   }
}