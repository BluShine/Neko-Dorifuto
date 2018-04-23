using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    bool raceActive = true;
    bool win = false;
    bool lose = false;

    public BezierCurve track;

    Car car;
    Vector3 carStart;
    Quaternion carStartRot;

    float endTimer = 0;

    TowerManager towerManager;

    public RandomSound startEndSounds;

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
        startEndSounds.PlaySound();
    }

    public void endRace()
    {
        foreach (RaceCheckpoint c in checkpoints)
        {
            c.Deactivate();
        }
        winText.enabled = false;
        loseText.enabled = false;
        timeTextPanic.enabled = false;
        lapText.enabled = false;
        timeText.enabled = false;
        car.transform.position = carStart;
        car.transform.rotation = carStartRot;
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        car.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        car.engineSound.Stop();
        raceActive = false;
        startEndSounds.PlaySound();
    }

    public void startRace()
    {
        car.transform.position = carStart;
        car.transform.rotation = carStartRot;
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        car.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        car.engineSound.Play();
        foreach (RaceCheckpoint c in checkpoints)
        {
            c.Deactivate();
        }
        checkpoints[0].Activate();
        currentTime = startingTime;
        currentLaps = laps - 1;
        winText.enabled = false;
        loseText.enabled = false;
        timeTextPanic.enabled = false;
        lapText.enabled = true;
        timeText.enabled = true;
        raceActive = true;
        win = false;
        lose = false;
        endTimer = 0;

        startEndSounds.PlaySound();
    }

	// Use this for initialization
	void Start () {
        car = FindObjectOfType<Car>();
        carStart = car.transform.position;
        carStartRot = car.transform.rotation;
        towerManager = FindObjectOfType<TowerManager>();
        startRace();
        currentLaps = 1;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if (raceActive)
        {
            if (!win)
            {
                currentTime -= Time.fixedDeltaTime;
            }
            currentTime = Mathf.Max(0, currentTime);
            lapText.text = "Lap  " + (laps - currentLaps) + "/" + laps;
            if (win)
            {
                timeText.text = "RACE COMPLETE: $10\nBONUS: $" + Mathf.Ceil(currentTime);
            }
            else
            {
                timeText.text = "Time\n" + Mathf.Ceil(currentTime);
                timeTextPanic.text = "Time\n" + Mathf.Ceil(currentTime);
            }
            if (!win && currentTime < 10 && Mathf.Floor(currentTime * 5) % 2 == 0)
            {
                timeTextPanic.enabled = true;
            }
            else
            {
                timeTextPanic.enabled = false;
            }

            if (currentTime == 0 && !win)
                lose = true;

            if (win)
            {
                winText.enabled = true;
                endTimer += Time.fixedDeltaTime;
                if (endTimer > 5)
                {
                    //next wave
                    endRace();
                    towerManager.money += 10 + Mathf.CeilToInt(currentTime);
                    towerManager.ActivateTowerPhase();
                }
            }
            if (lose)
            {
                loseText.enabled = true;
                endTimer += Time.fixedDeltaTime;
                if (endTimer > 5)
                {
                    //restart
                    SceneManager.LoadScene(0);
                }
            }
        }
    }

    public Transform GetLastCheckpoint()
    {
        return checkpoints[(currentCheckpoint - 1 + checkpoints.Count) % checkpoints.Count].transform;
    }
}
