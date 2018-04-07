using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBase : MonoBehaviour 
{
    [System.Serializable]
    public class Movement
    {
        [System.Serializable]
        public class ToPos
        {
            public Vector2 targetPos;
            public float timeTaken;
            public float startDelay;

            public ToPos(Vector2 targetPos, float timeTaken, float startDelay)
            {
                this.targetPos = targetPos;
                this.timeTaken = timeTaken;
                this.startDelay = startDelay;
            }
        }

        [HideInInspector] public int currActionNum;
        [HideInInspector] public bool isCoroutine;
        public List<ToPos> targetPosList = new List<ToPos>();
    }

    // Status.
    public int currHitPoint = 100;
    public int totalHitPoint = 100;
    public float moveSpeed = 1;

    public float scoreMultiplier = 1.0f;
//    public int scoreGetPerBullet = 100;

    // Animation that is being used.
    public Animator anim;
    public List<AttackPattern> attackPatternList = new List<AttackPattern>();
    public List<Movement> movementList = new List<Movement>();

    protected Transform mPlayer1, mPlayer2;
    protected List<BulletManager.Individual.TypeOfBullet> mTypeOfBulletList;
    protected MagicCirlce mMagicCircle;

    protected int mCurrActionNum = 0;
    protected List<List<Action>> mListOfActionList = new List<List<Action>>();
    protected bool mIsStopTime = false;

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

        for (int i = 0; i < movementList.Count; i++)
        { movementList[i].currActionNum = i; }

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
            int dmg = other.GetComponent<BulletMove>().GetBulletDamage;
            GetDamaged(dmg);

            if (other.tag == p1Tag)
            {
                mPlayer1Controller.UpdateLinkBar();
                mPlayer1Controller.UpdateScore((int)(dmg * scoreMultiplier));
            }
            else if (other.tag == p2Tag)
            {
                mPlayer2Controller.UpdateLinkBar();
                mPlayer2Controller.UpdateScore((int)(dmg * scoreMultiplier));
            }
        }

        other.gameObject.SetActive(false);
    }

    public IEnumerator MoveToPos(Movement currMovement, Action doLast)
    {
        currMovement.isCoroutine = true;
        for (int i = 0; i < currMovement.targetPosList.Count; i++)
        {
            EnemyBase.Movement.ToPos currToPos = currMovement.targetPosList[i];
            Vector2 pos = currToPos.targetPos;
            float timeTaken = currToPos.timeTaken;
            float delay = currToPos.startDelay;

            yield return new WaitForSeconds(delay);

            Vector3 defaultPos = transform.position;
            float timeStartLerp = Time.time;

            while((Vector2)transform.position != pos)
            {
                float timeSinceStarted = Time.time - timeStartLerp;
                float percentageComplete = timeSinceStarted / timeTaken;

                transform.position = Vector3.Lerp(defaultPos, pos, percentageComplete);
                yield return null;
            }
        }
        doLast();
        currMovement.isCoroutine = false;
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
