using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PursuitCone : MonoBehaviour
{
    // enemy variables

    // cooldown between when enemy searches for player again
    public float searchCooldown;

    // angle of the vision cone, will extend this angle in both directions
    public float visionAngle;

    // maximum range of vision cone
    public float visionConeRange;

    // general enemy variables
    private Player target;
    private NavMeshAgent agent;
    [SerializeField] private float detectionRadius = 0.5f;

    // route enemy will patrol on
    private Vector3 alertPoint;
    private Vector3 spawnPoint;

    // variables for tracking enemy state
    public enum State { SPAWN_POINT,ALERT_POINT,PURSUE, DEAD };
    public State currentState;
    private State previousState;
    private bool reverse; // for detecting if we're doing the route in reverse, not the patrol state reverse
    private bool scanning = false;
    private bool finished = false;
    private bool running = false;
    public float nodeDetectRange = 0.5f;

    private AlertManager alertMan; //Because instances don't work apparently

    public Vector3 AlertPoint
    {
        get { return alertPoint; }
        set { alertPoint = value; }
    }
    public Vector3 SpawnPoint
    {
        get { return spawnPoint; }
        set { spawnPoint = value; }
    }

    // Use this for initialization
    void Start()
    {
        // setup
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // route initialization
        // gameManager should handle adding points/setting first point
        reverse = false;
        currentState = State.ALERT_POINT;
        previousState = State.ALERT_POINT;

        //Assign spotlight to shadow detect scipt
        Light[] light = GetComponentsInChildren<Light>();
        for(int i = 0; i < light.Length; i++) 
            target.GetComponent<ShadowDetect.ShadowDetect>().Lights.Add(light[i]);
        agent.Warp(transform.position);

        alertMan = GameObject.FindGameObjectWithTag("Alert").GetComponent<AlertManager>();
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

            if (Physics.Raycast(transform.position, direction, out hit, visionConeRange) && hit.transform.gameObject == target.gameObject)
            {
                //AlertManager.Instance.Alert(hit.transform); //Error
                alertMan.Alert(hit.transform);
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
    private void Patrol(Vector3 nextNode, State nextState)
    {
        if ((transform.position - nextNode).magnitude < nodeDetectRange)
            currentState = nextState;
        
        if (VisionCone())
        {
            currentState = State.PURSUE;
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
        //Check if player enters detection radius
        RaycastHit rayHit;
        Vector3 direction = target.transform.position - transform.position;
        if ((Physics.Raycast(transform.position, direction, out rayHit, detectionRadius) && rayHit.transform.gameObject == target.gameObject))
        {
            //AlertManager.Instance.Alert(rayHit.transform);
            alertMan.Alert(rayHit.transform); //Error
            if (!target.InLightFlag) //prevents InLight method from stacking
                rayHit.transform.gameObject.GetComponent<Player>().InLight("ShadowDetect");
        }

        switch (currentState)
        {
            case State.ALERT_POINT:
                agent.destination = alertPoint;
                Patrol(alertPoint, State.SPAWN_POINT);
                break;

            case State.SPAWN_POINT:
                agent.destination = spawnPoint;
                Patrol(spawnPoint, State.DEAD);
                break;

            case State.DEAD:
                //AlertManager.Instance.Dogs.Remove(this); //Error
                alertMan.Dogs.Remove(this);
                Light[] light = GetComponentsInChildren<Light>();
                for (int i = 0; i < light.Length; i++)
                    target.GetComponent<ShadowDetect.ShadowDetect>().Lights.Remove(light[i]);
                //if (AlertManager.Instance.Dogs.Count == 0) //Error
                //    AlertManager.Instance.dogsSpawned = false;
                if (alertMan.Dogs.Count == 0) //Error
                    alertMan.dogsSpawned = false;
                Destroy(gameObject);
                break;

            case State.PURSUE:
                Pursue();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
