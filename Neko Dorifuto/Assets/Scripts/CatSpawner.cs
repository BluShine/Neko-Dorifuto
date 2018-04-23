using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSpawner : MonoBehaviour {

    BezierCurve track;
    public float trackRadius = 3;

    public int attempts = 10;

    public int ammo = 5;
    int currentAmmo;
    public float delay = 5;
    float delayTimer = 0;

    public GameObject catPrefab;

    public bool paused = true;

	// Use this for initialization
	void Start () {
        delayTimer = delay + Random.value;
        track = FindObjectOfType<RaceManager>().track;
        currentAmmo = ammo;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void refresh()
    {
        currentAmmo = ammo;
        delayTimer = delay + Random.value;
    }

    private void FixedUpdate()
    {
        if(!paused && currentAmmo > 0)
        {
            delayTimer -= Time.fixedDeltaTime;
            if (delayTimer <= 0)
            {
                delayTimer = delay + Random.value;
                currentAmmo--;
                //find a target
                Vector3 bestAttempt = track.GetRandomPoint();
                for(int i = 0; i < attempts; i++)
                {
                    Vector3 testPoint = track.GetRandomPoint();
                    if(Vector3.Distance(transform.position, testPoint) < Vector3.Distance(transform.position, bestAttempt))
                    {
                        bestAttempt = testPoint;
                    }
                }
                //spawn a cat
                GameObject cat = GameObject.Instantiate(catPrefab);
                CatBlock cBlock = cat.GetComponent<CatBlock>();
                cBlock.target = bestAttempt;
                cBlock.transform.position = transform.position;
            }
        }
    }
}
