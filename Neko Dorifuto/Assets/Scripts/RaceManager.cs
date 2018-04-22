using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour {

    public float targetTime = 60;

    public List<RaceCheckpoint> checkpoints;
    int currentCheckpoint = 0;

    public float currentLapTime = 0;
    public int laps = 0;

    public void HitCheckpoint()
    {
        currentCheckpoint++;
        if(currentCheckpoint == checkpoints.Count)
        {
            currentLapTime = 0;
            currentCheckpoint = 0;
            laps++;
        }
        checkpoints[currentCheckpoint].Activate();
    }

	// Use this for initialization
	void Start () {
        checkpoints[0].Activate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        currentLapTime += Time.fixedDeltaTime;
    }
}
