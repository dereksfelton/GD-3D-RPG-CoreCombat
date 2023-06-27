using Newtonsoft.Json.Linq;
using RPG.Saving;
using System;
using UnityEngine;

namespace RPG.Stats
{
   public class Experience : MonoBehaviour, IJsonSaveable
   {
      [SerializeField] float experiencePoints = 0;

      public event Action onExperienceGained;

      public float XP { get { return experiencePoints; } }

      public void GainExperience(float experience)
      {
         experiencePoints += experience;
         onExperienceGained();
      }

      // Implement IJsonSaveable interface______________________
      public JToken CaptureAsJToken()
      {
         return JToken.FromObject(experiencePoints);
      }

      public void RestoreFromJToken(JToken state)
      {
         experiencePoints = state.ToObject<float>();
      }
   }
}