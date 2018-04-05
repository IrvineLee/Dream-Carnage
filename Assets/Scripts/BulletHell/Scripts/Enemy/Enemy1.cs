using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy1 : EnemyBase 
{
    static int mBulletNum = 0;

    int mCurrMoveNum = 0;
    bool mIsUpdatedAtk = false;

    public override void Start()
    {
        base.Start();
        mTypeOfBulletList = BulletManager.sSingleton.GetEnemy1BulletGroup;

        for (int i = 0; i < attackPatternList.Count; i++)
        {  AddToActionList(attackPatternList[i]); }
    }

	void Update () 
    {
        if (mIsStopTime) return;

        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (mCurrActionNum < mListOfActionList.Count)
        {
            // Handle movement.
            if (mCurrMoveNum < movementList.Count)
            {
                Movement currMovement = movementList[mCurrMoveNum];
                if (currMovement.currActionNum == mCurrActionNum)
                {
                    if(!currMovement.isCoroutine) StartCoroutine(MoveToPos(currMovement, AddToNextMovement));
                }
            }

            // Handle current action. Loop is when there are more than 1 attack pattern together.
            List<Action> currAP = mListOfActionList[mCurrActionNum];
            for (int i = 0; i < currAP.Count; i++)
            { currAP[i](); }
        }
	}

    public void UpdateAttack()
    {
        if (!mIsUpdatedAtk)
        {
            mCurrActionNum++;
            mIsUpdatedAtk = true;
        }
    }

    void ShootAtPlayer(ShootAtk currAtkPat)
    {
        if (!currAtkPat.IsCoroutine)
        {
            ShowPotraitAndTimer(currAtkPat);

            if (currAtkPat.type == ShootAtk.Type.SINGLE_SHOT)
                StartCoroutine(currAtkPat.ShootAtPlayerSingleShot(mPlayer1, () => GetUpdatedBulletTrans(), UpdateAttack));
            else if (currAtkPat.type == ShootAtk.Type.ANGLE_SHOT)
                StartCoroutine(currAtkPat.ShootAtPlayerAngle(mPlayer1, () => GetUpdatedBulletTrans(), UpdateAttack));
        }
    }

    void ShootAroundInCircles(ShootAroundInCircleAtk currAtkPat) 
    { 
        if (!currAtkPat.IsCoroutine)
        {
            ShowPotraitAndTimer(currAtkPat);
            StartCoroutine(currAtkPat.ShootAroundInCirclesRoutine(() => GetUpdatedBulletTrans(), UpdateAttack)); 
        }
    }

    void SineWaveShoot(SineWaveAtk currAtkPat) 
    { 
        if (!currAtkPat.IsCoroutine)
        {
            ShowPotraitAndTimer(currAtkPat);
            currAtkPat.SineWaveShoot(mPlayer1, () => GetUpdatedBulletTrans(), UpdateAttack);
        }
    }

    void ShowPotraitAndTimer(AttackPattern ap)
    {
        mIsUpdatedAtk = false;
        ap.ShowPotrait();
        if (ap.isShowDuration) UIManager.sSingleton.ActivateBossTimer(ap.duration);
    }

    Transform GetUpdatedBulletTrans()
    {
        Transform currBullet = mTypeOfBulletList[0].bulletTransList[mBulletNum];

        mBulletNum++;
        if (mBulletNum >= mTypeOfBulletList[0].bulletTransList.Count) mBulletNum = 0;

        return currBullet;
    }

    void AddToNextMovement() { mCurrMoveNum++; }

    void AddToActionList(AttackPattern ap)
    {
        List<Action> actionList = new List<Action>();
       
        // Multiple attack patterns in a single attack.
        if (ap.gameObject.tag != TagManager.sSingleton.attackPatternTag)
        {
            // Put the sprite onto the 1st attack pattern so it will only get called once.
            AttackPattern firstAP = ap.transform.GetChild(0).GetComponent<AttackPattern>();
            firstAP.charSprite = ap.charSprite;
            firstAP.spellCardSprite = ap.spellCardSprite;

            for (int i = 0; i < ap.transform.childCount; i++)
            {
                AttackPattern currAP = ap.transform.GetChild(i).GetComponent<AttackPattern>();
                currAP.duration = ap.duration;
//                currAP.isDisableBulletAftDone = ap.isDisableBulletAftDone;
//                currAP.onceStartDelay = ap.onceStartDelay;
                AddCurrAttack(currAP, ref actionList);
            }
        }
        // Single attack pattern only.
        else AddCurrAttack(ap, ref actionList);

        mListOfActionList.Add(actionList);
    }

    void AddCurrAttack(AttackPattern ap, ref List<Action> actionList)
    {
        if (ap is ShootAtk) actionList.Add(new Action( () => ShootAtPlayer( (ShootAtk) ap) ));
        else if (ap is ShootAroundInCircleAtk) actionList.Add(new Action( () => ShootAroundInCircles( (ShootAroundInCircleAtk) ap) ));
        else if (ap is SineWaveAtk) actionList.Add(new Action( () => SineWaveShoot( (SineWaveAtk) ap) ));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other is BoxCollider2D)
        {
            gameObject.GetComponentInParent<EnemyBase>().PullTrigger(other);
        }
    }
}
