using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public HeroController _heroController;
    public PlayerInputManager _playerInputManager;
    public Chaser _chaser;


    public float chasePerDistance = 0.2f;
    public float initChaseSpeed = 1f;

    public Vector3 MAXFORCE;
    public Vector3 startSessionForce;

    public float forcePerDistance = 0.05f;
    public float forcePerDistancePerTime = 0.01f;
    public float distToStartSpeedUp = 10f;


    public static LevelManager Instance;

    void Awake()
    {
	if(Instance != null) DestroyImmediate(this);
        Instance = this;

        _heroController.GetComponent<HeroController>();
    }

    void Start()
    {
        startSessionForce = _playerInputManager.maxForce;

        EventManager.Instance.StartListening(GameManager.SESSION_START, onSessionStart);
        EventManager.Instance.StartListening(GameManager.SESSION_END, onSessionEnd);
    }

    void Update()
    {
        int distPer = Mathf.RoundToInt(StatsManager.Instance.GetDistanceCurrentSession()) / 10;

        _playerInputManager.maxForce.z = Mathf.Clamp(startSessionForce.z + forcePerDistance * distPer, startSessionForce.z, MAXFORCE.z);
        _playerInputManager.maxForce.y = Mathf.Clamp(startSessionForce.y + forcePerDistancePerTime * distPer, startSessionForce.y, MAXFORCE.y);

        _chaser.chaseSpeed = Mathf.Clamp(initChaseSpeed + chasePerDistance * distPer, 2f, _chaser.MAX_CHASE);
    }

    void onSessionStart()
    {
        LevelReset();
    }

    public void LevelReset()
    {
	_playerInputManager.maxForce = startSessionForce;
        _chaser.chaseSpeed = initChaseSpeed;
    }

    void onSessionEnd()
    {
    }

}
