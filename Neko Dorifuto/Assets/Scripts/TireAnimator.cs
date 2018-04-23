using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireAnimator : MonoBehaviour {

    public Transform tire;
    public float suspensionLength = .2f;
    public float radius = .05f;
    Vector3 startPos;

	// Use this for initialization
	void Start () {
        startPos = tire.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        RaycastHit rayHit = new RaycastHit();
        Ray ray = new Ray(transform.TransformPoint(startPos), -transform.up);
        if(Physics.SphereCast(ray, radius, out rayHit, suspensionLength))
        {
            tire.position = rayHit.point + transform.up * radius;
        }
        else
        {
            tire.position = transform.TransformPoint(startPos) - transform.up * suspensionLength;
        }
    }
}
