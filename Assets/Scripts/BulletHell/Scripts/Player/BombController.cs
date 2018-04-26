using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour 
{
    public enum Type
    {
        NONE = 0, 
        TIME_STOP,
		LASER
    }
    public Type type = Type.NONE;

    public Transform potraitTrans;
	public Transform dualLinkLaserTrans;

    float duration = 3.0f;
    float returnDefaultSpdDur = 1.0f;

    float mTimeScale = 0.05f;
    float mFixedDeltaTime = 0.0001f, mSavedFixedDT;
    bool mIsUsingBomb = false;

    PlayerController mPlayerController;

    void Start()
    {
        mPlayerController = GetComponent<PlayerController>();

        mSavedFixedDT = Time.fixedDeltaTime;

		if (type == Type.TIME_STOP) 
		{
			duration = BombManager.sSingleton.bombTimeStopDur;
			returnDefaultSpdDur = BombManager.sSingleton.bombReturnSpdDur;
		}
		else if(type == Type.LASER)
		{
		}
    }

    public void ActivateBomb()
    {
		if (mIsUsingBomb) return;

		mIsUsingBomb = true;
        if (type == Type.TIME_STOP)
        {
			BombManager.sSingleton.isTimeStopBomb = true;

            Time.timeScale = mTimeScale;
            Time.fixedDeltaTime = mFixedDeltaTime;
            StartCoroutine(TimeStopSequence(duration, returnDefaultSpdDur));
        }
		else if(type == Type.LASER)
		{

		}
    }

    public void ActivateDualLinkBomb()
    {
        mIsUsingBomb = true;
        potraitTrans.gameObject.SetActive (false);
        dualLinkLaserTrans.gameObject.SetActive (true);
    }

    public void DeactivateDualLinkBomb()
    {
        mIsUsingBomb = false;
        dualLinkLaserTrans.gameObject.SetActive (false);
        mPlayerController.ResetLinkBar();
    }

    public void ActivatePotrait()
    {
        potraitTrans.gameObject.SetActive (true);
    }

    public void ResetDualLinkVal()
    {
        potraitTrans.gameObject.SetActive (false);
        BombManager.sSingleton.dualLinkState = BombManager.DualLinkState.NONE;
    }

    public bool IsUsingBomb { get { return mIsUsingBomb; } }

    IEnumerator TimeStopSequence (float stopDur, float returnSpdDur)
    {
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(stopDur));

        float currTime = 0;
        while(currTime < returnSpdDur)
        {
            while (UIManager.sSingleton.IsPauseMenu)
            {
                yield return null;
            }

            currTime += Time.unscaledDeltaTime;

            float val = currTime / returnSpdDur * (1 - mTimeScale); 
            if (val > 1) val = 1;

            Time.timeScale = mTimeScale + val;
            yield return null;
        }

        Time.timeScale = 1;
        Time.fixedDeltaTime = mSavedFixedDT;
        mIsUsingBomb = false;
		BombManager.sSingleton.isTimeStopBomb = false;
    }
}
