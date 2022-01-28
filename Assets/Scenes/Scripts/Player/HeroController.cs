using System;
using System.Collections;
using UnityEngine;

public class HeroController : MonoBehaviour
{

    private Rigidbody _rigidBody;
    private CenterOfMass _centerOfMass;

    [SerializeField]
    private TimeManager timeManager;

    // overall bool status checks 
    public bool hasShoot = false;
    public bool isFlipComplete = false;
    public bool isOnGround = false;
    bool isFlipFailed = false;
    public bool hasFlipLanded = false;
    public bool isGameReady = false;

    // to calculate if it is upright
    // 1 is total 90 and 0.9 gives us -0.9 > 1 < 0.9 check
    [SerializeField]
    private float anglePrecision = 0.86f;
    Quaternion upRightAngle = Quaternion.Euler(0, 0, 0);
    public float waitTimeForUprightCalc = 4f;
    private float landTime;
    public int collisionCount = 0;

    // calculate rotation per flip 
    private Quaternion markedRotationX;
    private Quaternion lastRotationX;
    private float totalAngle;
    public int rotationLastFlip;


    private float maxAngularVelocity;

    // events for each flip action
    public delegate void OnFlipStart();
    public event OnFlipComplete onFlipStart;

    public delegate void OnFlipSuccess();
    public event OnFlipSuccess onFlipSuccess;

    public delegate void OnFlipFailed();
    public event OnFlipFailed onFlipFailed;

    public delegate void OnFlipComplete();
    public event OnFlipComplete onFlipComplete;

    // for debugging
    public Renderer _renderer;
    public string stringToEdit;

    


    void Awake(){
        _rigidBody = GetComponent<Rigidbody>();
        _centerOfMass = GetComponentInChildren<CenterOfMass>();
    }
    
    void Start()
    {
	_rigidBody.centerOfMass =  _centerOfMass.transform.localPosition;
        onFlipComplete += _OnFlipComplete; 

        EventManager.Instance.StartListening(GameManager.SESSION_START, ReadyForSession);
    }

    void OnDrawGizmosSelected()
    {
	_rigidBody = GetComponent<Rigidbody>();
	Debug.DrawRay(transform.TransformPoint(_rigidBody.centerOfMass), Vector3.up * 2f, Color.red, Time.deltaTime);
    }


    void ReadyForSession()
    {
        Reset();
        isGameReady = true;
    }


    void FixedUpdate()
    {
	if(!isGameReady) return;

        if(CheckIfUpRight()){
            _renderer.material.color = Color.green;
	    stringToEdit = "Upright";
        } else
        {
            _renderer.material.color = Color.red;
	    stringToEdit = "Not Upright";
        }

        if (isOnGround)
        {

            if (_rigidBody.IsSleeping()
		&& !isFlipFailed
		&& !CheckIfUpRight())
            {

                if (onFlipFailed != null) onFlipFailed();
                if (onFlipComplete != null) onFlipComplete();
                isFlipFailed = true;
            }


            if (CheckIfUpRight() && !isFlipComplete)
            {
                if (Time.time - landTime > waitTimeForUprightCalc)
                {
                    StartCoroutine(RotateToUpright(1));
                    if (onFlipSuccess != null) onFlipSuccess();
                    if (onFlipComplete != null) onFlipComplete();
                }
            }
        }

        // calculate rotation this flip
        if (hasShoot && !isFlipComplete && !isOnGround)
        {
            Quaternion newOrientation = Quaternion.Euler(transform.eulerAngles.x, 0, 0);
            var angle = Quaternion.Angle(newOrientation, lastRotationX);

            totalAngle += angle;
            if (totalAngle >= 360)
            {
                rotationLastFlip++;
                totalAngle = 0;
            }

            lastRotationX = newOrientation;
        }

        if (transform.position.y <= -10)
        {
            if (onFlipFailed != null) onFlipFailed();
        }

    }

    public void Shoot(Vector3 force, Vector3 torque)
    {
        if (hasShoot) return;
	if (!CheckIfUpRight()) return;

        transform.position = new Vector3(transform.position.x, 0.8f, transform.position.z);
        _rigidBody.isKinematic = false;
	_rigidBody.AddForce(force, ForceMode.Impulse);
	_rigidBody.AddRelativeTorque(torque);

	markedRotationX = Quaternion.Euler(transform.eulerAngles.x, 0, 0);
	lastRotationX = markedRotationX;
	totalAngle = 0;
	rotationLastFlip = 0;

	if (onFlipStart != null) onFlipStart();
	hasShoot = true;
	isFlipComplete = false;
	isFlipFailed = false;
        hasFlipLanded = false;
    }

    public void OnAirControl(Vector3 movement, Vector3 torque)
    {
	if(isOnGround) return;
        _rigidBody.AddRelativeForce(movement);
        UpdateAngularVelocity(Quaternion.identity);
    }

    void UpdateAngularVelocity(Quaternion desired) {

	var z = Vector3.Cross(transform.forward, desired * Vector3.forward);
	var y = Vector3.Cross(transform.up, desired * Vector3.up);

 
	var thetaZ = Mathf.Asin(z.magnitude);
	var thetaY = Mathf.Asin(y.magnitude);

	var dt = Time.fixedDeltaTime;
	var wZ = z.normalized * (thetaZ / dt);
	var wY = y.normalized * (thetaY / dt);

	var q = transform.rotation * _rigidBody.inertiaTensorRotation;
	var T = q * Vector3.Scale(_rigidBody.inertiaTensor, Quaternion.Inverse(q) * (wZ + wY));

	// too wobbly
	_rigidBody.AddTorque(T, ForceMode.VelocityChange);

	// stable, but still laggy
	// _rigidBody.angularVelocity = T;
	// _rigidBody.maxAngularVelocity = T.magnitude;
 
    }

    public void Reset()
    {
        transform.position = Vector3.up;
        _rigidBody.velocity = new Vector3(0f,0f,0f); 
	_rigidBody.angularVelocity = new Vector3(0f,0f,0f);
	transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
	_rigidBody.isKinematic = false;
        hasShoot = false;
        isFlipComplete = false;
    }

    private void _OnFlipComplete()
    {
        hasShoot = false;
        isFlipComplete = true;
    }

    void OnCollisionExit()
    {

        collisionCount--;
        if (collisionCount == 0)
        {
	    isOnGround = false;
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        isOnGround = true;
        collisionCount++;

        ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
        collision.GetContacts(contactPoints);
        for (int i = 0; i < contactPoints.Length; i++)
        {
            ContactPoint cp = contactPoints[i];
            if (cp.otherCollider.tag == "Obstacle")
            {
                _rigidBody.AddForce(cp.normal * 10f);
                if (onFlipFailed != null)
                {
                    onFlipFailed();
                    break;
                }
            }
        }


    }

    IEnumerator RotateToUpright(float atTime)
    {
        float percent = 0;
        float time = 0;

	_rigidBody.angularVelocity = Vector3.zero;
	_rigidBody.velocity = Vector3.zero;
	transform.rotation = Quaternion.identity;			    
	
        while(percent <= 1)
        {
            percent = time / atTime;



            time += Time.deltaTime;
            yield return null;
        }

    }

    bool CheckIfUpRight()
    {
        return Math.Abs(Quaternion.Dot(this.transform.rotation, upRightAngle)) > anglePrecision;
    }

}
