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

    bool mIsUsingBomb = false;

    public void ActivateBomb()
    {
        if (type == Type.TIME_STOP && !mIsUsingBomb)
        {
            mIsUsingBomb = true;
            GameManager.sSingleton.isTimeStopBomb = true;
            EnemyManager.sSingleton.StopAllEnemy();
            BulletManager.sSingleton.TimeStopEffect(duration, returnDefaultSpdDur);
            PickUpManager.sSingleton.TimeStopEffect(duration, returnDefaultSpdDur);
            StartCoroutine(WaitThenDisableTimeStop(duration + returnDefaultSpdDur));
        }
    }

    public bool IsUsingBomb { get { return mIsUsingBomb; } }

    IEnumerator WaitThenDisableTimeStop (float duration)
    {
        yield return new WaitForSeconds(duration);
        mIsUsingBomb = false;
        GameManager.sSingleton.isTimeStopBomb = false;
        BulletManager.sSingleton.ResetValAfterTimeStop();
    }
}
