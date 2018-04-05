using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour 
{
    public static EnemyManager sSingleton { get { return _sSingleton; } }
    static EnemyManager _sSingleton;

    public List<Transform> EnemyList = new List<Transform>();

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    public void EnableAllEnemy()
    {
        for (int i = 0; i < EnemyList.Count; i++)
        {
            EnemyBase currEnemy = EnemyList[i].GetComponent<EnemyBase>();
            currEnemy.IsStopTime = false;
        }
    }

    public void StopAllEnemy()
    {
        for (int i = 0; i < EnemyList.Count; i++)
        {
            EnemyBase currEnemy = EnemyList[i].GetComponent<EnemyBase>();
            currEnemy.IsStopTime = true;
        }
    }

    public void AddToList(Transform trans)
    {
        EnemyList.Add(trans);
    }
}
