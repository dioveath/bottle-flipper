using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    [SerializeField]
    private float slowDownTime = 2f;
    private bool toResetTime = false;

    void Update()
    {
        if (toResetTime)
        {
            Time.timeScale += (1/slowDownTime) * Time.unscaledDeltaTime;
            if (Time.timeScale >= 1)
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = 0.02f;
                toResetTime = false;
            }	    
        }
    }


    public void SlowDownTimeFor(float timeScale, float time = 0.4f)
    {
	Time.timeScale = timeScale;
	Time.fixedDeltaTime = timeScale * 0.02f; // to make it run 50 times per second whatever the case
	
        slowDownTime = time;
        toResetTime = true;
    }

    public void ResetTime()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

}
