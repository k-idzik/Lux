using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    private bool showingCredits;
    [SerializeField] private TextMesh creditsTxt;

	// Use this for initialization
	void Start () {
        showingCredits = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GotoScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowCredits()
    {
        //update credits bool
        showingCredits = !showingCredits;
        Debug.Log(showingCredits);

        //update text
        if (showingCredits)
        {
            //creditsTxt.text = "Joel, Charles, Josh, John, and Kevin";
            //creditsTxt.enabled = true;
            creditsTxt.text = "Kevin Idzik\nJosh Malmquist\nJohn Palermo\nJoel Shuart\nCharles Williams";
            creditsTxt.fontSize = 85;
            creditsTxt.lineSpacing = .75f;
            creditsTxt.transform.position = new Vector3(creditsTxt.transform.position.x, 45, creditsTxt.transform.position.z);
        }
        else
        {
            //creditsTxt.text = "Credits";
            //creditsTxt.enabled = false;
            creditsTxt.text = "LUX";
            creditsTxt.fontSize = 500;
            creditsTxt.lineSpacing = 1;
            creditsTxt.transform.position = new Vector3(creditsTxt.transform.position.x, -1, creditsTxt.transform.position.z);
        }
    }

}
