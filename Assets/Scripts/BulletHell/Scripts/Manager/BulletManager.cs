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

    // Group of bullets.
    public class GroupOfBullet
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

        public GroupOfBullet()
        {
            this.ownerName = "";
            this.typeOfBulletList = new List<TypeOfBullet>();
        }

        public GroupOfBullet(string name, List<TypeOfBullet> bulletGroupList)
        {
            this.ownerName = name;
            this.typeOfBulletList = bulletGroupList;
        }
    }

    bool mIsDisableSpawnBullet = false;
    float mDisableSpawnBulletTimer = 0, mDisableSpawnBulletTime = 0;

    List<GroupOfBullet.TypeOfBullet> mP1BulletGroupList = new List<GroupOfBullet.TypeOfBullet>();
    List<GroupOfBullet.TypeOfBullet> mP2BulletGroupList = new List<GroupOfBullet.TypeOfBullet>();
    List<GroupOfBullet.TypeOfBullet> mEnemy1BulletGroupList = new List<GroupOfBullet.TypeOfBullet>();

    List<GroupOfBullet> mAllBulletList = new List<GroupOfBullet>();
    List<GroupOfBullet> mAllEnemyBulletList = new List<GroupOfBullet>();

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
                    EnableEnemyBulletMovement();
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
            List<GroupOfBullet.TypeOfBullet> typeOfBulletList = mAllBulletList[i].typeOfBulletList;

            if (ownerName == TagManager.sSingleton.player1Bullet) mP1BulletGroupList = typeOfBulletList;
            else if (ownerName == TagManager.sSingleton.player2Bullet) mP2BulletGroupList = typeOfBulletList;
            else if (ownerName == TagManager.sSingleton.enemy1Bullet) 
            {
                AddFromToList(typeOfBulletList, ref mEnemy1BulletGroupList);
                mAllEnemyBulletList.Add(new GroupOfBullet("EnemyBullets", typeOfBulletList));
            }
        }
    }

    void AddFromToList(List<GroupOfBullet.TypeOfBullet> fromList, ref List<GroupOfBullet.TypeOfBullet> toList)
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

    public List<GroupOfBullet.TypeOfBullet> GetPlayer1BulletGroup { get { return mP1BulletGroupList; } }
    public List<GroupOfBullet.TypeOfBullet> GetPlayer2BulletGroup { get { return mP2BulletGroupList; } }
    public List<GroupOfBullet.TypeOfBullet> GetEnemy1BulletGroup { get { return mEnemy1BulletGroupList; } }

    public bool IsDisableSpawnBullet { get { return mIsDisableSpawnBullet; } }

    public void InstantiateAndCacheBullet(Transform ownerTrans, int total)
    {
        List<Bullet> bulletList = ownerTrans.GetComponent<BulletController>().bulletList;
        int count = bulletList.Count;

        // Group name.
        GameObject go = new GameObject();
        go.name = ownerTrans.name + "Bullet";   

        List<GroupOfBullet.TypeOfBullet> typeOfBulletList = new List<GroupOfBullet.TypeOfBullet>();

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
            GroupOfBullet.TypeOfBullet typeOfBullet = new GroupOfBullet.TypeOfBullet(goBulletName, instantiatedList);
            typeOfBulletList.Add(typeOfBullet);
        }
        mAllBulletList.Add(new GroupOfBullet(go.name, typeOfBulletList));
    }

    public void DisableEnemyBullets(bool isDisableSpawnBullet)
    {
        // TODO : not enemy1bullets
        int groupCount = mEnemy1BulletGroupList.Count;
        for (int i = 0; i < groupCount; i++)
        {
            GroupOfBullet.TypeOfBullet currGroupOfBullet = mEnemy1BulletGroupList[i];
            int bulletCount = currGroupOfBullet.bulletTransList.Count;

            for (int j = 0; j < bulletCount; j++)
            {
                GameObject currGO = currGroupOfBullet.bulletTransList[j].gameObject;
                SpriteRenderer sr = currGO.GetComponent<SpriteRenderer>();

                currGO.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(IEAlphaOutSequence(sr));
            }
        }
        mIsDisableSpawnBullet = isDisableSpawnBullet;
        mDisableSpawnBulletTime = GameManager.sSingleton.enemyDisBulletTime;
    }

    void EnableEnemyBulletMovement()
    {
        for (int i = 0; i < mStoppedEnemyBullets.Count; i++)
        {
            mStoppedEnemyBullets[i].GetComponent<BulletMove>().MoveBullet();
        }

        GameManager.sSingleton.isTimeStopBomb = false;
        mStoppedEnemyBullets.Clear();
    }

    public void DisableEnemyBulletMovement(float duration)
    {
        // Loop through all enemies.
        for (int i = 0; i < mAllEnemyBulletList.Count; i++)
        {
            GroupOfBullet currEnemy = mAllEnemyBulletList[i];

            // Loop through all type of bullets of current enemy.
            for (int j = 0; j < currEnemy.typeOfBulletList.Count; j++)
            {
                GroupOfBullet.TypeOfBullet currBulletType = currEnemy.typeOfBulletList[j];

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
}
