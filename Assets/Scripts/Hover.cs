using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
	//Use this for initialization
	void Start()
    {
		
	}
	
	//Update is called once per frame
	void Update()
    {
        transform.Translate(0, Mathf.Sin(Time.time) * .01f, 0);
	}
}
