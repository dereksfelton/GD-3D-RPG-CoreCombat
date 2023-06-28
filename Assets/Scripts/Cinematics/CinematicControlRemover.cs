using RPG.Core;
using RPG.Control;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
   public class CinematicControlRemover : MonoBehaviour
   {
      GameObject player;

      private void Awake()
      {
         player = GameObject.FindWithTag("Player");
      }

      void OnEnable()
      {
         GetComponent<PlayableDirector>().played += DisableControl;
         GetComponent<PlayableDirector>().stopped += EnableControl;
      }

      void OnDisable()
      {
         GetComponent<PlayableDirector>().played -= DisableControl;
         GetComponent<PlayableDirector>().stopped -= EnableControl;
      }

      void DisableControl(PlayableDirector pDirector)
      {
         player.GetComponent<ActionScheduler>().CancelCurrentAction();
         player.GetComponent<PlayerController>().enabled = false;
      }
      void EnableControl(PlayableDirector pDirector)
      {
         player.GetComponent<PlayerController>().enabled = true;
      }
   }
}
