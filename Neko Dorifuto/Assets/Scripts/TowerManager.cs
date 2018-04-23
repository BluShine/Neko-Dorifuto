using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerManager : MonoBehaviour {

    public GameObject spawnerPrefab;
    public GameObject smalltowerPrefab;
    public GameObject largetowerPrefab;

    public GameObject spawnAreas;
    BoxCollider[] spawnBoxes;

    RaceManager raceManager;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI moneyDisplay;
    public TextMeshProUGUI selectionText;
    public TextMeshPro instructionText;
    public TextMeshProUGUI countdown;

    public float fastMoveSpeed = 200;
    public float slowMoveSpeed = 50;
    static float maxMove = 200;

    public int money = 10;

    int wave = 0;

    float countdownTimer = 3;

    public LayerMask castMask;

    public Camera topdownCam;

    static int WAVEHISCORE = 0;

    bool phaseActive = false;

	// Use this for initialization
	void Start () {
        spawnBoxes = spawnAreas.GetComponentsInChildren<BoxCollider>();
        //PlaceSpawners(10);
        raceManager = FindObjectOfType<RaceManager>();
        topdownCam.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if (!phaseActive)
            return;

        if(money < 5)
        {
            countdown.enabled = true;
            countdown.text = "READY\n" + Mathf.Ceil(countdownTimer);
            countdownTimer -= Time.fixedDeltaTime;
            if(countdownTimer <= 0)
            {
                DeactivateTowerPhase();
            }
        } else
        {
            bool selecting = false;
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit rayhit;
            if(Physics.SphereCast(ray, .7f, out rayhit, 100, castMask))
            {
                selecting = true;
                CatSpawner spawner = rayhit.collider.gameObject.GetComponent<CatSpawner>();
                Tower tower = rayhit.collider.gameObject.GetComponentInChildren<Tower>();
                if (spawner != null)
                {
                    selectionText.text = "Cat crate. Cats will spawn here!";
                }
                else if (tower != null)
                {
                    if(tower.upgraded)
                    {
                        selectionText.text = "X";
                    } else
                    {
                        selectionText.text = "Upgrade $7";
                        if(money >= 7)
                        {
                            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
                            {
                                //upgrade tower
                                money -= 7;
                                GameObject upTower = GameObject.Instantiate(largetowerPrefab);
                                upTower.transform.position = rayhit.collider.gameObject.transform.position;
                                upTower.transform.rotation = rayhit.collider.gameObject.transform.rotation;
                                Destroy(rayhit.collider.gameObject);
                            }
                        }
                    }
                } else if(rayhit.collider.gameObject.layer == 12)
                {
                    selectionText.text = "Build tower $5";
                    if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
                    {
                        money -= 5;
                        GameObject placedTower = GameObject.Instantiate(smalltowerPrefab);
                        placedTower.transform.position = rayhit.point;
                        placedTower.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
                    }
                }
                else
                {
                    selecting = false;
                    selectionText.text = "X";
                }
            }
            else
            {
                selectionText.text = "X";
            }

            if(selecting)
            {
                transform.position += Time.fixedDeltaTime * slowMoveSpeed * new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            } else
            {
                transform.position += Time.fixedDeltaTime * fastMoveSpeed * new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            }
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -maxMove, maxMove), transform.position.y, Mathf.Clamp(transform.position.z, -maxMove, maxMove));
        }
    }

    public void ActivateTowerPhase()
    {
        wave++;
        WAVEHISCORE = Mathf.Max(WAVEHISCORE, wave);
        PlaceSpawners(wave * 2 + 2);
        transform.position = new Vector3(0, transform.position.y, 0);
        titleText.enabled = true;
        moneyDisplay.enabled = true;
        instructionText.enabled = true;
        selectionText.enabled = true;
        topdownCam.enabled = true;
        phaseActive = true;
        if(wave == 1)
        {
            HintCube cube = FindObjectOfType<HintCube>();
            if (cube != null)
                cube.renderer.enabled = true;
        }
    }

    public void DeactivateTowerPhase()
    {
        foreach(CatSpawner c in FindObjectsOfType<CatSpawner>())
        {
            c.refresh();
            c.paused = false;
        }
        titleText.enabled = false;
        moneyDisplay.enabled = false;
        instructionText.enabled = false;
        countdown.enabled = false;
        selectionText.enabled = false;
        topdownCam.enabled = false;
        phaseActive = false;
        if (wave == 1)
        {
            HintCube cube = FindObjectOfType<HintCube>();
            if (cube != null)
                cube.renderer.enabled = false;
        }
        raceManager.startRace();
    }

    public void PlaceSpawners(int count)
    {
        for(int i = 0; i < count; i++)
        {
            BoxCollider sBox = spawnBoxes[Random.Range(0, spawnBoxes.Length)];
            Vector3 randPos = new Vector3(Random.Range(-.5f, .5f), 0, Random.Range(-.5f, .5f));
            randPos.Scale(sBox.size);
            Vector3 spawnPos = sBox.transform.position + randPos;
            GameObject spawner = GameObject.Instantiate(spawnerPrefab);
            spawner.transform.position = spawnPos;
            spawner.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
        }
    }
}
