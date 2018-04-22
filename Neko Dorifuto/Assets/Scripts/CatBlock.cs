using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatBlock : MonoBehaviour {

    public CatProperties properties;
    [HideInInspector]
    public Vector3 target;
    Vector3 velocity;
    float hoistTimer;
    Rigidbody body;
    bool dropped = false;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.Euler(Random.Range(-10, 10), Random.Range(0, 360), Random.Range(-10, 10));
        body.angularVelocity = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * 5;
        hoistTimer = properties.hoistTime;
        body.velocity = Vector3.up * properties.hoistVelocity;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if(hoistTimer > 0)
        {
            body.angularVelocity = body.angularVelocity * Mathf.Max(0, 1 - Time.fixedDeltaTime * properties.hoistDrag);
            hoistTimer -= Time.fixedDeltaTime;
            if(hoistTimer <= 0)
            {
                body.useGravity = false;
                velocity = -(transform.position - target).normalized;
            }
        } else if (!dropped)
        {
            body.useGravity = false;
            body.velocity = velocity * properties.speed;
            Ray ray = new Ray(transform.position, velocity);
            if (Physics.SphereCast(ray, 1, 1, properties.mask))
            {
                dropped = true;
                body.useGravity = true;
            }
        }
    }
}
