using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlayerInputManager : MonoBehaviour
{

    [SerializeField]
    private Vector2 maxSwipe;
    [SerializeField]
    private float torqueForce = 30;
    public Vector3 maxForce;

    [SerializeField]
    private float swipeDistanceThresholdPercent = 0.05f;
    [SerializeField]
    private float minSwipeTime = 0.1f;
    [SerializeField]
    private float maxSwipeTime = 0.5f;
    [SerializeField]
    private Vector2 swipeDependencyPercentThreshold;

    public HeroController heroController;
    public bool isControlDisabled = true;


    float startInputTime;
    Vector2 startPosition;

    bool timeStopped = false;
    bool isCalculateLand = false;
    float startCalculateLandTime;
    LandData currentLandData;
    public float someMagicNumber = 0.9f;

    Camera camera;

    public bool isDebug = false;
    

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Awake()
    {
        camera = Camera.main;
        float convertedMaxSwipeX = maxSwipe.x / 1080 * Screen.width;
        float convertedMaxSwipeY = maxSwipe.y / 1920 * Screen.height;
        maxSwipe = new Vector2(convertedMaxSwipeX, convertedMaxSwipeY);
    }


    void Update()
    {

        if (isDebug)
        {
            if (isCalculateLand)
            {
                if (Time.time - startCalculateLandTime > currentLandData.landTime)
                {
                    // FindObjectOfType<TimeManager>().SlowDownTimeFor(0, 5f);
                    timeStopped = true;
                    isCalculateLand = false;
                }
                DrawPath();
            }

            if (timeStopped)
            {
                if (Touch.activeTouches.Count > 0)
                {
                    // FindObjectOfType<TimeManager>().ResetTime();
                    timeStopped = false;
                }
            }
        }

        if(isControlDisabled) return;

        if (Touch.activeTouches.Count > 0)
        {
            var touch = Touch.activeTouches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchStarted(touch);
                    break;
                case TouchPhase.Moved:
                    OnTouchMoved(touch);
                    break;
                case TouchPhase.Ended:
                    OnTouchEnded(touch);
                    break;
            }
        }
    }


    void OnTouchStarted(Touch touch)
    {
        startPosition = touch.screenPosition;
	startInputTime = Time.time;
    }


    void OnTouchMoved(Touch touch)
    {
        // if (heroController.hasShoot && !heroController.isOnGround)
        // {
            Vector2 swippingVector = touch.delta * 0.5f;
            Vector3 onAirMove = new Vector3(swippingVector.x, Mathf.Abs(swippingVector.y * 0.1f), 0);
            Vector3 onAirTorque = new Vector3(swippingVector.x, 0, 0);
            // heroController.OnAirControl(onAirMove, onAirTorque);
        // }
    }


    void OnTouchEnded(Touch touch)
    {
	if(!heroController.isOnGround) return;
	if(heroController.hasShoot) return;

        Vector2 lastPosition = touch.screenPosition;

	Vector2 swipeVector = lastPosition - startPosition;
	float intervalTime = Time.time - startInputTime;

	if (intervalTime <= maxSwipeTime &&
	    intervalTime >= minSwipeTime && 
	    swipeVector.magnitude > (maxSwipe.y * swipeDistanceThresholdPercent))
	{
	    float timePercent = Mathf.Clamp((intervalTime / (maxSwipeTime - minSwipeTime)), 0.4f, 1f);
	    float swipeXPercent = Mathf.Clamp(swipeVector.x / maxSwipe.x, -0.8f, 0.8f);
	    float swipeYPercent = Mathf.Clamp(swipeVector.y / maxSwipe.y, 0.8f, 1f);

            // less time more multiplier ;; need to cap ;;
            // player don't need to go blazing fast
            float timePercentMultiplier = 1 - timePercent;
            Vector3 forceVector = new Vector3((swipeXPercent * timePercentMultiplier) * maxForce.x,
                          (swipeYPercent * timePercentMultiplier) * maxForce.y,
                          maxForce.z);

            Vector3 torqueVector = Vector3.left * swipeYPercent * torqueForce;
			
	    heroController.Shoot(forceVector, torqueVector);

            if (isDebug)
            {
                isCalculateLand = true;
                startCalculateLandTime = Time.time;
                currentLandData = CalculateLand(forceVector);
            }

        }	
    }




    public LandData CalculateLand(Vector3 forceVector)
    {
        Vector3 finalLandPosition;
        Rigidbody rigidBody = heroController.GetComponent<Rigidbody>();
        Transform heroTransform = heroController.transform;

	// ?? due to gravity acceleration?? too high
        float u = forceVector.y;
        float g = Physics.gravity.y;

        float h = -u * u / 2f * g;
        float t1 = Mathf.Sqrt(-(2 * h / g));

	// X-axis
        float P = forceVector.z; 
        float M = rigidBody.mass;
        float U = P / M;
        float X = (U * (t1 * 2));

        Vector3 launchVelocity = new Vector3(forceVector.x, u, U);

        finalLandPosition = new Vector3(0, 0, X);

        // Debug.DrawLine(heroTransform.position, finalLandPosition + heroTransform.position, Color.red, t1*2);
        Debug.DrawLine(heroTransform.position, new Vector3(0, h, 0) + heroTransform.position, Color.blue, t1*2);
	
        return new LandData(heroTransform.position, finalLandPosition, t1, t1 * 2, launchVelocity);
    }


    public void DrawPath()
    {
        Vector3 initialPosition = currentLandData.initialPosition;
        Vector3 previousDrawPoint = initialPosition;

        int resolution = 10;
        float g = Physics.gravity.y;

        // Rigidbody heroBody = heroController.GetComponent<Rigidbody>();
        // Vector3 testLaunch = currentLandData.launchVelocity + Vector3.up * currentLandData.launchVelocity.y / heroBody.mass * someMagicNumber;

	Debug.Log("FinalLandPosition: " + currentLandData.finalLandPosition);

        for (int i = 1;i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * currentLandData.landTime;
            Vector3 displacement = currentLandData.launchVelocity * simulationTime + Vector3.up * g * simulationTime * simulationTime / 2f;
            // Vector3 displacement = Vector3.forward * currentLandData.launchVelocity.z * simulationTime;

            Debug.Log(i + ": " + displacement);

            Vector3 drawPoint = initialPosition + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }

    }


    public struct LandData
    {
        public Vector3 launchVelocity;
        public Vector3 initialPosition;
        public Vector3 finalLandPosition;
        public float timeToHeight;
        public float landTime;

        public LandData(Vector3 initialPosition, Vector3 finalLandPosition, float timeToHeight, float landTime, Vector3 launchVelocity)
        {
            this.initialPosition = initialPosition;
            this.finalLandPosition = finalLandPosition;
            this.timeToHeight = timeToHeight;
            this.landTime = landTime;
            this.launchVelocity = launchVelocity;
        }
    }

}


