using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // enemy variables

    // determines how the patrol behaves when it reaches its final node
    // LOOP: will restart at the beginning of its route, creating a loop
    // REVERSE: will visit all nodes in reverse order, good for lines
    public enum PatrolType { LOOP, REVERSE};
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
    public List<GameObject> patrolRoute;

    // variables for tracking enemy state
    private enum State { PATROL, PURSUE};
    private State currentState;
    private List<bool> visitedWaypoints;
    private GameObject currentWaypoint;
    private bool reverse; // for detecting if we're doing the route in reverse, not the patrol state reverse
    private bool patrol;
    private bool pursuit;
    private bool scanning = false;
    private bool finished = false;
    private bool running = false;

    public bool Pursuit
    {
        get
        {
            return pursuit;
        }
    }

    // Use this for initialization
    void Start()
    { 
        // setup
        agent = GetComponent<NavMeshAgent>();
        detectionSphere = GetComponentInChildren<DetectionSphere>();
        target = GameObject.FindGameObjectWithTag("Player");
        visitedWaypoints = new List<bool>();
        currentWaypoint = patrolRoute[0];
        reverse = false;
        currentState = State.PATROL;

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

        if (visitedWaypoints[visitedWaypoints.Count - 1] == true && patrolBehavior == PatrolType.REVERSE)
        {
            currentWaypoint = patrolRoute[visitedWaypoints.Count - 2];

            reverse = true;

            for (int i = 0; i < visitedWaypoints.Count; i++)
            {
                visitedWaypoints[i] = false;
            }
        }
        else if (visitedWaypoints[visitedWaypoints.Count - 1] == true && patrolBehavior == PatrolType.LOOP)
        {
            currentWaypoint = patrolRoute[0];

            for (int i = 0; i < visitedWaypoints.Count; i++)
            {
                visitedWaypoints[i] = false;
            }
        }

        if (visitedWaypoints[0] == true)
        {
            currentWaypoint = patrolRoute[1];

            reverse = false;

            for (int i = 0; i < visitedWaypoints.Count; i++)
            {
                visitedWaypoints[i] = false;
            }
        }

        // player detection
        if (VisionCone() || detectionSphere.PlayerDetected)
        {
            currentState = State.PURSUE;
        }
    }

    /// <summary>
    /// dectecting when an enemy hits a waypoint, updates the previous
    /// waypoint and sets the new one
    /// </summary>
    /// <param name="coll"></param>
    public void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.tag == "Waypoint" && coll.gameObject == currentWaypoint && reverse == false)
        {
            for (int i = 0; i < patrolRoute.Count; i++)
            {
                if (coll.gameObject == patrolRoute[i])
                {
                    //agent.Stop();

                    visitedWaypoints[i] = true;

                    if (i != patrolRoute.Count - 1)
                    {
                        currentWaypoint = patrolRoute[i + 1];
                    }

                    return;
                }
            }
        }

        if (coll.transform.tag == "Waypoint" && coll.gameObject == currentWaypoint && reverse == true)
        {
            for (int i = 0; i < patrolRoute.Count; i++)
            {
                if (coll.gameObject == patrolRoute[i])
                {
                    //agent.Stop();

                    visitedWaypoints[i] = true;

                    if (i != 0)
                    {
                        currentWaypoint = patrolRoute[i - 1];
                    }

                    return;
                }
            }
        }
    }

    public void OnTriggerStay(Collider coll)
    {
        if (coll.transform.tag == "Waypoint" && coll.gameObject == currentWaypoint && reverse == false)
        {
            for (int i = 0; i < patrolRoute.Count; i++)
            {
                if (coll.gameObject == patrolRoute[i])
                {
                    visitedWaypoints[i] = true;

                    if (i != patrolRoute.Count - 1)
                    {
                        currentWaypoint = patrolRoute[i + 1];
                    }

                    return;
                }
            }
        }

        if (coll.transform.tag == "Waypoint" && coll.gameObject == currentWaypoint && reverse == true)
        {
            for (int i = 0; i < patrolRoute.Count; i++)
            {
                if (coll.gameObject == patrolRoute[i])
                {
                    visitedWaypoints[i] = true;

                    if (i != 0)
                    {
                        currentWaypoint = patrolRoute[i - 1];
                    }

                    return;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Player playerScript = FindObjectOfType<Player>();
            Debug.Log("Gotcha!");
        }
    }

    /// <summary>
    /// responsible for finding and pursuing player
    /// </summary>
    private void Pursue()
    {
        if (finished == false)
        {
            Debug.DrawLine(transform.position,target.transform.position,Color.red);
            agent.destination = GameObject.FindGameObjectWithTag("Player").transform.position;
            if (running == false)
            {
                StartCoroutine(Chase(searchCooldown));
            }
        }

        if (finished == true)
        {
            finished = false;
            currentState = State.PATROL;
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
            case State.PATROL:
                Patrol();
                break;

            case State.PURSUE:
                Pursue();
                break;
        }
    }
}
