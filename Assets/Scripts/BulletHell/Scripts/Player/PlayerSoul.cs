using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSoul : MonoBehaviour 
{
    public Transform timerTextTrans;
    public Transform revivalCircleTrans;

    float mTimer;
    Text mTimerText;
    bool mStopTime = false;

    SpriteRenderer sr;
    PlayerController mPlayerController;

    void Start()
    {
        mTimerText = timerTextTrans.GetComponent<Text>();
        sr = GetComponent<SpriteRenderer>();
        mPlayerController = GetComponentInParent<PlayerController>();
    }
	
	void Update () 
    {
        if (sr.enabled && !mStopTime)
        {
            mTimer -= Time.deltaTime;
            mTimerText.text = ((int)mTimer).ToString();

            if (mTimer <= 0)
            {
                mPlayerController.ReviveSelf();
                Deactivate();
                Debug.Log("Ended");
            }
        }
	}

    public void Activate()
    {
        sr.enabled = true;
        ResetDeadTimeToDefault();
        revivalCircleTrans.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        sr.enabled = false;
        timerTextTrans.gameObject.SetActive(false);
        revivalCircleTrans.gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        mStopTime = false;
    }

    public void StopTimer()
    {
        mStopTime = true;
    }

    void ResetDeadTimeToDefault()
    {
        mTimer = GameManager.sSingleton.plySoulTime;
        mTimerText.text = mTimer.ToString();
        timerTextTrans.position = transform.GetChild(0).position;
        timerTextTrans.gameObject.SetActive(true);
    }
}
