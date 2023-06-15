using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
   // NOTE: Keep these enum values (other than None) synced with scene ids defined
   //       in File | Build Settings... | Scenes In Build
   public enum Map:short {
      None = -1,
      Village = 0,
      Mountains = 1
   }

   // Ids you can apply to portals in a single map. Of course you could
   // rename these or add even more, but 26 portals per map gives plenty 
   // of options.
   public enum PortalId:byte { 
      None, A, B, C, D, E, F, G, H, I, J, K, L, 
      M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z 
   };

   public class Portal : MonoBehaviour
   {
      // NOTE: a portal's id should never remain None
      [SerializeField] PortalId portalId = PortalId.None;

      // NOTE: if this portal is just the destination of a one-way portal,
      //       then destinationMap could remain None with further checks
      [SerializeField] Map destinationMap = Map.None;

      // id of the destination portal in the destination scene
      // ...likewise, if this portal is just the destination of a 
      //    one-way portal, destinationPortalId can remain None.
      [SerializeField] PortalId destinationPortalId = PortalId.None; 

      // reference to the portal instance's spawn point, assigned in Prefab editor
      [SerializeField] Transform spawnPoint;

      // fader duration values
      [SerializeField] float fadeOutDuration = 1.5f;
      [SerializeField] float fadeInDuration = 1.5f;
      [SerializeField] float waitWhileFadedDuration = 0.5f;

      void OnTriggerEnter(Collider other) {
         // if this is a one-way destination, don't do anything
         if (destinationMap == Map.None) return;

         // otherwise do the transtion if the collider is tagged as Player
         if (other.gameObject.CompareTag("Player"))
         {
            StartCoroutine(Transition());
         }
      }

      private IEnumerator Transition()
      {
         // get the current scene's build index
         int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
         
         // get the build index of the destaintion map
         short destMapIndex = (short)destinationMap;

         // determine whether we're moving to a new map
         bool movingToNewMap = sceneBuildIndex != destMapIndex;

         // get reference to Fader & fade out
         Fader fader = FindObjectOfType<Fader>();
         yield return fader.FadeOut(fadeOutDuration);

         //only load new scene if we're moving to a new map
         if (movingToNewMap)
         {
            // NOTE: DontDestroyOnLoad only works when gameObject is at scene root!
            DontDestroyOnLoad(gameObject);

            // save state of scene we're leaving
            SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();
            savingWrapper.Save();

            // load new scene asynchronously; wait until fully loaded
            AsyncOperation loadingScene = SceneManager.LoadSceneAsync(destMapIndex);
            while (!loadingScene.isDone) yield return null;

            // load state of scene we're entering
            savingWrapper.Load();
         }

         // find the other portal
         Portal otherPortal = GetOtherPortal();
         
         // update player location and rotation based on the portal found
         UpdatePlayer(otherPortal);

         // wait while faded out (to let camera to settle, etc.), then fade in
         yield return new WaitForSeconds(waitWhileFadedDuration);
         yield return fader.FadeIn(fadeInDuration);

         // finally destroy this portal object, but only if we've moved to a new map
         if (movingToNewMap) Destroy(gameObject);
      }

      private void UpdatePlayer(Portal otherPortal)
      {
         // get references to player and their nav mesh agent
         GameObject player = GameObject.FindWithTag("Player");
         NavMeshAgent navMeshAgent = player.GetComponent<NavMeshAgent>();

         // move and rotate player based on the destination portal's spawn point
         navMeshAgent.Warp(otherPortal.spawnPoint.position);
         player.transform.rotation = otherPortal.spawnPoint.rotation;
      }

      private Portal GetOtherPortal()
      {
         // get the current scene's build index
         int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

         // loop through active Portal instances
         foreach (Portal portal in FindObjectsByType<Portal>(FindObjectsSortMode.None))
         {
            // if the portal's scene's build index doesn't match the current one,
            // don't consider it. This checkl is necessary because the "source" portal
            // is among the portals returned by FindObjectsByType ... probably because
            // we told it to DontDestroyOnLoad.
            if (sceneBuildIndex != portal.gameObject.scene.buildIndex) continue;
            
            // if the id of the portal we're looping over now matches that of the
            // source portal's destination, then we found the one we want to move to.
            if (portal.portalId == this.destinationPortalId) return portal;
         }
         return null;
      }
   }
}