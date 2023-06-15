using System;
using UnityEngine;

namespace RPG.Core {
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectPrefab;

        static bool hasSpawned = false; // our only concession to the "singleton" pattern
                                        // ...much better than making the whole class static
        private void Awake() {
            if (hasSpawned) return;

            SpawnPersistentObjects();
            hasSpawned = true;
        }

      private void SpawnPersistentObjects()
      {
         GameObject persistentObject = Instantiate(persistentObjectPrefab);
         DontDestroyOnLoad(persistentObject);
      }
   }
}
