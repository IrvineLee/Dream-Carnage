﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour 
{
    public static EnemyManager sSingleton { get { return _sSingleton; } }
    static EnemyManager _sSingleton;

    [HideInInspector] public List<Transform> EnemyList = new List<Transform>();

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    public void AddToList(Transform trans)
    {
        EnemyList.Add(trans);
    }
}
