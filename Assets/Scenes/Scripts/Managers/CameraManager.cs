using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager Instance { get; private set; }
    public CinemachineVirtualCamera cineCamera;

    //Shake
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private float shakeTimer = 0;
    // private float shakeTime = 0;
    private float shakeIntensity = 0;

    void Awake()
    {
	if(Instance != null) DestroyImmediate(this);
        Instance = this;

        cinemachineBasicMultiChannelPerlin = cineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
	    shakeTimer -= Time.deltaTime;
            if(shakeTimer <= 0)
		cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        }
    }

    public void ScreenShake(float intensity, float time) {
        shakeTimer = time;
        shakeIntensity = intensity;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
    }

    public void StopShake()
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }

}
