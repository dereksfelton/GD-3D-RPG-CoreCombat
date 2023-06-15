using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
   public class SavingWrapper : MonoBehaviour
   {
      const string defaultSaveFile = "save";

      private void Update()
      {
         if (Input.GetKeyDown(KeyCode.L))
         {
            Load();
         }

         if (Input.GetKeyDown(KeyCode.S))
         {
            Save();
         }
      }

      public void Save()
      {
         GetComponent<SavingSystem>().Save(defaultSaveFile);
      }

      public void Load()
      {
         GetComponent<SavingSystem>().Load(defaultSaveFile);
      }
   }
}