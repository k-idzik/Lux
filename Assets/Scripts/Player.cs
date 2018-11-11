﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    //Player Attributes
    public float maxLife = 100.0f;
    [SerializeField] private float shadowLife;            //Amount of shadow life player has
    public float maxLifeSections = 4.0f;          //number of sections on life bar- hp will cap at the end of current section
    [SerializeField] private float availableSections;        // number of sections that player has used up
    [SerializeField] private float lifePerSection;           //life in each section - maxlife / max life sections

    public float speed = 0.5f;              //Speed Player moves at normally
    public float turnRate = 0.5f;           //Speed Player turns at
    public float crouchSpeed = 0.25f;       //Speed player moves at when crouching
    public float runSpeed = 1.0f;           //Speed player moves at when running
    private float currentSpeed;             //Speed at which the player currently moves at
    private Vector3 prevMousePos;           //holds the previous mouse Postition

    public float lightDamage= 2.0f;         //Damage taken when in light
    public float shadowRechargeRate = 0.5f; //Rate at which shadow life recharges

    [SerializeField] private float pulseRange = 5.0f;   //Range that Pulse Light will shine to
    [SerializeField] private float pulseRate = 0.05f;   //rate at which to brighten/dim pulse light
    [SerializeField] private float pulseCost = 25.0f;   //Shadow Life cost to use pulse
    [SerializeField] private float pulseCooldownTime = 1.0f; //Time player must wait before they can use the pulse power again
   
    //Player components
    Animator animator;
    private SkinnedMeshRenderer meshRenderer;
    public Material lightMaterial;
    public Material shadowMaterial;
    public Material safeMaterial;
    private ParticleSystem lightParticles;
    private Light pulseLight;               //Point Light used for the Pulse power
    
    //Shadow detection
    //Prevents the bad
    private ShadowDetect.ShadowDetect playerShadowDetect;
    private DetectLights playerDetectLights;

    //Player Flags
    bool isMoving = false; //Indicates whether player is moving
    bool isCrouched = false;
    bool isRunning = false;
    bool canPulse = true;   //Indicates whether the player can use the pulse ability
    private bool inLight = false; //Indicates whether player is currently in light
    private bool isSafe = false; // Indicates if a player is in a safe zone - used to know when to change material

    //screen tint
    [SerializeField] private Image screenTint;

    //Next level to load
    [SerializeField] private int nextScene;

    private AlertManager alertMan; //Because instances don't work apparently

    #region Properties
    public float ShadowLife
    {
        get { return shadowLife; }
        set
        {
            shadowLife = value;

            //update hp cap section
            if(shadowLife <= (lifePerSection * (availableSections - 1))) //is life less than the bottom of the current section
                availableSections--;

            //Check to make sure shadowLife stays within its bounds (0 to the section cap)
            if (shadowLife <= 0)
            {
                Die();
            }
            else if (shadowLife > (lifePerSection * availableSections)) //hp is capped at the section
                shadowLife = (lifePerSection * availableSections); 

            return;
        }
    }

    public bool InLightFlag { get { return inLight; } }
    #endregion

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        currentSpeed = speed;
        prevMousePos = Input.mousePosition;
        lightParticles = gameObject.GetComponent<ParticleSystem>();

        pulseLight = GetComponentInChildren<Light>();

        shadowLife = maxLife; //set HP intillay to max
        availableSections = maxLifeSections; //all sections are available fromt he begining
        lifePerSection = maxLife / maxLifeSections;

        //Lock mouse cursor to the middle of the screen
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        playerShadowDetect = GetComponent<ShadowDetect.ShadowDetect>();
        playerDetectLights = GetComponent<DetectLights>();

        //Setup shadowDetection Lights
        GameObject[] lights = GameObject.FindGameObjectsWithTag("Light");
        for(int i =0; i < lights.Length; i++)
        {
            playerShadowDetect.Lights.Add(lights[i].GetComponent<Light>()); //adds Sensor light to light Detection scr
        }

        InShadow(); //Set this now for instances where there are no lights, only tiles

        // detect if a player spawns in a safe zone
        // seemed wasteful to write an onTriggerStay for a use case that could only happen at the start
        // so this will function the same as that, only it runs once
        GameObject [] safeZones = GameObject.FindGameObjectsWithTag("SafeZone");
        int zonesCount = safeZones.Length;
        for(int i = 0; i < zonesCount; i++)
        {
            if (safeZones[i].GetComponent<BoxCollider>().bounds.Contains(transform.position))
            {
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                meshRenderer.material = safeMaterial;
                isSafe = true;
            }
        }

        alertMan = GameObject.FindGameObjectWithTag("Alert").GetComponent<AlertManager>();
        alertMan.Restart();
    }
	
	// Update is called once per frame
	void Update () {

        ToggleCrouch(); 
        ToggleRun();
        Move();

        if(Input.GetButtonDown("Pulse") && canPulse) //Determine whether
        {
            //Physics.OverlapSphere(transform.position, pulseRange, LayerMask.GetMask("Enemy"));
            StartCoroutine(Pulse());
        }

        prevMousePos = Input.mousePosition;
	}

    /// <summary>
    /// Toggles whether player should crouch
    /// </summary>
    private void ToggleCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouched = !isCrouched; //Toggle Crouch boolean to be opposite of what it was originally
            animator.SetBool("Squat", isCrouched); //Tell animator whether player is crouching or not
            currentSpeed = (isCrouched == true) ? crouchSpeed : speed;

            //Turn Run off
            isRunning = false;
        }
    }

    /// <summary>
    /// Toggles whether the player should run 
    /// </summary>
    private void ToggleRun()
    {
        if(Input.GetButtonDown("Run"))
        {
            isRunning = !isRunning; //Toggle run boolean
            currentSpeed = (isRunning == true) ? runSpeed : speed;

            //Make sure crouch is turnned off if it is not already
            animator.SetBool("Squat", false);
            isCrouched = false;
        }
    }

    /// <summary>
    /// The Move method is responsible for moving the player and setting the animator to play the proper animations
    /// </summary>
    private void Move()
    {
        Vector3 movementVector = new Vector3();

        isMoving = false;

        //Rotate Player based on mouse movements
        Vector3 deltaMousePos = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"),0); //Get the change in mouse Position

        transform.Rotate(0, turnRate * deltaMousePos.x * Time.deltaTime, 0);

        //Check to see if player should turn
        if (Input.GetButton("Horizontal"))
        {
            // transform.Rotate(new Vector3(0, turnRate * Input.GetAxis("Horizontal") * Time.deltaTime, 0));
            movementVector += transform.right * Input.GetAxis("Horizontal");
            isMoving = true;
        }

        //Check to see if player should move forwards or backwards
        if (Input.GetButton("Vertical"))
        {
            movementVector += transform.forward * Input.GetAxis("Vertical"); // set players movement vector to be the forwards or backwards vector based on sign of vertical axis
            isMoving = true;
        }

        if (!isMoving) //if player is not moving
            animator.SetFloat("Speed", 0.0f); //Tell animator not to move
        else
            animator.SetFloat("Speed", currentSpeed); // Tells animator the speed at which the player is moving

        //Move the player based on the movement vector and current speed relative to the world space.
        transform.Translate(movementVector * currentSpeed * Time.deltaTime, Space.World);

        //Reset Cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;
    }

    /// <summary>
    /// Helper method for displaying damage taken on UI
    /// </summary>
    private void DisplayDamage()
    {
        screenTint.color = new Color32(135, 27, 27, 33);

        //check if we are already running the light particles -if not turn them on
        if (!lightParticles.isPlaying)
        {
            lightParticles.Play();
        }
    }

    public void InShadow()
    {
        inLight = false;

        //Make sure these are enabled
        playerShadowDetect.enabled = true;
        playerDetectLights.enabled = true;

        //Debug.Log("InShadow");
        // will not overwrite material if in safe zone
        if (!isSafe)
            meshRenderer.material = shadowMaterial;
        ModShadowLife(shadowRechargeRate);

        screenTint.color = new Color32(0, 0, 0, 0);

        //check if light particles are running - if so turn them off
        if (lightParticles.isPlaying)
        {
            lightParticles.Stop();
        }
    }

    public void InLight(string scriptName)
    {
        inLight = true;
        //Avoid damage effect flashing
        if (scriptName == "DetectLights")
        {
            playerShadowDetect.enabled = false;
        }
        else if (scriptName == "ShadowDetect")
        {
            playerDetectLights.enabled = false;
        }
        else
        {
            playerShadowDetect.enabled = false;
            playerDetectLights.enabled = false;
        }

        //Debug.Log("IN LIGHT");
        // will not overwrite material if in safe zone
        if (!isSafe)
            meshRenderer.material = lightMaterial;
        ModShadowLife(-lightDamage);

        DisplayDamage();
    }

    private void ModShadowLife(float mod)
    {
        ShadowLife = ShadowLife + mod;

        //Set Player scale based on amount of shadow Health left
        Vector3 playerScale = new Vector3(1, 1, 1) * (shadowLife / 100.0f);
        transform.localScale = playerScale;
    }

    private IEnumerator Pulse()
    {
        pulseLight.enabled = true; //Turn on Pulse Light 
        canPulse = false;

        //Brighten/Grow Light Phase
        while (pulseLight.range < pulseRange)
        {
            shadowLife -= pulseCost; //Decrease Shadow Life by cost of pulse
            pulseLight.range += pulseRate;
            //DisplayDamage();
            yield return null; //Cause coroutine to pause till next frame before continuing
        }
        // hold pulse while user holds button
        while (Input.GetButton("Pulse"))
        {
            shadowLife -= pulseCost;
            //DisplayDamage();
            yield return null;
        }
        //Dim/Shrink Light Phase
        while (pulseLight.range > 0.0f)
        {
            pulseLight.range -= pulseRate;
            yield return null;
        }

        pulseLight.enabled = false; //Turn off pulse light
        yield return new WaitForSeconds(pulseCooldownTime);
        canPulse = true;
    }

    //Kills the Player
    private void Die()
    {
        animator.Play("Death");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Reload the scene when the player dies
        //Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Goal")
        {
            SceneManager.LoadScene(nextScene);
        }
        else if (other.tag == "SafeZone") // player has entered safe zone, make it so enemies can't see them
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            meshRenderer.material = safeMaterial;
            isSafe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SafeZone") // player has left safe zone, re-enable raycast
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            meshRenderer.material = shadowMaterial;
            isSafe = false;
        }
    }
}
