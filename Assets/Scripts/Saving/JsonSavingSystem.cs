using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
   public class JsonSavingSystem : MonoBehaviour
   {
      [SerializeField] SavingStrategy strategy;

      /// <summary>
      /// Will load the last scene that was saved and restore the state. This
      /// must be run as a coroutine.
      /// </summary>
      /// <param name="saveFile">The save file to consult for loading.</param>
      public IEnumerator LoadLastScene(string saveFile)
      {
         JObject state = LoadJsonFromFile(saveFile);
         IDictionary<string, JToken> stateDict = state;
         int buildIndex = SceneManager.GetActiveScene().buildIndex;
         if (stateDict.ContainsKey("lastSceneBuildIndex"))
         {
            buildIndex = (int)stateDict["lastSceneBuildIndex"];
         }
         yield return SceneManager.LoadSceneAsync(buildIndex);
         RestoreFromToken(state);
      }

      /// <summary>
      /// Save the current scene to the provided save file.
      /// </summary>
      public void Save(string saveFile)
      {
         JObject state = LoadJsonFromFile(saveFile);
         CaptureAsToken(state);
         SaveFileAsJSon(saveFile, state);
      }

      /// <summary>
      /// Delete the state in the given save file.
      /// </summary>
      public void Delete(string saveFile)
      {
         string path = GetPathFromSaveFile(saveFile);
         Debug.Log($"Deleting save file {path}");
         File.Delete(path);
      }

      public void Load(string saveFile)
      {
         RestoreFromToken(LoadJsonFromFile(saveFile));
      }

      public IEnumerable<string> ListSaves()
      {
         foreach (string path in Directory.EnumerateFiles(Application.persistentDataPath))
         {
            if (Path.GetExtension(path) == strategy.GetExtension())
            {
               yield return Path.GetFileNameWithoutExtension(path);
            }
         }
      }

      // PRIVATE

      private JObject LoadJsonFromFile(string saveFile)
      {
         return strategy.LoadFromFile(saveFile);
      }

      private void SaveFileAsJSon(string saveFile, JObject state)
      {
         strategy.SaveToFile(saveFile, state);
      }


      private void CaptureAsToken(JObject state)
      {
         IDictionary<string, JToken> stateDict = state;
         foreach (JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())
         {
            stateDict[saveable.GetUniqueIdentifier()] = saveable.CaptureAsJtoken();
         }

         stateDict["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
      }


      private void RestoreFromToken(JObject state)
      {
         IDictionary<string, JToken> stateDict = state;
         foreach (JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())
         {
            string id = saveable.GetUniqueIdentifier();
            if (stateDict.ContainsKey(id))
            {
               saveable.RestoreFromJToken(stateDict[id]);
            }
         }
      }


      private string GetPathFromSaveFile(string saveFile)
      {
         return strategy.GetPathFromSaveFile(saveFile);
      }
   }
}

