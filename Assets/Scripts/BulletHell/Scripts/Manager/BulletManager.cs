using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour 
{
    public static BulletManager sSingleton { get { return _sSingleton; } }
    static BulletManager _sSingleton;

    [System.Serializable]
    public class Bullet
    {
        public enum State
        {
            NONE = 0,
            ONE_DIRECTION,
            SINE_WAVE,
            STYLE_ATK_1
        }
        public State state;

        // General stats.
        public Transform prefab;
        public Vector2 direction;
        public int damage;
        public float speed;
        public float spawnY_Offset;

        // Sine wave variables.
        public float frequency;  // Speed of sine movement
        public float magnitude;   // Size of sine movement
        public float magnitudeExpandMult;

        public Bullet()
        {
            state = State.NONE;
            prefab = null;
            direction = Vector2.up;
            damage = 1;
            speed = 1.0f;
            frequency = 1;
            magnitude = 1;
            spawnY_Offset = 0;
            magnitudeExpandMult = 0;
        }

        public Bullet(Transform prefab, Vector2 direction, int damage, float speed)
        {
            this.state = State.ONE_DIRECTION;
            this.prefab = prefab;
            this.direction = direction;
            this.damage = damage;
            this.speed = speed;
        }

        public Bullet(Transform prefab, Vector2 direction, int damage, float speed, float frequency, float magnitude)
        {
            this.state = State.SINE_WAVE;
            this.prefab = prefab;
            this.direction = direction;
            this.damage = damage;
            this.speed = speed;
            this.frequency = frequency;
            this.magnitude = magnitude;
        }
    }

    [System.Serializable]
    public class ChangeBulletProp
    {
        public float time;
        public float speed;
    }

    // Group of individual bullets.
    public class Individual
    {
        public string ownerName;
        public List<TypeOfBullet> typeOfBulletList;

        public class TypeOfBullet
        {
            public string bulletName;
            public List<Transform> bulletTransList;

            public TypeOfBullet()
            {
                this.bulletName = "";
                this.bulletTransList = new List<Transform>();
            }

            public TypeOfBullet(string name, List<Transform> bulletTransList)
            {
                this.bulletName = name;
                this.bulletTransList = bulletTransList;
            }
        }

        public Individual()
        {
            this.ownerName = "";
            this.typeOfBulletList = new List<TypeOfBullet>();
        }

        public Individual(string name, List<TypeOfBullet> bulletGroupList)
        {
            this.ownerName = name;
            this.typeOfBulletList = bulletGroupList;
        }
    }

    bool mIsDisableSpawnBullet = false;
    float mDisableSpawnBulletTimer = 0, mDisableSpawnBulletTime = 0;
    float mReturnDefaultSpdDur = 0;

    List<Individual.TypeOfBullet> mP1BulletGroupList = new List<Individual.TypeOfBullet>();
    List<Individual.TypeOfBullet> mP2BulletGroupList = new List<Individual.TypeOfBullet>();
    List<Individual.TypeOfBullet> mEnemy1BulletGroupList = new List<Individual.TypeOfBullet>();

    List<Individual> mAllBulletList = new List<Individual>();
    List<Individual> mAllEnemyBulletList = new List<Individual>();

    // When player activated time stop bomb.
    List<Transform> mStoppedEnemyBullets = new List<Transform>();

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (mIsDisableSpawnBullet)
        {
            mDisableSpawnBulletTimer += Time.deltaTime;

            if (mDisableSpawnBulletTimer >= mDisableSpawnBulletTime)
            {
                mDisableSpawnBulletTimer = 0;
                mIsDisableSpawnBullet = false;

                if (GameManager.sSingleton.isTimeStopBomb)
                {
                    mIsDisableSpawnBullet = true;
                    EnableEnemyBulletMovement(mReturnDefaultSpdDur);
                    EnemyManager.sSingleton.EnableAllEnemy();
                }
            }
        }
    }

    void Initialize()
    {
        for (int i = 0; i < mAllBulletList.Count; i++)
        {
            string ownerName = mAllBulletList[i].ownerName;
            List<Individual.TypeOfBullet> typeOfBulletList = mAllBulletList[i].typeOfBulletList;

            if (ownerName == TagManager.sSingleton.player1Bullet) mP1BulletGroupList = typeOfBulletList;
            else if (ownerName == TagManager.sSingleton.player2Bullet) mP2BulletGroupList = typeOfBulletList;
            else if (ownerName == TagManager.sSingleton.enemy1Bullet) 
            {
                AddFromToList(typeOfBulletList, ref mEnemy1BulletGroupList);
                mAllEnemyBulletList.Add(new Individual("EnemyBullets", typeOfBulletList));
            }
        }
    }

    void AddFromToList(List<Individual.TypeOfBullet> fromList, ref List<Individual.TypeOfBullet> toList)
    {
        for (int i = 0; i < fromList.Count; i++)
        {
            if (toList.Count != 0 && toList[i].bulletName == fromList[i].bulletName)
            {
                for (int j = 0; j < fromList[i].bulletTransList.Count; j++)
                {
                    toList[i].bulletTransList.Add(fromList[i].bulletTransList[j]);
                }
            }
            else toList.Add(fromList[i]);
        }
    }

    public List<Individual.TypeOfBullet> GetPlayer1BulletGroup { get { return mP1BulletGroupList; } }
    public List<Individual.TypeOfBullet> GetPlayer2BulletGroup { get { return mP2BulletGroupList; } }
    public List<Individual.TypeOfBullet> GetEnemy1BulletGroup { get { return mEnemy1BulletGroupList; } }

    public bool IsDisableSpawnBullet { get { return mIsDisableSpawnBullet; } set { mIsDisableSpawnBullet = value; } }

    public void InstantiateAndCacheBullet(Transform ownerTrans, int total)
    {
        List<Bullet> bulletList = ownerTrans.GetComponent<BulletController>().bulletList;
        int count = bulletList.Count;

        // Group name.
        GameObject go = new GameObject();
        go.name = ownerTrans.name + "Bullet";   

        List<Individual.TypeOfBullet> typeOfBulletList = new List<Individual.TypeOfBullet>();

        for (int i = 0; i < count; i++)
        {
            // Group name for type of bullets.
            GameObject goBulletGroup = new GameObject();
            string goBulletName = "Bullet" + i;
            goBulletGroup.name = goBulletName;
            goBulletGroup.transform.parent = go.transform;

            List<Transform> instantiatedList = new List<Transform>();

            if (bulletList[i].prefab == null) return;
            
            // Instantiate total amount of current bullet type.
            for (int j = 0; j < total; j++)
            {
                Bullet ownerBullet = bulletList[i];

                Transform trans = Instantiate(ownerBullet.prefab, Vector3.zero, Quaternion.identity);
                trans.name = ownerTrans.name;
                trans.SetParent(goBulletGroup.transform);
                trans.gameObject.SetActive(false);

                // Set sort order for enemy bullets.
                SpriteRenderer transSr = trans.GetComponent<SpriteRenderer>();
                if (transSr != null && transSr.sortingLayerName == TagManager.sSingleton.sortLayerTopG) 
                    transSr.sortingOrder = j;

                BulletMove bulletMv = trans.gameObject.GetComponent<BulletMove>();

                if (bulletMv != null) bulletMv.SetBulletValues(ownerBullet);
                else
                {
                    BulletMove[] bulletMvArray = trans.gameObject.GetComponentsInChildren<BulletMove>();
                    for (int k = 0; k < bulletMvArray.Length; k++)
                    { bulletMvArray[k].SetBulletValues(ownerBullet); }
                }
                instantiatedList.Add(trans);
            }
            Individual.TypeOfBullet typeOfBullet = new Individual.TypeOfBullet(goBulletName, instantiatedList);
            typeOfBulletList.Add(typeOfBullet);
        }
        mAllBulletList.Add(new Individual(go.name, typeOfBulletList));
    }

    public void TimeStopEffect(float stopDuration, float returnDefaultSpdDur)
    {
        DisableEnemyBulletMovement(stopDuration);
        mReturnDefaultSpdDur = returnDefaultSpdDur;
    }

    public void DisableEnemyBullets(bool isDisableSpawnBullet)
    {
        // TODO : not enemy1bullets
        for (int i = 0; i < mAllEnemyBulletList.Count; i++)
        {
            Individual currEnemy = mAllEnemyBulletList[i];

            // Loop through all type of bullets of current enemy.
            for (int j = 0; j < currEnemy.typeOfBulletList.Count; j++)
            {
                Individual.TypeOfBullet currBulletType = currEnemy.typeOfBulletList[i];

                // Loop through all the same bullet type.
                for (int k = 0; k < currBulletType.bulletTransList.Count; k++)
                {
                    GameObject currBulletGO = currBulletType.bulletTransList[k].gameObject;
                    SpriteRenderer sr = currBulletGO.GetComponent<SpriteRenderer>();

                    currBulletGO.GetComponent<Collider2D>().enabled = false;
                    StartCoroutine(IEAlphaOutSequence(sr));
                }
            }
        }

        mIsDisableSpawnBullet = isDisableSpawnBullet;
        mDisableSpawnBulletTime = GameManager.sSingleton.enemyDisBulletTime;
    }

    void EnableEnemyBulletMovement(float duration)
    {
        bool isActive = false;
        for (int i = 0; i < mStoppedEnemyBullets.Count; i++)
        {
            Transform currBullet = mStoppedEnemyBullets[i];
            if (currBullet.gameObject.activeSelf)
            {
                isActive = true;
                currBullet.GetComponent<BulletMove>().MoveBullet(duration);
            }
        }

        // If any stopped bullet is not active, wait for the duration and disable time stop.
        if (!isActive) StartCoroutine(WaitThenDisableTimeStop(duration));
        mStoppedEnemyBullets.Clear();
    }

    void DisableEnemyBulletMovement(float duration)
    {
        // Loop through all enemies.
        for (int i = 0; i < mAllEnemyBulletList.Count; i++)
        {
            Individual currEnemy = mAllEnemyBulletList[i];

            // Loop through all type of bullets of current enemy.
            for (int j = 0; j < currEnemy.typeOfBulletList.Count; j++)
            {
                Individual.TypeOfBullet currBulletType = currEnemy.typeOfBulletList[j];

                // Loop through all the same bullet type.
                for (int k = 0; k < currBulletType.bulletTransList.Count; k++)
                {
                    Transform currBullet = currBulletType.bulletTransList[k];

                    if (currBullet.gameObject.activeSelf)
                    {
                        currBullet.gameObject.GetComponent<BulletMove>().StopBullet();
                        mStoppedEnemyBullets.Add(currBullet);
                    }
                }
            }
        }

        GameManager.sSingleton.isTimeStopBomb = true;
        mIsDisableSpawnBullet = true;
        mDisableSpawnBulletTime = duration;
    }

    IEnumerator IEAlphaOutSequence (SpriteRenderer sr)
    {
        Color color = Color.white;
        while(sr.color.a > 0)
        {
            color = sr.color;
            color.a -= Time.deltaTime * GameManager.sSingleton.bulletDisappearSpeed;
            sr.color = color;

            yield return null;
        }

        // Reset the values back to default.
        color = sr.color;
        color.a = 1;
        sr.color = color;

        sr.gameObject.GetComponent<Collider2D>().enabled = true;
        sr.gameObject.SetActive(false);
    }

    IEnumerator WaitThenDisableTimeStop (float duration)
    {
        yield return new WaitForSeconds(duration);
        GameManager.sSingleton.isTimeStopBomb = false;
        mIsDisableSpawnBullet = false;
    }
}
