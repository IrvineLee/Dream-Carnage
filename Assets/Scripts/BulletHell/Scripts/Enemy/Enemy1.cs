using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy1 : EnemyBase 
{
    public Transform healthBarTrans;

    List<List<AttackPattern>> mFullAtkList = new List<List<AttackPattern>>();
    IEnumerator mMovementCo;

    EnemyBase mEnemyBase;
    EnemyHealth mEnemyHealth;

    public override void Start()
    {
        base.Start();

        mEnemyBase = gameObject.GetComponentInParent<EnemyBase>();
        mEnemyHealth = healthBarTrans.GetComponent<EnemyHealth>();

        for (int i = 0; i < attackTransList.Count; i++)
        {
            List<AttackPattern> attackTypeList = new List<AttackPattern>();

            AttackPattern[] atkTypeArray = attackTransList[i].GetComponents<AttackPattern>();

            for (int j = 0; j < atkTypeArray.Length; j++)
            {
                attackTypeList.Add(atkTypeArray[j]);
            }
            mFullAtkList.Add(attackTypeList);
        }
    }

	public override void Update () 
    {
        base.Update();

        if (currActionNum < attackTransList.Count)
        {
            // Handle movement.
            if (currActionNum < movementList.Count && !movementList[currActionNum].isCoroutine)
            {
                mMovementCo = MoveToWayPoint();
                StartCoroutine(MoveToWayPoint());
            }

            List<AttackPattern> currAtkSequence = mFullAtkList[currActionNum];
            for (int i = 0; i < currAtkSequence.Count; i++)
            {
                AttackPattern currAtkType = currAtkSequence[i];
                currAtkType.StartAttack(mPlayer1, UpdateAttack);
            }
        }
	}

    public void UpdateAttack()
    {
        StopCurrMovement(mMovementCo);
        currActionNum++;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other is BoxCollider2D)
        {
            mEnemyBase.PullTrigger(other);
            mEnemyHealth.UpdateHpBarUI(currHitPoint, totalHitPoint);
        }
    }
}
