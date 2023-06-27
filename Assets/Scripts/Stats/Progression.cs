using UnityEngine;

namespace RPG.Stats
{
   [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
   public class Progression : ScriptableObject {
      [SerializeField] ProgressionCharacterClass[] characterClasses = null;

      public float GetStat(Stat stat, CharacterClass characterClass, int level)
      {
         // loop through character classes to find passed one
         foreach (ProgressionCharacterClass progClass in characterClasses)
         {
            if (progClass.characterClass != characterClass) continue;

            // loop through this character class's stats to find passed one
            foreach (ProgressionStat progressionStat in progClass.stats)
            {
               if (progressionStat.stat != stat) continue;

               // guard against trying to access a stat by a level that doesn't exist
               if (progressionStat.levels.Length < level) continue;

               return progressionStat.levels[level -1];
            }
         }
         return 0;
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