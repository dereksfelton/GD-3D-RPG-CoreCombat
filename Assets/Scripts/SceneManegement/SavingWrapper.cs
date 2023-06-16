using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
   public class SavingWrapper : MonoBehaviour
   {
      const string defaultSaveFile = "save";

      [SerializeField] float fadeInDuration = 0.2f;

      private IEnumerator Start()
      {
         Fader fader = FindFirstObjectByType<Fader>();
         fader.FadeOutImmediate();
         
         yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);

         yield return fader.FadeIn(fadeInDuration);
      }

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