using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    //Player Attributes
    public float speed = 0.5f;              //Speed Player moves at normally
    public float turnRate = 0.5f;           //Speed Player turns at
    public float crouchSpeed = 0.25f;       //Speed player moves at when crouching
    public float runSpeed = 1.0f;           //Speed player moves at when running
    private float currentSpeed;             //Speed at which the player currently moves at

   

    //Player components
    Animator animator;
    private SkinnedMeshRenderer meshRenderer;
    public Material lightMaterial;
    public Material shadowMaterial;

    //Player Flags
    bool isMoving = false; //Indicates whether player is moving
    bool isCrouched = false;
    bool isRunning = false;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        currentSpeed = speed;
	}
	
	// Update is called once per frame
	void Update () {

        ToggleCrouch(); 
        ToggleRun();
        Move();
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

        //Check to see if player should turn
        if (Input.GetButton("Horizontal"))
        {
            transform.Rotate(new Vector3(0, turnRate * Input.GetAxis("Horizontal") * Time.deltaTime, 0));
        }

        //Check to see if player should move forwards or backwards
        if (Input.GetButton("Vertical"))
        {
            movementVector = transform.forward * Input.GetAxis("Vertical"); // set players movement vector to be the forwards or backwards vector based on sign of vertical axis
            isMoving = true;
        }

        if (!isMoving) //if player is not moving
            animator.SetFloat("Speed", 0.0f); //Tell animator not to move
        else
            animator.SetFloat("Speed", currentSpeed); // Tells animator the speed at which the player is moving

        //Move the player based on the movement vector and current speed relative to the world space.
        transform.Translate(movementVector * currentSpeed * Time.deltaTime, Space.World);
    }

    public void InShadow()
    {
        Debug.Log("InShadow");
        meshRenderer.material = shadowMaterial;
    }

    public void InLight()
    {
        Debug.Log("IN LIGHT");
        meshRenderer.material = lightMaterial;
    }
}
