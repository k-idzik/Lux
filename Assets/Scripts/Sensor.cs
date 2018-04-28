using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sensor : MonoBehaviour {

    // Sensor variables
    public bool orbSensor = false;

    [SerializeField] private float detectionRadius; //Radius player must be within to be detected NOTE: Orb sensors the radius should be same as pointlight

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

    // route enemy will patrol on
    public List<Node> patrolRoute;

    private Node currentWaypoint;
    private bool reverse; // for detecting if we're doing the route in reverse, not the patrol state reverse
    private bool scanning = false;
    private bool finished = false;
    private bool running = false;

    private Light sensorLight;

    // Use this for initialization
    void Start()
    {
        // setup
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        currentWaypoint = patrolRoute[0];
        agent.destination = currentWaypoint.transform.position;
        reverse = false;

        sensorLight = GetComponentInChildren<Light>();
    }

    /// <summary>
    /// responsible for detecting the player based on
    /// the cone of vision and checking 
    /// </summary>
    private bool VisionCone()
    {
        Vector3 direction = target.transform.position - transform.position;

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

            

            if (Physics.Raycast(transform.position, direction, out hit, visionConeRange) && hit.transform.gameObject.tag == "Player")
            {
                AlertManager.Instance.Alert(hit.transform);
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

        //Get distance to next node
        float distToNode = (currentWaypoint.transform.position - transform.position).magnitude;
        if (distToNode <= currentWaypoint.nodeRange)
        {
            //Set waypoint to next node
            currentWaypoint = patrolRoute[currentWaypoint.nextNodeIndex];
        }

        // player detection
        if(!orbSensor)
        VisionCone();

        agent.destination = currentWaypoint.transform.position;
    }
   
    // Update is called once per frame
    void Update()
    {
        Patrol();

        //Detection sphere
        RaycastHit rayHit;
        Vector3 direction = target.transform.position - transform.position;
        if(Physics.Raycast(transform.position, direction,out rayHit, detectionRadius) && rayHit.transform.gameObject == target)
        {
            AlertManager.Instance.Alert(rayHit.transform);
        }
    }

    private void OnDrawGizmos()
    {
        //Draw Detection Radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
