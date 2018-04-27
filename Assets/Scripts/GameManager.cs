using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // prefabs for spawnable enemies
    public GameObject pursuitCone;
    public GameObject[] spawnPoints;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// called from sensor enemy to handle all alert changes
    /// </summary>
    public void alert(Transform alertPoint)
    {
        // spawn all enemies at spawn points
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            PursuitCone newCone = GameObject.Instantiate(pursuitCone).GetComponent<PursuitCone>();

            // set variables in newly instantiated pursuit cone
            
        }
    }
}
