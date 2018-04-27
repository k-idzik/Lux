using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertManager : Singleton<AlertManager> {

    // prefabs for spawnable enemies
    public GameObject pursuitCone;
    public GameObject[] spawnPoints;
    public bool alerted = false;
    public bool dogsSpawned = false;
    public Transform lastKnownPosition;
    [SerializeField] private GameObject playerSpottedModel;
    private List<PursuitCone> dogs;
    private Coroutine alertedTimer;
    public float alertTime = 5.0f;

    //Properties
    public List<PursuitCone> Dogs
    {
        get { return dogs; }
    }
    // Use this for initialization
    void Start() {
        spawnPoints = GameObject.FindGameObjectsWithTag("Dog Spawn");
        dogs = new List<PursuitCone>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void Alert(Transform detectedPosition)
    {
        // update dogs to go after players last known position
        if (dogsSpawned)
        {
            for(int i = 0; i < dogs.Count; i++)
            {
                dogs[i].AlertPoint = detectedPosition.position;
                dogs[i].currentState = PursuitCone.State.ALERT_POINT;
            }
        }
        else
        {
            spawnPursuitCones(detectedPosition.position);
            dogsSpawned = true;
        }

        alerted = true; //set alert status to alerted
        lastKnownPosition = detectedPosition; //Store last known player position

        //Turn on LastSeen Player model
        playerSpottedModel.transform.position = lastKnownPosition.position;
        playerSpottedModel.transform.rotation = lastKnownPosition.rotation;
        playerSpottedModel.SetActive(true);

        //Start Alert Timer
        if(alertedTimer != null)
        {
            StopCoroutine(alertedTimer);
        }

        alertedTimer = StartCoroutine(AlertTimer());
    }

    public IEnumerator AlertTimer()
    {
        yield return new WaitForSeconds(alertTime);
        alerted = false;
        playerSpottedModel.SetActive(false);
    }

    /// <summary>
    /// will spawn ANGERY BOIS
    /// </summary>
    public void spawnPursuitCones(Vector3 alertPoint)
    {
        if(dogsSpawned) { return; }//Don't spawn dogs if they are currently spawned

        // spawn all enemies at spawn points
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            dogs.Add(Instantiate(pursuitCone, spawnPoints[i].transform.position, Quaternion.identity).GetComponent<PursuitCone>());

            // set transforms in newly instantiated pursuit cone
            dogs[i].SpawnPoint = spawnPoints[i].transform.position;
            dogs[i].AlertPoint = alertPoint;
        }

        dogsSpawned = true;
    }
}
