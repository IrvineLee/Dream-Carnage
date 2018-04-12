using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBase : MonoBehaviour 
{
    [System.Serializable]
    public class MoveInfo
    {
        [ReadOnly] public Vector3 target;
        [ReadOnly] public Vector3 moveDirection;
        [ReadOnly] public Vector3 velocity;
        [ReadOnly] public int currWayPoint;
    }

    [System.Serializable]
    public class Movement
    {
        [System.Serializable]
        public class WayPoint
        {
            public Transform targetTrans;
            public float speed;
            public float startDelay;

            public WayPoint()
            {
                this.targetTrans = null;
                this.speed = 1;
                this.startDelay = 0;
            }
        }

        public List<WayPoint> wayPointList = new List<WayPoint>();
        public bool isRepeat = false;
        [HideInInspector] public bool isCoroutine;
    }

    // Status.
    public bool isBoss = false;
    public int currHitPoint = 100;
    public int totalHitPoint = 100;
    public float moveSpeed = 1;

    public float scoreMultiplier = 1.0f;
//    public int scoreGetPerBullet = 100;

    // Animation that is being used.
    public Animator anim;

    [ReadOnly] public int currActionNum = 0;
    public List<AttackPattern> attackPatternList = new List<AttackPattern>();

    // Enemy movement.
    public MoveInfo moveInfo;
    public List<Movement> movementList = new List<Movement>();

    protected Transform mPlayer1, mPlayer2;
    protected List<BulletManager.Individual.TypeOfBullet> mTypeOfBulletList;
    protected MagicCirlce mMagicCircle;

    protected List<List<Action>> mListOfActionList = new List<List<Action>>();
    protected bool mIsStopTime = false;

    Rigidbody2D rgBody;
    bool mIsChangeColor = false;

    PlayerController mPlayer1Controller, mPlayer2Controller;

    void Awake()
    {
        EnemyManager.sSingleton.AddToList(transform);
    }

    public virtual void Start()
    {
        mPlayer1 = GameManager.sSingleton.player1;
        mPlayer1Controller = mPlayer1.GetComponent<PlayerController>();
            
        if (GameManager.sSingleton.player2 != null)
        {
            mPlayer2 = GameManager.sSingleton.player2;
            mPlayer2Controller = mPlayer2.GetComponent<PlayerController>();
        }

        rgBody = GetComponent<Rigidbody2D>();
        mMagicCircle = gameObject.GetComponentInChildren<MagicCirlce>();
    }

    public void EnableMagicCircle()
    {
        if(mMagicCircle != null)
            mMagicCircle.enabled = true;
    }

    public void PullTrigger(Collider2D other)
    {
        string p1Tag = TagManager.sSingleton.player1BulletTag;
        string p2Tag = TagManager.sSingleton.player2BulletTag;

        if (other.tag == p1Tag || other.tag == p2Tag)
        {
            int damage = other.GetComponent<BulletMove>().GetBulletDamage;
            GetDamaged(damage);

            if (other.tag == p1Tag)
            {
                mPlayer1Controller.UpdateLinkBar();
                mPlayer1Controller.UpdateScore((int)(damage * scoreMultiplier));
            }
            else if (other.tag == p2Tag)
            {
                mPlayer2Controller.UpdateLinkBar();
                mPlayer2Controller.UpdateScore((int)(damage * scoreMultiplier));
            }
        }

        other.gameObject.SetActive(false);
    }

    public IEnumerator MoveToWayPoint()
    {
        int savedActionNum = currActionNum;
        movementList[savedActionNum].isCoroutine = true;
        moveInfo.currWayPoint = 0;

        while(savedActionNum == currActionNum)
        {
            Movement currMoveThisAct = movementList[savedActionNum];

            int currWayIndex = moveInfo.currWayPoint;
            if (currWayIndex < currMoveThisAct.wayPointList.Count)
            {
                Movement.WayPoint currWayPoint = currMoveThisAct.wayPointList[currWayIndex];
                yield return new WaitForSeconds(currWayPoint.startDelay);

                moveInfo.target = currWayPoint.targetTrans.position;
                moveInfo.moveDirection = moveInfo.target - transform.position;
                moveInfo.velocity = rgBody.velocity;

                if (moveInfo.moveDirection.magnitude < 0.5f) moveInfo.currWayPoint++;
                else moveInfo.velocity = moveInfo.moveDirection.normalized * currWayPoint.speed;
            }
            else
            {
                if (currMoveThisAct.isRepeat) moveInfo.currWayPoint = 0;
                else moveInfo.velocity = Vector3.zero;
            }
            rgBody.velocity = moveInfo.velocity;

            yield return null;
        }
        rgBody.velocity = Vector3.zero;  
        movementList[savedActionNum].isCoroutine = false;
    }

    protected void StopCurrMovement(IEnumerator co) 
    { 
        if (co == null) return;
        StopCoroutine(co);
        rgBody.velocity = Vector3.zero;
    }

    public bool IsStopTime
    {
        get { return mIsStopTime; }
        set { mIsStopTime = value; }
    }

    void GetDamaged(int damagedValue)
    {
        currHitPoint -= damagedValue;
        if (currHitPoint <= 0)
        {
            if (isBoss) CameraShake.sSingleton.ShakeCamera();

            // TODO: Enemy destroyed animation..
            Destroy(gameObject);
            PickUpManager.sSingleton.TransformBulletsIntoPoints(mTypeOfBulletList);
            BulletManager.sSingleton.DisableEnemyBullets(false);
        }
        if (!mIsChangeColor) StartCoroutine(GetDamagedColorChange());
    }

    IEnumerator GetDamagedColorChange()
    {
        mIsChangeColor = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color defaultColor = sr.color;

        sr.color = GameManager.sSingleton.enemyDmgColor;
        yield return new WaitForSeconds(GameManager.sSingleton.enemyDmgColorDur);
        sr.color = defaultColor;
        mIsChangeColor = false;
    }
}
