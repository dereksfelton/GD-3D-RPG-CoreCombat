using RPG.Attributes;
using UnityEngine;

namespace Scripts.Attributes
{
   public class HealthBar : MonoBehaviour
   {
      [SerializeField] GameObject healthBarImage = null;

      Vector3 _scalingVector;
      bool _healthBarEnabled = false; // ensure bar is hidden to start

      void Awake()
      {
         // set bar to 100% health
         _scalingVector = new Vector3(1, 1, 1);
         healthBarImage.transform.localScale = _scalingVector;
         
         // deactivate visualization
         transform.GetChild(0).gameObject.SetActive(false);
      }

      public void UpdateHealthBar(GameObject character)
      {
         // if bar is inactive, activate it and its visualization
         if (!_healthBarEnabled)
         {
            transform.GetChild(0).gameObject.SetActive(true);
            _healthBarEnabled = true;
         }
         // calculate my percentage of max health as a decimal
         _scalingVector = new Vector3(character.GetComponent<Health>().GetPercentage0to1(), 1, 1);
         
         // set health bar to this percentage
         healthBarImage.transform.localScale = _scalingVector;
         
         // deactivate the bar if I'm dead
         if (_scalingVector.x <= 0)
         {
            DeactivateHealthBar();
         }
      }

      void DeactivateHealthBar()
      {
         transform.GetChild(0).gameObject.SetActive(false);
         _healthBarEnabled = false;
      }
   }
}