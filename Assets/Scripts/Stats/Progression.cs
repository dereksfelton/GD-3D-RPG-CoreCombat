using UnityEngine;

namespace RPG.Stats
{
   [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
   public class Progression : ScriptableObject {
      [SerializeField] ProgressionCharacterClass[] characterClasses = null;

      [System.Serializable]
      class ProgressionCharacterClass
      {
         public CharacterClass characterClass;
         public float[] health;
      }

      public float GetHealth(CharacterClass characterClass, int level)
      {
         // not optimal technique, but it will do for now
         foreach (ProgressionCharacterClass progClass in characterClasses)
         {
            if (progClass.characterClass == characterClass)
            {
               return progClass.health[level -1];
            }
         }
         return 0;
      }
   }
}