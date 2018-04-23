using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnStart : MonoBehaviour {

    public AudioSource sound;

    public int intervals = 5;

    static float[] pentatonic = { 1, 9f / 8f, 5f / 4f, 3f / 2f, 5f / 3f, 2f, 2 * 9f / 8f, 2 * 5f / 4f, 2 * 3f / 2f, 2 * 5f / 3f, 4 };

	// Use this for initialization
	void Start () {
        float tone = pentatonic[Random.Range(0, intervals)];
        sound.pitch = tone;
        sound.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
