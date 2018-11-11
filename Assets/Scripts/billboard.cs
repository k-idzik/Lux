using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboard : MonoBehaviour {
    [SerializeField] private GameObject focus;

    // Use this for initialization
    void Start () {
        focus = GameObject.FindGameObjectWithTag("Goal");
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        //toggle compass on/off
        if (Input.GetMouseButtonDown(1))
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(!gameObject.transform.GetChild(0).gameObject.activeSelf);
        }

        //turn compass parent- not the child, the child maintains its rotation so its always facing up from the ground
        Vector3 lookPos = new Vector3(focus.transform.position.x, gameObject.transform.position.y, focus.transform.position.z);
        gameObject.transform.LookAt(lookPos);
    }
}