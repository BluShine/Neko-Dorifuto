using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

    Rigidbody body;

    public float gravity = 10;
    public float topSpeed = 50;
    public float drag = 1;
    public AnimationCurve accelCurve;
    public float accelForce = 20;
    public float energyLoss = 5;
    public float brakeForce = 40;
    public float tractionForce = 20;
    public float steeringSpeed = 10;
    public float steeringAccel = 10;
    public AnimationCurve steeringTopSpeedCurve;
    public float steeringTilt = 20;
    public float targetSuspension = .5f;
    public float suspensionForce = 20;
    public float suspensionUprightForce = 1;
    public float airRotationDrag = 1;

    HoverPIDController hoverPID;
    public HoverPIDProperties hoverPIDProperties;

    public LayerMask raycastMask;
    static float RAYRADIUS = .3f;
    static float RAYLENGTH = 1;

    private bool isColliding = false;

    RaceManager manager;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
        hoverPID = new HoverPIDController(hoverPIDProperties);
        manager = FindObjectOfType<RaceManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
    }

    private void FixedUpdate()
    {
        bool forwardInput = Input.GetButton("Jump") || Input.GetAxis("Vertical") > 0;
        bool backwardInput = !forwardInput && (Input.GetButton("Fire1") || Input.GetAxis("Vertical") < 0);

        bool onGround = false;
        Vector3 relativeVel = transform.InverseTransformDirection(body.velocity);
        //suspension
        #region suspension forces
        //gravity
        body.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        //hover force
        Vector3 upwardsDirection = transform.up;
        RaycastHit rayhit = new RaycastHit();
        Vector3 rayorigin = transform.position + transform.up * (RAYRADIUS + .1f);
        Ray ray = new Ray(rayorigin, -transform.up);
        if (Physics.SphereCast(ray, RAYRADIUS, out rayhit, RAYLENGTH, raycastMask))
        {
            float compression = Vector3.Distance(rayorigin, rayhit.point);
            float error = targetSuspension - compression;
            float upwardsForce = hoverPID.updatePosition(error, Time.fixedDeltaTime) * suspensionForce;
            body.AddForce(transform.up * upwardsForce, ForceMode.Acceleration);
            upwardsDirection = rayhit.normal;
            onGround = true;
        }
        //keep upright forces
        if (isColliding && transform.up.y < 0)
        {
            upwardsDirection = Vector3.up;
            body.AddTorque(hoverPID.updateRotation(transform, upwardsDirection, Time.fixedDeltaTime) * 50, ForceMode.Acceleration);

        }
        else if(onGround)
        {
            body.AddTorque(hoverPID.updateRotation(transform, upwardsDirection, Time.fixedDeltaTime) * suspensionUprightForce, ForceMode.Acceleration);
        } else
        {
            body.AddTorque(-body.angularVelocity.normalized * airRotationDrag, ForceMode.Acceleration);
        }
        #endregion
        //acceleration
        #region acceleration forces
        //drag when surpassing top speed in forward or reverse
        if(relativeVel.z > -topSpeed)
        {
            float vDrag = (relativeVel.z + topSpeed) * drag;
            body.AddForce(-transform.forward * drag, ForceMode.Acceleration);
        } 
        else if (relativeVel.z > topSpeed)
        {
            float vDrag = (relativeVel.z - topSpeed) * drag;
            body.AddForce(-transform.forward * drag, ForceMode.Acceleration);
        }
        //acceleration
        if (onGround)
        {
            float accelOutput = accelForce * accelCurve.Evaluate(Mathf.Abs(relativeVel.z) / topSpeed);
            if (forwardInput && relativeVel.z < topSpeed)
            {
                //if we are close to top speed, set velocity to top speed.
                if (accelOutput * Time.fixedDeltaTime + relativeVel.z > topSpeed)
                {
                    body.AddForce(transform.forward * (relativeVel.z - topSpeed), ForceMode.VelocityChange);
                }
                else
                {
                    body.AddForce(transform.forward * accelOutput, ForceMode.Acceleration);
                }
            }
            else if (backwardInput && relativeVel.z > -topSpeed * .5f)
            {
                //if we are close to top speed, set velocity to top speed.
                if (-accelOutput * Time.fixedDeltaTime + relativeVel.z < -topSpeed)
                {
                    body.AddForce(-transform.forward * (relativeVel.z + topSpeed * .5f), ForceMode.VelocityChange);
                }
                else
                {
                    body.AddForce(-transform.forward * accelOutput * .5f, ForceMode.Acceleration);
                }
            }
        }
        #endregion
        //steering
        #region steering forces
        if (onGround) {
            float turnVel = transform.InverseTransformDirection(body.angularVelocity).y;
            if (Input.GetAxis("Horizontal") == 0)
            {
                if (Mathf.Abs(turnVel) < Time.fixedDeltaTime * steeringAccel)
                {
                    body.AddRelativeTorque(new Vector3(0, -turnVel, 0), ForceMode.VelocityChange);
                } else
                {
                    body.AddRelativeTorque(new Vector3(0, -Mathf.Sign(turnVel), 0) * steeringAccel, ForceMode.Acceleration);
                }
            } else
            {
                if(Mathf.Abs(turnVel) > steeringSpeed)
                {
                    body.AddTorque(new Vector3(0, -Mathf.Sign(turnVel), 0) * steeringAccel, ForceMode.Acceleration);
                }
                else if(Input.GetAxis("Horizontal") > 0 && 
                    turnVel + steeringAccel * Time.fixedDeltaTime > steeringSpeed)
                {
                    body.AddRelativeTorque(new Vector3(0, steeringSpeed - turnVel, 0), ForceMode.VelocityChange);
                }
                else if (Input.GetAxis("Horizontal") < 0 &&
                  turnVel - steeringAccel * Time.fixedDeltaTime < -steeringSpeed)
                {
                    body.AddRelativeTorque(new Vector3(0, -steeringSpeed - turnVel, 0), ForceMode.VelocityChange);
                }
                else
                {
                    body.AddRelativeTorque(new Vector3(0, Input.GetAxis("Horizontal") * steeringAccel, 0), ForceMode.Acceleration);
                }
            }
        }
        #endregion
        //traction force
        #region traction forces
        if (onGround)
        {
            float horizontalVel = relativeVel.x;
            Vector2 skiddingNormal = new Vector2(relativeVel.x, relativeVel.z).normalized;
            if (Mathf.Abs(horizontalVel) < Time.deltaTime * tractionForce)
            {
                body.AddRelativeForce(-horizontalVel * Vector3.right, ForceMode.VelocityChange);
            }
            else
            {
                body.AddRelativeForce(Mathf.Sign(-horizontalVel) * Vector3.right * tractionForce, ForceMode.Acceleration);
            }
            //forwards friction
            float forwardVelocity = relativeVel.z;
            float skidAmount = Mathf.Min(Mathf.Abs(skiddingNormal.x), Mathf.Abs(skiddingNormal.y)) / .707107f; //how close we are to a 45-degree skid
            if (energyLoss * Time.fixedDeltaTime > Mathf.Abs(forwardVelocity))
            {
                body.AddRelativeForce(Vector3.forward * -forwardVelocity, ForceMode.VelocityChange);
            }
            else
            {
                body.AddRelativeForce(Vector3.forward * energyLoss * -Mathf.Sign(forwardVelocity), ForceMode.Acceleration);
            }
            //braking
            if (!backwardInput && !forwardInput)
            {
                Vector2 groundVel = new Vector2(relativeVel.x, relativeVel.z);
               if(groundVel.magnitude < brakeForce * Time.fixedDeltaTime)
                {
                    body.AddRelativeForce(new Vector3(-relativeVel.x, 0, -relativeVel.z), ForceMode.VelocityChange);
                } else
                {
                    groundVel.Normalize();
                    body.AddRelativeForce(new Vector3(groundVel.x, 0, groundVel.y) * -brakeForce, ForceMode.Acceleration);
                }
            }
        }
        #endregion

        isColliding = false;
    }

    public void Respawn()
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        Transform lastCheck = manager.GetLastCheckpoint();
        transform.position = lastCheck.transform.position + lastCheck.transform.up * .5f;
        transform.rotation = lastCheck.transform.rotation;
    }
}
