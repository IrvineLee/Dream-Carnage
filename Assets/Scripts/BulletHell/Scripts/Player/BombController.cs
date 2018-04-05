﻿using System.Collections;
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

    public void ActivateBomb()
    {
        if (type == Type.TIME_STOP)
        {
            EnemyManager.sSingleton.StopAllEnemy();
            BulletManager.sSingleton.TimeStopEffect(duration, returnDefaultSpdDur);
        }
    }
}
