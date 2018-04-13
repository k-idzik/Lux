using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Detect if the player is in light
public class DetectLights : MonoBehaviour
{
    private Player player; //Player script
    private Light[] lightsOnGOs; //All the lights in the scene
    private int lightPlayerIsIn = -1; //What light the player is under, avoids calls to player from all the lights every frame

	//Use this for initialization
	void Awake()
    {
        player = GetComponent<Player>();

        GameObject[] temp = GameObject.FindGameObjectsWithTag("Light");

        lightsOnGOs = new Light[temp.Length]; //I never remember to initialize the fucking array

        for (int i = 0; i < temp.Length; ++i)
        {
            lightsOnGOs[i] = temp[i].GetComponent<Light>();
        }
    }
	
	//Update is called once per frame
	void Update()
    {
        //Loop through all lights
        for (int i = 0; i < lightsOnGOs.Length && lightPlayerIsIn != -2; ++i)
        {
            //http://answers.unity.com/answers/301825/view.html
            //https://forum.unity.com/threads/how-do-i-detect-if-an-object-is-in-front-of-another-object.53188/

            RaycastHit hit; //Return for the raycast
            Vector3 rayDirection = transform.position - lightsOnGOs[i].transform.position; //Update the direction of the raycast

            //Debug.DrawRay(lightsOnGOs[i].transform.position, rayDirection, Color.yellow); //Debug ray between the player and all of the lights

            //If this is a spotlight
            //Set these to work even if this isn't a spotlight
            float angleToPlayer = 0;
            float spotAngle = 1;
            if (lightsOnGOs[i].type == LightType.Spot)
            {
                angleToPlayer = Vector3.Angle(rayDirection, lightsOnGOs[i].transform.forward);
                spotAngle = lightsOnGOs[i].spotAngle / 2; //Cut this in half, could be on either side
            }

            //If there is a hit
            //Must be within the spotlight, if applicable
            //Make sure it's the player (it will be, or it'll be untagged, but that's probably also the player)
            if (angleToPlayer <= spotAngle && Physics.Raycast(lightsOnGOs[i].transform.position, rayDirection, out hit, lightsOnGOs[i].range) && hit.transform.tag == "Player")
            {
                //Debug.DrawRay(lightsOnGOs[i].transform.position, rayDirection, Color.red); //Debug ray between the player and the light they're within range of

                player.InLight();
                lightPlayerIsIn = i; //Set the light the player is currently in
            }
            //If there's not a hit, the player is in shadow
            //Either leaving a light or all lights have been checked when the player
            //already wasn't under one
            else if (i == lightPlayerIsIn || (lightPlayerIsIn == -1 && i == lightsOnGOs.Length - 1))
            {
                player.InShadow();
                lightPlayerIsIn = -1;
            }
        }
    }

    //Collision with floor tiles
    private void OnTriggerStay(Collider coll)
    {
        //When the player enters a light tile
        if (coll.tag == "LightTile")
        {
            lightPlayerIsIn = -2;
            player.InLight();
        }
    }

    //Collision with floor tiles
    private void OnTriggerExit(Collider coll)
    {
        //When the player leaves a light tile
        if (coll.tag == "LightTile")
        {
            lightPlayerIsIn = -1;
            player.InShadow();
        }
    }
}
