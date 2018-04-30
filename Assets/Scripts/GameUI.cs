using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    //GameUI Components
    [SerializeField] private Slider shadowMeter; //Tracks how much shadow life force player has left
    [SerializeField] private GameObject sectionsParent;
    [SerializeField] private GameObject sectionPrefab;

    //GameUI variables
    private Player player; //Holds reference to the player

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        CreateSections();
	}
	
	// Update is called once per frame
	void Update () {
        shadowMeter.value = player.ShadowLife;
	}

    //uses fields set in player to visually show the end of each health bar section
    private void CreateSections()
    {
        //loop through each section and create the end point
        for (int i = 0; i < (player.maxLifeSections - 1); i++) //max sections - 1 bc we dont need an end section for hp at full
        {
            //create instance of the section end
            GameObject currentSection = Instantiate(sectionPrefab) as GameObject;

            //make section end a child of sections parent
            currentSection.transform.SetParent(sectionsParent.transform);

            //zero out position
            currentSection.transform.localPosition = sectionsParent.transform.localPosition;

            //move section to proper spot
            //float xPos = (this.gameObject.transform.position.x / (player.maxLifeSections - 2.0f)) * (i + 1.0f);
            RectTransform rt = (RectTransform)this.gameObject.transform;
            float xPos = (rt.rect.width / (player.maxLifeSections)) * (i + 1.0f);
            currentSection.transform.position = new Vector3(xPos, currentSection.transform.position.y, currentSection.transform.position.z);
        }
    }
}
