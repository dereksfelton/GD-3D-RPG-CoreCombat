using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    [SerializeField] Transform target;

    Ray lastRay;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MoveToCursor();
        }
        UpdateAnimator();
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);

        if  (hasHit)
        {
            player.GetComponent<NavMeshAgent>().destination = hit.point;
        }
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;

        // convert to local value relative to character...
        // lets you convert global values into local values the animator needs to know
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        // find how fast I should be moving in a forward direction
        float speed = localVelocity.z;

        // set animator's forwardSpeed value to the speed we calculated...
        // this effectively does what sliding the blend graph to that value would do
        GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }
}
