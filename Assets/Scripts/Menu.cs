using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    private bool showingCredits;
    [SerializeField] private Text creditsTxt;

	// Use this for initialization
	void Start () {
        showingCredits = false;
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

        //update text
        if (showingCredits)
        {
            creditsTxt.text = "Joel, Charles, Josh, John, and Kevin";
        }
        else
        {
            creditsTxt.text = "Credits";
        }
    }

}
