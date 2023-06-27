using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Stats
{
   public class BaseStats : MonoBehaviour
   {
      [Range(1, 99)]
      [SerializeField] int startingLevel = 1;
      [SerializeField] CharacterClass characterClass;
      [SerializeField] Progression progression;

      public void Update()
      {
         if (gameObject.CompareTag("Player"))
         {
            print("Level: " + GetLevel());
         }
      }

      public float GetStat(Stat stat)
      {
         return progression.GetStat(stat, characterClass, GetLevel());
      }

      public int GetLevel()
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