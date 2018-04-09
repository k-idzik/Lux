using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchLight : MonoBehaviour {
    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = this.gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        Turn();
	}

    //Turn the spotlight
    public void Turn()
    {
        //check if player is hitting interact button
        if (Input.GetKey(KeyCode.Space))
        {
            //freeze pos and xz rot
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        }
        else
        {
            //freeze spotlight
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
