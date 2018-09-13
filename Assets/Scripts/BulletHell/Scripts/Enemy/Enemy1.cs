using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy1 : EnemyBase 
{
    List<List<AttackPattern>> mFullAtkList = new List<List<AttackPattern>>();
    Coroutine mMovementCo;

    int mPrevAtkNum, mMaxAtkNum;
    float mTimer;
    EnemyBase mEnemyBase;

    public override void Start()
    {
        base.Start();
        mEnemyBase = gameObject.GetComponentInParent<EnemyBase>();
    }

    void OnEnable()
    {
        EnemyManager.sSingleton.AddToList(transform);
        UpdateAttackPattern();
        GetMaxAttackNumber();
    }

	public override void Update () 
    {
        base.Update();

        if (GameManager.sSingleton.IsBossMakeEntrance() || !sr.isVisible) return;

        if (delayBeforeAttack != 0)
        {
            mTimer += Time.deltaTime;
            if (mTimer < delayBeforeAttack) return;
        }

        if (currActionNum < attackTransList.Count)
        {
            // Handle movement.
            if (mEnemyMovement.enabled == true && currActionNum < mEnemyMovement.movementList.Count && !mEnemyMovement.movementList[currActionNum].isCoroutine)
            {
                mMovementCo = StartCoroutine(mEnemyMovement.MoveToWayPoint(currActionNum));
//                StartCoroutine(mMovementCo);
            }

            List<AttackPattern> currAtkSequence = mFullAtkList[currActionNum];
            int currAtkNum = AttackPattern.currAtkNum;

            mPrevAtkNum = currAtkNum;
            if (mPrevAtkNum > mMaxAtkNum)
            {
                mPrevAtkNum = 0;
                currAtkNum = 0;
                AttackPattern.currAtkNum = 0;
            }

            if (currActionNum < attackDelayList.Count && attackDelayList[currActionNum] > 0)
            {
//                Debug.Log(attackDelayList[currActionNum]);
                attackDelayList[currActionNum] -= Time.deltaTime;
                return;
            }

            for (int i = 0; i < currAtkSequence.Count; i++)
            {
                AttackPattern currAtkType = currAtkSequence[i];
                if (currAtkNum == currAtkType.attackNum)
                {
                    if (i == 0) currAtkType.StartAttack(UpdateAttack);
                    else currAtkType.StartAttack(() => { });
                }
            }

            if (isBoss)
            {
                // Update to next attack and transform all enemy bullets into score pickup.
                float hpToMinus = currAtkSequence[0].hpPercentSkipAtk / 100 * totalHitPoint;
                float hpThresholdToSkip = totalHitPoint - hpToMinus;

                if (currHitPoint <= hpThresholdToSkip)
                {
                    for (int i = 0; i < currAtkSequence.Count; i++)
                    {
                        currAtkSequence[i].StopCoroutine();
                    }

                    UpdateAttack();
                    BulletManager.sSingleton.TransformEnemyBulsIntoScorePU ();
                }

                // Handle the appearing and disappearing of enemy health bar if too close to it.
                float length = (transform.position - GameManager.sSingleton.player1.position).sqrMagnitude;
                float length2 = 0;

                bool isP2 = false;
                if (GameManager.sSingleton.IsThisPlayerActive(2))
                {
                    isP2 = true;
                    length2 = (transform.position - GameManager.sSingleton.player2.position).sqrMagnitude;
                }

                if (length <= hpBarVisibleDistance || (isP2 && length2 <= hpBarVisibleDistance))
                {
                    mEnemyHealth.DisableHpBarAlpha();
                }
                else mEnemyHealth.EnableHpBarAlpha();
            }
        }
	}

    void UpdateAttack()
    {
        // UI: Update the hp bar for current attack.
        if (isBoss)
        {
            float hpToMinus = mFullAtkList[currActionNum][0].hpPercentSkipAtk / 100 * totalHitPoint;
            float hpThresholdToSkip = totalHitPoint - hpToMinus;

            if (mEnemyHealth != null) mEnemyHealth.ReduceHpBarUI(currHitPoint, hpThresholdToSkip, totalHitPoint);
            currHitPoint = hpThresholdToSkip;

            AttackPattern.currAtkNum = 0;

            if (AudioManager.sSingleton != null) AudioManager.sSingleton.PlayEnemyCurrAtkSfx();
            if (currHitPoint <= 0) EnemyDiedByTime();
        }

        mEnemyMovement.StopCurrMovement();
        StopCoroutine(mMovementCo);

        UIManager.sSingleton.DeactivateBossTimer();

        List<AttackPattern> currAtkSequence = mFullAtkList[currActionNum];
        for (int i = 0; i < currAtkSequence.Count; i++)
        {
            currAtkSequence[i].StopCoroutine();
        }
        currActionNum++;

        GetMaxAttackNumber();
    }

    void UpdateAttackPattern()
    {
        List<List<AttackPattern>> atkList = new List<List<AttackPattern>>();

        for (int i = 0; i < attackTransList.Count; i++)
        {
            List<AttackPattern> attackTypeList = new List<AttackPattern>();

            AttackPattern[] atkTypeArray = attackTransList[i].GetComponents<AttackPattern>();
            if (atkTypeArray.Length == 0) return;

            for (int j = 0; j < atkTypeArray.Length; j++)
            {
                attackTypeList.Add(atkTypeArray[j]);
            }
            atkList.Add(attackTypeList);
        }
        mFullAtkList = atkList;
    }

    void GetMaxAttackNumber()
    {
        if (currActionNum > mFullAtkList.Count - 1) return;

        List<AttackPattern> currAtkSequence = mFullAtkList[currActionNum];
        mMaxAtkNum = currAtkSequence[0].attackNum;

        for (int i = 1; i < currAtkSequence.Count; i++)
        {
            AttackPattern currAtkType = currAtkSequence[i];
            if (mMaxAtkNum < currAtkType.attackNum) mMaxAtkNum = currAtkType.attackNum;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if ((isBoss && GameManager.sSingleton.currState == GameManager.State.BOSS_MOVE_INTO_SCREEN) || !sr.isVisible) return;

        if (other.tag == TagManager.sSingleton.player1BulletTag || other.tag == TagManager.sSingleton.player2BulletTag ||
			other.tag == TagManager.sSingleton.magnumRadTag)
        {
            mEnemyBase.PullTrigger(other);
            if (mEnemyHealth != null && currHitPoint != totalHitPoint) mEnemyHealth.UpdateHpBarUI(currHitPoint, totalHitPoint);
        }
    }

    void OnDisable()
    {
        mTimer = 0;
        mPrevAtkNum = 0;
        currActionNum = 0;
        currHitPoint = totalHitPoint;
        if (sr != null) sr.color = mDefaultColor;
        EnemyManager.sSingleton.RemoveFromList(transform);
    }
}
