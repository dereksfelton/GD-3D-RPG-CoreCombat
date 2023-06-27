using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
   [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
   public class Progression : ScriptableObject {
      [SerializeField] ProgressionCharacterClass[] characterClasses = null;

      Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

      public float GetStat(Stat stat, CharacterClass characterClass, int level)
      {
         BuildLookup();

         float[] levels = lookupTable[characterClass][stat];
         return (levels.Length >= level) ? levels[level - 1] : 0;
      }

      private void BuildLookup()
      {
         // if we've already built it, return
         if (lookupTable != null) return;

         lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

         // loop through all progression character classes
         foreach (ProgressionCharacterClass progClass in characterClasses)
         {
            var statLookupTable = new Dictionary<Stat, float[]>();

            // loop through all progression stats
            foreach (ProgressionStat progStat in progClass.stats)
            {
               statLookupTable[progStat.stat] = progStat.levels;
            }

            lookupTable[progClass.characterClass] = statLookupTable;
         }
      }
   }

   [System.Serializable]
   class ProgressionCharacterClass
   {
      public CharacterClass characterClass;
      public ProgressionStat[] stats;
   }

   [System.Serializable]
   class ProgressionStat
   {
      public Stat stat;
      public float[] levels;
   }
}