using UnityEngine;
using UnityEngine.Playables;
namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool alreadyTriggered = false;
        private void OnTriggerEnter(Collider other) {
            if (alreadyTriggered) return;

            if (string.Equals(other.tag, "Player"))
            {
                alreadyTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }
    }
}