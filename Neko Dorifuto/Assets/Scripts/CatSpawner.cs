using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSpawner : MonoBehaviour {

    public BezierCurve track;
    public float trackRadius = 3;

    public int attempts = 10;

    public int ammo = 5;
    public float delay = 5;
    float delayTimer = 0;

    public GameObject catPrefab;

	// Use this for initialization
	void Start () {
        delayTimer = delay;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if(ammo > 0)
        {
            delayTimer -= Time.fixedDeltaTime;
            if (delayTimer <= 0)
            {
                delayTimer = delay;
                ammo--;
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
