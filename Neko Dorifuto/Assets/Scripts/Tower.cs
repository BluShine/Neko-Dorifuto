using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public GameObject projectile;
    public float fireRate = 4;
    public float projectileVelocity = 25;
    float cooldownTimer = 0;

    CatBlock nearestTarget = null;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        cooldownTimer -= Time.fixedDeltaTime;
        cooldownTimer = Mathf.Max(0, cooldownTimer);
        if(cooldownTimer == 0 && nearestTarget != null)
        {
            //calculate our aim direction
            Vector3 targetPos = nearestTarget.transform.position;
            Vector3 targetVel = nearestTarget.GetComponent<Rigidbody>().velocity;
            Vector3 toTarget = targetPos - transform.position;
            float a = Vector3.Dot(targetVel, targetVel) - projectileVelocity * projectileVelocity;
            float b = 2 * Vector3.Dot(targetVel, toTarget);
            float c = Vector3.Dot(toTarget, toTarget);

            float p = -b / (2 * a);
            float q = Mathf.Sqrt((b * b) - 4 * a * c) / (2 * a);

            float t1 = p - q;
            float t2 = p + q;
            float t;

            if(t1 > t2 && t2 > 0)
            {
                t = t2;
            } else
            {
                t = t1;
            }

            Vector3 targetSpot = targetPos + targetVel * t;
            Vector3 aimDir = targetSpot - transform.position;
            aimDir.Normalize();
            //fire the projectile!
            GameObject fireProjectile = GameObject.Instantiate(projectile);
            fireProjectile.GetComponent<Rigidbody>().velocity = aimDir * projectileVelocity;
            fireProjectile.transform.position = transform.position;
            //cleanup
            cooldownTimer = fireRate;
            nearestTarget = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CatBlock cat = other.gameObject.GetComponent<CatBlock>();
        if (cat != null && cat.readyToTarget())
        {
            if(nearestTarget == null)
            {
                nearestTarget = cat;
            } else
            {
                if(Vector3.Distance(transform.position, nearestTarget.transform.position) > Vector3.Distance(transform.position, cat.transform.position))
                {
                    nearestTarget = cat;
                }
            }
        }
    }
}
