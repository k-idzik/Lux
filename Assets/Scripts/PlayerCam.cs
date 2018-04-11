using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour {

    [SerializeField] private GameObject playerTarget;    //Player that camera should follow
    [SerializeField] private float distFromPlayer; //Distance Camera should maintain from player
    [SerializeField] private float height;         //height camera should mantain from floor
   
    // Use this for initialization
	void Start () {
        Vector3 newCamPosition = (-playerTarget.transform.forward.normalized) * distFromPlayer + playerTarget.transform.position;
        newCamPosition.y = height;

        transform.position = newCamPosition;
        transform.LookAt(playerTarget.transform);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newCamPosition = (-playerTarget.transform.forward.normalized) * distFromPlayer + playerTarget.transform.position;
        newCamPosition.y = height;

        transform.position = newCamPosition;
        transform.LookAt(playerTarget.transform);
    }
}
