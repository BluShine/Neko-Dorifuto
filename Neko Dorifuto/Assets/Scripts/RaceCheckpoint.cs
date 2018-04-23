using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCheckpoint : MonoBehaviour {

    public GameObject checkpointGraphics;

    RaceManager manager;
    bool activated = false;

	// Use this for initialization
	void Awake () {
        manager = FindObjectOfType<RaceManager>();
        checkpointGraphics.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        activated = true;
        checkpointGraphics.SetActive(true);
    }

    public void Deactivate()
    {
        activated = false;
        checkpointGraphics.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(activated && other.gameObject.layer == 9)
        {
            Deactivate();
            manager.HitCheckpoint();
        }
    }
}
