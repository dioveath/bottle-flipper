using UnityEngine;

public class Chaser : MonoBehaviour
{

    public GameObject _chaseTarget;
    public GameObject _camera;

    // only on z-axis
    public float MAX_CHASE = 5f;
    public float chaseSpeed = 2f;

    public float initZDistToTarget = 6f;

    private bool isChasing = false;

    void Start()
    {
        EventManager.Instance.StartListening(GameManager.SESSION_START, onSessionStart);
        EventManager.Instance.StartListening(GameManager.SESSION_END, onSessionEnd);

        initZDistToTarget = _chaseTarget.transform.position.z - _camera.transform.position.z;

        ResetPosition();
    }

    void onSessionStart()
    {
        isChasing = true;
        ResetPosition();
    }

    void onSessionEnd()
    {
        isChasing = false;
    }

    void ResetPosition()
    {
        Vector3 newResetPosition = new Vector3(0, 0, -initZDistToTarget);
        transform.position = newResetPosition;
    }

    void Update()
    {
	if(!isChasing) return;
        int distToPlayer = Mathf.RoundToInt(StatsManager.Instance.GetDistanceCurrentSession()) / 10;

        float toTargetZ = _chaseTarget.transform.position.z - transform.position.z;
 
        if (Mathf.Abs(toTargetZ) > Mathf.Abs(initZDistToTarget))
        {
            transform.position = new Vector3(0, 0, _chaseTarget.transform.position.z - initZDistToTarget);
        }

	Vector3 chaseDir = (_chaseTarget.transform.position - transform.position).normalized;
	transform.Translate(chaseDir * chaseSpeed * Time.deltaTime);

    }

}
