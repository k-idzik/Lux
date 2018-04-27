using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertManager : Singleton<AlertManager> {

    // prefabs for spawnable enemies
    public GameObject pursuitCone;
    public GameObject[] spawnPoints;
    public bool alerted = false;
    public Transform lastKnownPosition;
    [SerializeField] private GameObject playerSpottedModel;
    private Coroutine alertedTimer;
    public float alertTime = 5.0f;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Alert(Transform detectedPosition)
    {

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
    /// called from sensor enemy to handle all alert changes
    /// </summary>
    public void LUANCHBOI(Transform alertPoint)
    {
        // spawn all enemies at spawn points
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            PursuitCone newCone = GameObject.Instantiate(pursuitCone).GetComponent<PursuitCone>();

            // set variables in newly instantiated pursuit cone
            
        }
    }
}
