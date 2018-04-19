using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour {

    [SerializeField] private float onTime;
    [SerializeField] private float offTime;
    private float timeTaken;
    private Light flickerLight;

	// Use this for initialization
	void Start () {
        flickerLight = this.gameObject.GetComponent<Light>();	
	}
	
	// Update is called once per frame
	void Update () {
        //check if light state
        if (flickerLight.enabled) //on
        {
            //update time taken
            timeTaken += Time.deltaTime;

            //check if its time to turn off the light
            if (onTime <= timeTaken) //it is
            {
                //reset timeTaken
                timeTaken = 0;

                //turn off light
                flickerLight.enabled = false;
            }
        }
        else //off
        {
            //update time taken
            timeTaken += Time.deltaTime;

            //check if its time to turn on the light
            if (offTime <= timeTaken) //it is
            {
                //reset timeTaken
                timeTaken = 0;

                //turn off light
                flickerLight.enabled = true;
            }
        }
		
	}
}
