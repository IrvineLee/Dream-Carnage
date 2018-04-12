using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour 
{
    public enum Type
    {
        NONE = 0, 
        TIME_STOP
    }
    public Type type = Type.NONE;

    public float duration = 3.0f;
    public float returnDefaultSpdDur = 1.0f;

    float mTimeScale = 0.05f;
    float mFixedDeltaTime = 0.0001f, mSavedFixedDT;
    bool mIsUsingBomb = false;

    void Start()
    {
        mSavedFixedDT = Time.fixedDeltaTime;
    }

    public void ActivateBomb()
    {
        if (type == Type.TIME_STOP && !mIsUsingBomb)
        {
            mIsUsingBomb = true;
            GameManager.sSingleton.isTimeStopBomb = true;

            Time.timeScale = mTimeScale;
            Time.fixedDeltaTime = mFixedDeltaTime;
            StartCoroutine(TimeStopSequence(duration, returnDefaultSpdDur));
        }
    }

    public bool IsUsingBomb { get { return mIsUsingBomb; } }

    IEnumerator TimeStopSequence (float stopDur, float returnSpdDur)
    {
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(stopDur));

        float currTime = 0;
        while(currTime < returnSpdDur)
        {
            currTime += Time.unscaledDeltaTime;

            float val = currTime / returnSpdDur * (1 - mTimeScale); 
            if (val > 1) val = 1;

            Time.timeScale = mTimeScale + val;
            yield return null;
        }

        Time.timeScale = 1;
        Time.fixedDeltaTime = mSavedFixedDT;
        mIsUsingBomb = false;
        GameManager.sSingleton.isTimeStopBomb = false;
    }
}
