using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    //GameUI Components
    [SerializeField] private Slider shadowMeter; //Tracks how much shadow life force player has left

    //GameUI variables
    private Player player; //Holds reference to the player

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
	}
	
	// Update is called once per frame
	void Update () {
        shadowMeter.value = player.shadowLife;
	}
}
