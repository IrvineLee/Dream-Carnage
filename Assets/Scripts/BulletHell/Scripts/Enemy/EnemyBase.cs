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
    public float pingPongSpeed = 0.01f;
    public float pingPongVal = 0.005f;
    public bool isEndShakeScreen = false;
    public int currHitPoint = 100;
    public int totalHitPoint = 100;
    public float moveSpeed = 1;

    public float scoreMultiplier = 1.0f;
//    public int scoreGetPerBullet = 100;

    // Animation that is being used.
    public Animator anim;

    [ReadOnly] public int currActionNum = 0;
    public List<Transform> attackTransList = new List<Transform>();

    // Enemy movement.
    public MoveInfo moveInfo;
    public List<Movement> movementList = new List<Movement>();

    protected Transform mPlayer1, mPlayer2;
    protected List<List<Transform>> mBulletList = new List<List<Transform>>();
    protected SpriteRenderer sr;
    protected MagicCirlce mMagicCircle;

    protected List<List<Action>> mListOfActionList = new List<List<Action>>();

    Vector3 target;
    bool mIsChangeColor = false, mIsPPUp = true, mIsGetPPTarget = true;

    Rigidbody2D rgBody;
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

        sr = GetComponent<SpriteRenderer>();
        mMagicCircle = gameObject.GetComponentInChildren<MagicCirlce>();

        rgBody = GetComponent<Rigidbody2D>();
    }

    public virtual void Update()
    {
        if (UIManager.sSingleton.IsPauseMenu || 
            BombManager.sSingleton.dualLinkState == BombManager.DualLinkState.PLAYER_INPUT ||
            BombManager.sSingleton.dualLinkState == BombManager.DualLinkState.ACTIVATE_PAUSE) return;

		if (isBoss)
        {
            if (mIsGetPPTarget)
            {
                target = transform.position;
                mIsGetPPTarget = false;

                if (mIsPPUp) target.y += pingPongVal;
                else target.y -= pingPongVal;
            }

            transform.position = Vector3.MoveTowards(transform.position, target, pingPongSpeed * Time.deltaTime);
            if (transform.position == target)
            {
                mIsGetPPTarget = true;
                mIsPPUp = !mIsPPUp;
            }
        }
    }

    public void EnableMagicCircle()
    {
        if(mMagicCircle != null)
            mMagicCircle.enabled = true;
    }

    public void PullTrigger(Collider2D other)
    {
        if (currHitPoint <= 0) return;

        int damage = 0;
        string otherLayer = LayerMask.LayerToName(other.gameObject.layer);

        if (otherLayer == TagManager.sSingleton.playerBulletLayer)
        {
            damage = other.GetComponent<BulletMove>().GetBulletDamage;
            other.gameObject.SetActive(false);
        }
        else damage = other.GetComponent<Laser>().dmgPerFrame;

        GetDamaged(damage, other.tag);
    }

    protected IEnumerator MoveToWayPoint()
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

    void GetDamaged(int damagedValue, string otherTag)
    {
        float scoreGet = damagedValue * scoreMultiplier;

        currHitPoint -= damagedValue;
        if (currHitPoint <= 0)
        {
            if (isEndShakeScreen) CameraShake.sSingleton.ShakeCamera();

            scoreGet = (currHitPoint + damagedValue) * scoreMultiplier;

            // TODO: Enemy destroyed animation..
            Destroy(gameObject);

            BulletManager.sSingleton.TransformBulletsIntoScorePU();
            BulletManager.sSingleton.DisableEnemyBullets(false);
            UIManager.sSingleton.DeactivateBossTimer();
        }

        PlayerGainScore((int)scoreGet, otherTag);

        if (!mIsChangeColor) StartCoroutine(GetDamagedColorChange());
    }

    void PlayerGainScore(int val, string otherTag)
    {
        if (otherTag == TagManager.sSingleton.player1BulletTag)
        {
            mPlayer1Controller.UpdateLinkBar();
            mPlayer1Controller.UpdateScore(val);
        }
        else if (otherTag == TagManager.sSingleton.player2BulletTag)
        {
            mPlayer2Controller.UpdateLinkBar();
            mPlayer2Controller.UpdateScore(val);
        }
    }

    IEnumerator GetDamagedColorChange()
    {
        mIsChangeColor = true;
        Color defaultColor = sr.color;

        sr.color = GameManager.sSingleton.enemyDmgColor;
        yield return new WaitForSeconds(GameManager.sSingleton.enemyDmgColorDur);
        sr.color = defaultColor;
        mIsChangeColor = false;
    }
}
