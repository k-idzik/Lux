using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // determines how the patrol behaves when it reaches its final node
    // LOOP: will restart at the beginning of its route, creating a loop
    // REVERSE: will visit all nodes in reverse order, good for lines
    public enum PatrolType { LOOP, REVERSE};
    public PatrolType patrolBehavior;

    // enemy variables
    private Animator enemyAnim;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float visionAngle;
    private NavMeshAgent agent;
    private DetectionSphere detectionSphere;
    [SerializeField]
    private List<GameObject> patrolRoute;
    [SerializeField]
    private GameObject alertPoint;
    private List<bool> visitedWaypoints;
    private GameObject currentWaypoint;
    private bool reverse;
    private bool patrol;
    private bool pursuit;
    private bool alert;
    private bool scanning = false;
    private bool alertScan = false;
    private bool finished = false;
    private bool running = false;
    private Vector3 lastKnownLoc = Vector3.zero;

    public AudioClip enemyMove1;
    public AudioClip enemyMove2;

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
        agent = GetComponent<NavMeshAgent>();
        detectionSphere = GetComponentInChildren<DetectionSphere>();
        Debug.Log(patrolRoute.Count);

        visitedWaypoints = new List<bool>();

        currentWaypoint = patrolRoute[0];

        reverse = false;

        patrol = true;

        // assign a default value of false for each waypoint in the patrol route
        for (int i = 0; i < patrolRoute.Count; i++)
        {
            visitedWaypoints.Add(false);
        }
    }

    /// <summary>
    /// responsible for seeking the enemy's target.
    /// </summary>
    public Vector3 Seek()
    {
        Vector3 offset = target.transform.position - this.transform.position;
        Vector3 unitOffset = offset.normalized;
        return unitOffset;
    }

    /// <summary>
    /// responsible for detecting the player based on
    /// the cone of vision and checking 
    /// </summary>
    private bool VisionCone()
    {
        target = GameObject.FindGameObjectWithTag("Player");

        Vector3 direction = target.transform.position - this.transform.position;

        float angle = Vector3.Angle(direction, transform.forward);

        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(visionAngle, transform.up) * transform.forward * 20);
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-visionAngle, transform.up) * transform.forward * 20);

        if (angle > visionAngle)
        {
            return false;
        }
        else
        {
            RaycastHit hit;
           
            Physics.Raycast(transform.position, direction, out hit);

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
    }

    /// <summary>
    /// dectecting when an enemy hits a waypoint, updates the previous
    /// waypoint and sets the new one
    /// </summary>
    /// <param name="coll"></param>
    public void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.tag == "Waypoint" && reverse == false)
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

        if (coll.transform.tag == "Waypoint" && reverse == true)
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

        if (coll.transform.tag == "AlertPoint" && alert == true)
        {
            alertScan = true;
            Destroy(coll.gameObject);
        }
    }

    public void OnTriggerStay(Collider coll)
    {
        if (coll.transform.tag == "AlertPoint" && alert == true)
        {
            alertScan = true;
            Destroy(coll.gameObject);
        }
        if (coll.transform.tag == "Waypoint" && reverse == false)
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

        if (coll.transform.tag == "Waypoint" && reverse == true)
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
           
            Debug.Log("WORK");
            agent.destination = GameObject.FindGameObjectWithTag("Player").transform.position;
            if (running == false)
            {
                StartCoroutine(Chase(5));
            }
        }

        if (finished == true)
        {
            finished = false;
            pursuit = false;
            patrol = true;
        }
    }

    internal IEnumerator Chase(float duration)
    {
        running = true;

        yield return new WaitForSeconds(duration);
        finished = true;
        running = false;
    }

    /// <summary>
    /// if player is spotted on patrol enemies will go to last
    /// known location and look for player, if they detect them,
    /// then pursuit will begin
    /// </summary>
    /// <param name="location"></param>
    //private void Alert(Vector3 location)
    //{
    //    agent.destination = location;

    //    if (alertScan)
    //    {
    //        agent.Stop();
            
    //        Coroutine sa =StartCoroutine(Scan(3));

    //        if (scanning == true)
    //        {
    //            if (VisionCone() || detectionSphere.PlayerDetected)
    //            {
    //                StopCoroutine(sa);
    //                alert = false;
    //                pursuit = true;
    //                scanning = false;
    //                alertScan = false;
    //            }
    //        }
    //    }
    //}

    //internal IEnumerator Scan(float duration)
    //{
    //    scanning = true;
    //    visionAngle = 60;

    //    yield return new WaitForSeconds(duration);

    //    visionAngle = 20;
    //    scanning = false;
    //    alertScan = false;
    //    alert = false;
    //    patrol = true;
    //}

 

    // Update is called once per frame
    void Update()
    {
        //enemyAnim.SetBool("PursuitAnim", pursuit);

        if (patrol)
        {
            Patrol();
       
            if (VisionCone() || detectionSphere.PlayerDetected)
            {
                //SoundManager.instance.RandomizeEnemySfx(enemyMove1);
                //alert = true;
                pursuit = true;
                patrol = false;
                //lastKnownLoc = GameObject.FindGameObjectWithTag("Player").transform.position;

                //GameObject.Instantiate(alertPoint, lastKnownLoc, Quaternion.identity);
            }
        }

        if (pursuit)
        {
          
            Pursue();
            //Debug.Log(pursuit);
        }

        //if (alert)
        //{
        //    Alert(lastKnownLoc);
        //}

    }
}
