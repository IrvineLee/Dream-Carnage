using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineUtil
{
    public static IEnumerator WaitForRealSeconds(float time)
    {
        float timerPause = 0;
        float start = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup < start + time + timerPause)
        {
            while (UIManager.sSingleton.IsPauseMenu)
            {
                timerPause += Time.unscaledDeltaTime;
                yield return null;
            }
            yield return null;
        }
    }
}
