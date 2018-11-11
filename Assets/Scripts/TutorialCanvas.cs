using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCanvas : MonoBehaviour
{
	// Use this for initialization
	void Start()
    {
        Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update()
    {
        //Start the game
		if (Input.GetButton("Pulse"))
        {
            Time.timeScale = 1;
            Destroy(gameObject);
        }
	}
}
