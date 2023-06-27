using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
   public class ExperienceDisplay : MonoBehaviour
   {
      Experience experience;

      private void Awake()
      {
         experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
      }

      private void Update()
      {
         GetComponent<TMP_Text>().text = experience.XP.ToString();
      }
   }
}
