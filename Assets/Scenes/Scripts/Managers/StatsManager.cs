using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _player;
    private HeroController _heroController;

    [SerializeField]
    private int maxScoreForDistance;

    public int oneFlipScore = 10;
    public int currentScore = 0;

    private Vector3 flipStartPosition;
    private float flipStartRotationX;

    private Vector3 sessionFlipStartPosition;

    private bool inSession = false;

    public static StatsManager Instance { get; private set; }



    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
        }
        Instance = this;

        _heroController = _player.GetComponent<HeroController>();
    }

    void Start()
    {
	_heroController.onFlipStart += ScoreOnFlipStart;
        _heroController.onFlipSuccess += ScoreOnFlipSuccess;
        _heroController.onFlipFailed += ScoreOnFlipFailed;

        EventManager.Instance.StartListening(GameManager.SESSION_START, onSessionStart);
        EventManager.Instance.StartListening(GameManager.SESSION_END, onSessionEnd);
	
    }

    void Update()
    {
	if(!inSession) return;
    }

    void onSessionStart()
    {
        inSession = true;
    }

    void onSessionEnd()
    {
        inSession = false;
    }

    void ScoreOnFlipStart()
    {
        flipStartPosition = _heroController.transform.position;
        flipStartRotationX = _heroController.transform.rotation.x;
    }

    void ScoreOnFlipFailed()
    {
        if(PlayerPrefs.HasKey("highestScore")){
            if (currentScore > PlayerPrefs.GetInt("highestScore")) 
		PlayerPrefs.SetInt("highestScore", currentScore);
        } else
        {
            PlayerPrefs.SetInt("highestScore", currentScore);
        }
	currentScore = 0;
    }

    void ScoreOnFlipSuccess()
    {
        float dist = (_heroController.transform.position - flipStartPosition).magnitude;
        float multiplier = dist / 4 * _heroController.rotationLastFlip;
        currentScore += (int) (oneFlipScore * multiplier); 
    }

    public float GetDistanceCurrentSession()
    {
	return (_heroController.transform.position - sessionFlipStartPosition).magnitude;
    }


}
