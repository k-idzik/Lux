using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PursuitCone : MonoBehaviour
{
    // enemy variables

    // determines how the patrol behaves when it reaches its final node
    // LOOP: will restart at the beginning of its route, creating a loop
    // REVERSE: will visit all nodes in reverse order, good for lines
    public enum PatrolType { LOOP, REVERSE };
    public PatrolType patrolBehavior;

    // cooldown between when enemy searches for player again
    public float searchCooldown;

    // angle of the vision cone, will extend this angle in both directions
    public float visionAngle;

    // maximum range of vision cone
    public float visionConeRange;

    // general enemy variables
    private GameObject target;
    private NavMeshAgent agent;
    private DetectionSphere detectionSphere;

    // route enemy will patrol on
    private List<Transform> patrolRoute;

    // variables for tracking enemy state
    private enum State { PATROL ,PURSUE };
    private State currentState;
    private State previousState;
    private List<bool> visitedWaypoints;
    private Transform currentWaypoint;
    private bool reverse; // for detecting if we're doing the route in reverse, not the patrol state reverse
    private bool scanning = false;
    private bool finished = false;
    private bool running = false;
    public float nodeDetectRange = 1.5f;

    public List<Transform> PatrolRoute
    {
        get { return patrolRoute; }
        set { patrolRoute = value; }
    }

    public Transform CurrentWaypoint
    {
        get { return currentWaypoint; }
        set { currentWaypoint = value; }
    }

    public List<bool> VisitedWaypoints
    {
        get { return visitedWaypoints; }
        set { visitedWaypoints = value; }
    }

    // Use this for initialization
    void Start()
    {
        // setup
        agent = GetComponent<NavMeshAgent>();
        detectionSphere = GetComponentInChildren<DetectionSphere>();
        target = GameObject.FindGameObjectWithTag("Player");

        // route initialization
        // gameManager should handle adding points/setting first point
        visitedWaypoints = new List<bool>(); 
        reverse = false;
        currentState = State.ALERT_POINT;

        // assign a default value of false for each waypoint in the patrol route
        for (int i = 0; i < patrolRoute.Count; i++)
        {
            visitedWaypoints.Add(false);
        }
    }

    /// <summary>
    /// responsible for detecting the player based on
    /// the cone of vision and checking 
    /// </summary>
    private bool VisionCone()
    {
        Vector3 direction = target.transform.position - this.transform.position;

        float angle = Vector3.Angle(direction, transform.forward);

        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(visionAngle, transform.up) * transform.forward * visionConeRange);
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-visionAngle, transform.up) * transform.forward * visionConeRange);
        Debug.DrawLine(transform.position, transform.position + transform.forward * visionConeRange);

        if (angle > visionAngle)
        {
            return false;
        }
        else
        {
            RaycastHit hit;

            Physics.Raycast(transform.position, direction, out hit, visionConeRange);

            if (hit.transform.gameObject == target)
            {

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// routes the enemy along the patrol route,
    /// hitting each waypoint before resetting the
    /// route
    /// </summary>
    private void Patrol()
    {
        agent.destination = currentWaypoint.transform.position;

        // if they reach spawn again then destroy them
        if (visitedWaypoints[visitedWaypoints.Count - 1] == true)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // detect further waypoints
            Transform nextNode = patrolRoute[patrolRoute.FindIndex(t => currentWaypoint) + 1];
            if ((nextNode.position - transform.position).magnitude < nodeDetectRange)
            {
                visitedWaypoints[patrolRoute.FindIndex(t => currentWaypoint)] = true;
                currentWaypoint = nextNode;
            }

            // player detection
            if (VisionCone() || detectionSphere.PlayerDetected)
            {
                previousState = currentState;
                currentState = State.PURSUE;
            }
        }
    }

    /// <summary>
    /// responsible for finding and pursuing player
    /// </summary>
    private void Pursue()
    {
        if (finished == false)
        {
            Debug.DrawLine(transform.position, target.transform.position, Color.red);
            agent.destination = target.transform.position;
            if (running == false)
            {
                StartCoroutine(Chase(searchCooldown));
            }
        }
        else
        {
            finished = false;
            currentState = previousState;
        }
    }

    internal IEnumerator Chase(float duration)
    {
        running = true;
        yield return new WaitForSeconds(duration);
        finished = true;
        running = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.ALERT_POINT:
                Patrol();
                break;

            case State.SPAWN_POINT:
                Patrol();
                break;

            case State.PURSUE:
                Pursue();
                break;
        }
    }
}
