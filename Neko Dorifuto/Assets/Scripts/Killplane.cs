using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killplane : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Car car = collision.gameObject.GetComponent<Car>();
        if (car != null)
        {
            car.Respawn();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        CatBlock cat = collision.gameObject.GetComponent<CatBlock>();
        if (cat != null)
        {
            cat.killTimer -= Time.fixedDeltaTime;
            if(cat.killTimer < 0)
            {
                Destroy(cat.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Car car = other.gameObject.GetComponent<Car>();
        if(car != null)
        {
            car.Respawn();
        } else
        {
            Destroy(other.gameObject);
        }
    }
}
