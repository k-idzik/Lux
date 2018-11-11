using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionSphere : MonoBehaviour {

    private bool playerDetected;

    public bool PlayerDetected
    {
        get { return playerDetected; }
    }

	// Use this for initialization
	void Start () {
        playerDetected = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.tag == "Player")
        {
            playerDetected = true;
        }
    }
}
