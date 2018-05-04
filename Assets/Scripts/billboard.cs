using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboard : MonoBehaviour {
    [SerializeField] private GameObject focus;

	// Use this for initialization
	void Start () {
        focus = GameObject.FindGameObjectWithTag("Goal");
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
		
	}
	
	// Update is called once per frame
	void Update () {
        //toggle compass on/off
        if (Input.GetMouseButtonDown(1))
        {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(!this.gameObject.transform.GetChild(0).gameObject.activeSelf);

        }

        //turn compass parent- not the child, the child maintains its rotation so its always facing up from the ground
        Transform lookPos = focus.transform;
        lookPos.position = new Vector3(lookPos.transform.position.x, this.gameObject.transform.position.y, lookPos.position.z);
        gameObject.transform.LookAt (lookPos);

       
	}
}
