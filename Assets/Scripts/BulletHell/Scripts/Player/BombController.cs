using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BombController : MonoBehaviour 
{
    public enum Type
    {
        NONE = 0, 
        TIME_STOP,
		LASER
    }
    public Type type = Type.NONE;

    public SpriteRenderer potraitSR;
    public float appearSpeed;
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
        StartCoroutine(IEAlphaSequence(potraitSR, 0, () => { }));
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
        StartCoroutine(IEAlphaSequence(potraitSR, 1, () => { }));
    }

    public void ResetDualLinkVal()
    {
        StartCoroutine(IEAlphaSequence(potraitSR, 0, () => { }));
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

    IEnumerator IEAlphaSequence (SpriteRenderer sr, float toAlpha, Action doLast)
    {
        Color color = Color.white;
        if (sr.color.a < toAlpha)
        {
            while (sr.color.a < toAlpha)
            {
                color = sr.color;
                color.a += Time.unscaledDeltaTime * appearSpeed;

                if (color.a > toAlpha) color.a = toAlpha;
                sr.color = color;

                yield return null;
            }
        }
        else
        {
            while (sr.color.a > toAlpha)
            {
                color = sr.color;
                color.a -= Time.unscaledDeltaTime * appearSpeed;

                if (color.a < toAlpha) color.a = toAlpha;
                sr.color = color;

                yield return null;
            }
        }
        doLast();
    }
}
