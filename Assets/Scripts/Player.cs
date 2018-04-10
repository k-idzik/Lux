using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    //Player Attributes
    public float speed = 0.5f;
    public float turnRate = 0.5f;
    public float crouchSpeed = 0.25f;

    //Player components
    Animator animator;

    //Player Flags
    bool isMoving = false; //Indicates whether player is moving
    bool isCrouched = false;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>(); 
	}
	
	// Update is called once per frame
	void Update () {

        ToggleCrouch(); //Turn crouch on or off when crouch key is pressed
        Move();
	}

    private void ToggleCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouched = !isCrouched; //Toggle Crouch boolean to be opposite of what it was originally
            animator.SetBool("Squat", isCrouched); //Tell animator whether player is crouching or not
        }
    }

    private void Move()
    {
        Vector3 movementVector = new Vector3();

        isMoving = false;

        if (Input.GetButton("Horizontal"))
        {
            transform.Rotate(new Vector3(0, turnRate * Input.GetAxis("Horizontal") * Time.deltaTime, 0));
            //isMoving = false;
        }

        if (Input.GetButton("Vertical"))
        {
            movementVector = transform.forward * Input.GetAxis("Vertical");
            isMoving = true;
        }

        if (!isMoving) //if player is not moving
            animator.SetFloat("Speed", 0.0f); //Tell animator not to 
        else if (!isCrouched)
            animator.SetFloat("Speed", speed); //Player walk animation plays at normal speed
        else
            animator.SetFloat("Speed", crouchSpeed);

        if(!isCrouched)
            transform.Translate(movementVector * speed * Time.deltaTime, Space.World);
        else
            transform.Translate(movementVector * crouchSpeed * Time.deltaTime, Space.World);
    }
}
