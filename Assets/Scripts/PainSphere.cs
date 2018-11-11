using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainSphere : MonoBehaviour {

    private Player player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Hurt player real bad when they enter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // ensure player can be 'spotted' by the light
            Vector3 direction = player.gameObject.transform.position - this.transform.position;
            RaycastHit hit;
            Physics.Raycast(transform.position, direction, out hit);

            if (hit.transform.gameObject == player.gameObject)
            {
                player.InLight("PainSphere");
            }
        }
    }

    /// <summary>
    /// Hurt player real bad when they enter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            // ensure player can be 'spotted' by the light
            Vector3 direction = player.gameObject.transform.position - this.transform.position;
            RaycastHit hit;
            Physics.Raycast(transform.position, direction, out hit);

            if (hit.transform.gameObject == player.gameObject)
            {
                player.InLight("PainSphere");
            }
        }
    }

    /// <summary>
    /// Hurt player real bad when they enter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.InShadow();
        }
    }
}
