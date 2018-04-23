using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour {

    AudioSource[] sounds;

    public void PlaySound()
    {
        sounds[Random.Range(0, sounds.Length)].Play();
    }

    private void Awake()
    {
        sounds = GetComponents<AudioSource>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
