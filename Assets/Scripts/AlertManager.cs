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
    public Coroutine altertedTimer;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public  void Alert(Transform detectedPosition)
    {
        alerted = true;

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
