using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public TextMeshPro hiscoretext;

	// Use this for initialization
	void Start () {
        hiscoretext.text = "HI SCORE:\nWave " + TowerManager.WAVEHISCORE;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Jump"))
        {
            SceneManager.LoadScene(1);
        }
	}
}
