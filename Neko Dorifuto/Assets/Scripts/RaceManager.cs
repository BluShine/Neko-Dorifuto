using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour {

    public float startingTime = 30;
    public float checkpointTime = 3;

    public List<RaceCheckpoint> checkpoints;
    int currentCheckpoint = 0;

    float currentTime = 0;
    public int laps = 3;
    float currentLaps;

    public TextMeshProUGUI lapText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI timeTextPanic;

    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;

    bool win = false;
    bool lose = false;

    public void HitCheckpoint()
    {
        if(currentCheckpoint == 0)
        {
            currentLaps--;
            if(currentLaps < 0 && !lose)
            {
                win = true;
            }
        }
        currentCheckpoint++;
        if(currentCheckpoint == checkpoints.Count)
        {
            currentCheckpoint = 0;
        }
        checkpoints[currentCheckpoint].Activate();
        currentTime += checkpointTime;
    }

	// Use this for initialization
	void Start () {
        checkpoints[0].Activate();
        currentTime = startingTime;
        currentLaps = laps;
        winText.enabled = false;
        loseText.enabled = false;
        timeTextPanic.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        currentTime -= Time.fixedDeltaTime;
        currentTime = Mathf.Max(0, currentTime);
        lapText.text = "Lap  " + (laps - currentLaps) + "/" + laps;
        timeText.text = "Time\n" + Mathf.Ceil(currentTime);
        timeTextPanic.text = "Time\n" + Mathf.Ceil(currentTime);
        if (currentTime < 10 && Mathf.Floor(currentTime * 5) %2 == 0)
        {
            timeTextPanic.enabled = true;
        } else
        {
            timeTextPanic.enabled = false;
        }

        if (currentTime == 0 && !win)
            lose = true;

        if (win)
        {
            winText.enabled = true;
        }
        if(lose)
        {
            loseText.enabled = true;
        }
    }
}
