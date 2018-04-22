using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HoverPIDProperties", menuName = "BluShine/HoverPID", order = 1)]
public class HoverPIDProperties : ScriptableObject {

    public float hoverP = 1;
    public float hoverI = .1f;
    public float hoverD = .1f;

    public float rotationP = 1;
    public float rotationI = .1f;
    public float rotationD = .1f;
	
}

public class HoverPIDController
{
    public HoverPIDProperties properties;
    float posIntegrator = 0;
    float posLastError = 0;
    Vector2 rotIntegrator = Vector2.zero;
    Vector2 rotLastError = Vector2.zero;

    public HoverPIDController(HoverPIDProperties prop)
    {
        properties = prop;
    }

    //returns force
    public float updatePosition(float error, float deltaTime, float minForce = 0, float maxForce = 1)
    {
        //i
        posIntegrator += error * deltaTime;
        posIntegrator = Mathf.Clamp(posIntegrator, minForce, maxForce);
        //d
        float deriv = (error - posLastError) / deltaTime;
        posLastError = error;
        //force
        float force = error * properties.hoverP + posIntegrator * properties.hoverI + deriv * properties.hoverD;
        force = Mathf.Clamp(force, minForce, maxForce);
        return force;
    }

    //returns torque force vector in local space
    public Vector3 updateRotation(Transform transf, Vector3 upDir, float deltaTime)
    {
        //calculate pitch and roll error in local space
        Vector3 error = upDir - transf.up;
        Vector3 errorLocal = transf.InverseTransformDirection(error);
        Vector2 errorXZ = new Vector2(errorLocal.z, -errorLocal.x);//axes seem weird, don't worry about it :P
        //i
        rotIntegrator += errorXZ;
        rotIntegrator = new Vector2(Mathf.Clamp(rotIntegrator.x, -1, 1), Mathf.Clamp(rotIntegrator.y, -1, 1));
        //d
        Vector2 deriv = (errorXZ - rotLastError) / deltaTime;
        rotLastError = errorXZ + Vector2.zero;
        //force
        Vector2 forceXZ = errorXZ * properties.rotationP + rotIntegrator * properties.rotationI + deriv * properties.rotationD;
        forceXZ = new Vector2(Mathf.Clamp(forceXZ.x, -1, 1), Mathf.Clamp(forceXZ.y, -1, 1));
        Vector3 worldForce = transf.TransformDirection(new Vector3(forceXZ.x, 0, forceXZ.y));
        return worldForce;
        /*
        //p
        float error = Vector3.Angle(transf.TransformDirection(upDir), Vector3.up);//do this in local space for better behavior
        //i
        rotIntegrator += error * deltaTime;
        rotIntegrator = Mathf.Clamp(rotIntegrator, -maxForce, maxForce);
        //d
        float deriv = (error - rotLastError) / deltaTime;
        rotLastError = error;
        //force
        float force = error * properties.hoverP + rotIntegrator * properties.hoverI + deriv * properties.hoverD;
        force = Mathf.Clamp(force, -maxForce, maxForce);
        //cross product is the axis of rotation from our current vector to our target vector
        Vector3 cross = Vector3.Cross(transf.TransformDirection(upDir), Vector3.up);
        return cross * force;*/
    }
}