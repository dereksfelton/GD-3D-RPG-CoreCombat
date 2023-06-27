using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Attributes
{
   public class Experience : MonoBehaviour, IJsonSaveable
   {
      [SerializeField] float experiencePoints = 0;

      public void GainExperience(float experience)
      {
         experiencePoints += experience;
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