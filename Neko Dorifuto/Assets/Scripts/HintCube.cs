using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintCube : MonoBehaviour {

    [HideInInspector]
    public MeshRenderer renderer;

    float spin = 0;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<MeshRenderer>();
        renderer.enabled = false;
	}

    private void FixedUpdate()
    {
        spin += Time.deltaTime * 200;
        spin = spin % 360;
        transform.rotation = Quaternion.Euler(0, spin, 0); 
    }
}
