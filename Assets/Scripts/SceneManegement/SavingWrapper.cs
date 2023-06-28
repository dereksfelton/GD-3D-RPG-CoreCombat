using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
   public class SavingWrapper : MonoBehaviour
   {
      const string defaultSaveFile = "save";

      [SerializeField] float fadeInDuration = 0.2f;

      private void Awake()
      {
         StartCoroutine(LoadLastScene());
      }

      private IEnumerator LoadLastScene()
      {
         // load the last scene ... this ensures that all Awakes have 
         // been called before making calls to Fader.
         yield return GetComponent<JsonSavingSystem>().LoadLastScene(defaultSaveFile);

         // fade out, then fade in
         Fader fader = FindFirstObjectByType<Fader>();
         fader.FadeOutImmediate();
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

         if (Input.GetKeyDown(KeyCode.Delete))
         {
            Delete();
         }
      }

      public void Save()
      {
         GetComponent<JsonSavingSystem>().Save(defaultSaveFile);
      }

      public void Load()
      {
         GetComponent<JsonSavingSystem>().Load(defaultSaveFile);
      }

      public void Delete()
      {
         GetComponent<JsonSavingSystem>().Delete(defaultSaveFile);
      }
   }
}