using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour 
{
    public static BulletManager sSingleton 
    { 
        get 
        { 
            if (_sSingleton == null)
                _sSingleton = (BulletManager)FindObjectOfType(typeof(BulletManager));
            return _sSingleton; 
        } 
    }
    static BulletManager _sSingleton;

    public enum GroupIndex
    {
        PLAYER_MAIN = 0,
        PLAYER_SECONDARY,
        ENEMY
    }

    public BulletPrefabData bulletPrefabData;
//	[ReadOnlyAttribute]public int totalBulletsInPlay = 0;

    List<int> mPlyCurrMainBulIndexList = new List<int>();
    List<int> mPlyCurrSecondBulIndexList = new List<int>();
    List<int> mEnemyCurrBulIndexList = new List<int>();
    List<int> mEnemySparkIndexList = new List<int>();
    int plySkillBulIndex;

    List<List<Transform>> mPlyMainBulletList = new List<List<Transform>>();
    List<List<Transform>> mPlySecondaryBulletList = new List<List<Transform>>();
    List<List<Transform>> mEnemyBulletList = new List<List<Transform>>();
    List<List<Transform>> mEnemyBulletSparkList = new List<List<Transform>>();
    List<Transform> mPlySkillBulletList = new List<Transform>();

    int mOrderInLayer = 0;
    bool mIsDisableSpawnBullet = false;
    float mDisableSpawnBulletTimer = 0, mDisableSpawnBulletTime = 0;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

    void Start()
    {
        for (int i = 0; i < mPlyMainBulletList.Count; i++)
        {
            mPlyCurrMainBulIndexList.Add(0);
        }
        for (int i = 0; i < mPlySecondaryBulletList.Count; i++)
        {
            mPlyCurrSecondBulIndexList.Add(0);
        }
        for (int i = 0; i < mEnemyBulletList.Count; i++)
        {
            mEnemyCurrBulIndexList.Add(0);
        }
        for (int i = 0; i < mEnemyBulletSparkList.Count; i++)
        {
            mEnemySparkIndexList.Add(0);
        }
    }

    void Update()
    {
//		int total = 0;
//		List<Transform> sameBulletList = mEnemyBulletList[0];
//		for (int j = 0; j < sameBulletList.Count; j++) 
//		{
//			if (sameBulletList[j].gameObject.activeSelf) total++;
//		}
//		totalBulletsInPlay = total;

        if (mIsDisableSpawnBullet)
        {
            mDisableSpawnBulletTimer += Time.deltaTime;

            if (mDisableSpawnBulletTimer >= mDisableSpawnBulletTime)
            {
                mDisableSpawnBulletTimer = 0;
                mIsDisableSpawnBullet = false;
            }
        }
    }

    public void SetBulletTag(GroupIndex groupIndex, int index, string tag)
    {
        if (groupIndex == GroupIndex.PLAYER_MAIN)
        {
            for (int i = 0; i < mPlyMainBulletList[index].Count; i++)
            {
                mPlyMainBulletList[index][i].tag = tag;
            }
        }
        else if (groupIndex == GroupIndex.PLAYER_SECONDARY)
        {
            for (int i = 0; i < mPlySecondaryBulletList[index].Count; i++)
            {
                mPlySecondaryBulletList[index][i].tag = tag;
            }
        }
        else if (groupIndex == GroupIndex.ENEMY)
        {
            for (int i = 0; i < mEnemyBulletList[index].Count; i++)
            {
                mEnemyBulletList[index][i].tag = tag;
            }
        }
    }

    public bool IsDisableSpawnBullet { get { return mIsDisableSpawnBullet; } }

    public void InstantiateAndCacheBullet(Transform currBullet, int total, int groupIndex)
    {
        // Group name.
        GameObject go = new GameObject();
        go.name = currBullet.name;   

        List<Transform> sameBulletList = new List<Transform>();
        for (int i = 0; i < total; i++)
        {
            Transform trans = Instantiate(currBullet, Vector3.zero, currBullet.rotation);
            trans.name = currBullet.name;
            trans.SetParent(go.transform);
            trans.gameObject.SetActive(false);

            sameBulletList.Add(trans);

            // Set sort order for enemy bullets.
            SpriteRenderer transSr = trans.GetComponent<SpriteRenderer>();
            if (transSr != null && transSr.sortingLayerName == TagManager.sSingleton.sortLayerTopG) 
                transSr.sortingOrder = mOrderInLayer++;

            if (transSr == null)
            {
                transSr = trans.GetComponentInChildren<SpriteRenderer>();
                if (transSr != null && transSr.sortingLayerName == TagManager.sSingleton.sortLayerMidGTop2)
                    transSr.sortingOrder = mOrderInLayer++;
            }

            if (groupIndex == 0 || groupIndex == 1)
                trans.GetComponent<BulletMove>().SetPlayer();
        }

        if (groupIndex == 0) mPlyMainBulletList.Add(sameBulletList);
        else if(groupIndex == 1) mPlySecondaryBulletList.Add(sameBulletList);
        else mEnemyBulletList.Add(sameBulletList);

        mOrderInLayer = 0;
    }

    public void InstantiateAndCacheSparks(Transform currSpark, int total)
    {
        // Group name.
        GameObject go = new GameObject();
        go.name = currSpark.name;   

        List<Transform> sameSparkList = new List<Transform>();
        for (int i = 0; i < total; i++)
        {
            Transform trans = Instantiate(currSpark, Vector3.zero, currSpark.rotation);
            trans.name = currSpark.name;
            trans.SetParent(go.transform);
            trans.gameObject.SetActive(false);

            sameSparkList.Add(trans);

            // Set sort order for enemy bullets.
            SpriteRenderer transSr = trans.GetComponent<SpriteRenderer>();
            if (transSr != null) transSr.sortingOrder = i;
        }
        mEnemyBulletSparkList.Add(sameSparkList);
    }

    public void InstantiateAndCacheSkillBullet(Transform currBullet, int total)
    {
        // Group name.
        GameObject go = new GameObject();
        go.name = currBullet.name;   

        for (int i = 0; i < total; i++)
        {
            Transform trans = Instantiate(currBullet, Vector3.zero, currBullet.rotation);
            trans.name = currBullet.name;
            trans.SetParent(go.transform);
            trans.gameObject.SetActive(false);

            mPlySkillBulletList.Add(trans);

            // Set sort layer.
            SpriteRenderer transSr = trans.GetComponentInChildren<SpriteRenderer>();
            transSr.sortingLayerName = TagManager.sSingleton.sortLayerTopG2;
        }
    }

    public Transform GetBulletTrans(GroupIndex groupIndex, int index)
    {
        // Update index to the next one.
        int bulletIndex = 0, total = 0;
        if (groupIndex == GroupIndex.PLAYER_MAIN)
        {
            bulletIndex = mPlyCurrMainBulIndexList[index];
            total = mPlyMainBulletList[index].Count - 1;

            if (bulletIndex + 1 > total) mPlyCurrMainBulIndexList[index] = 0;
            else mPlyCurrMainBulIndexList[index]++;

            return mPlyMainBulletList[index][bulletIndex];
        }
        else if (groupIndex == GroupIndex.PLAYER_SECONDARY)
        {
            bulletIndex = mPlyCurrSecondBulIndexList[index];
            total = mPlySecondaryBulletList[index].Count - 1;

            if (bulletIndex + 1 > total) mPlyCurrSecondBulIndexList[index] = 0;
            else mPlyCurrSecondBulIndexList[index]++;

            return mPlySecondaryBulletList[index][bulletIndex];
        }
        else if (groupIndex == GroupIndex.ENEMY)
        {
            bulletIndex = mEnemyCurrBulIndexList[index];
            total = mEnemyBulletList[index].Count - 1;

            if (bulletIndex + 1 > total) mEnemyCurrBulIndexList[index] = 0;
            else mEnemyCurrBulIndexList[index]++;

            return mEnemyBulletList[index][bulletIndex];
        }

        return null;
    }

    public Transform GetBulletSpark(int index)
    {
        int sparkIndex = mEnemySparkIndexList[index];
        int total = mEnemyBulletSparkList[index].Count - 1;

        if (sparkIndex + 1 > total) mEnemySparkIndexList[index] = 0;
        else mEnemySparkIndexList[index]++;

        return mEnemyBulletSparkList[index][sparkIndex];
    }

    public Transform GetSkillBullet()
    {
        int total = mPlySkillBulletList.Count - 1;

        if (plySkillBulIndex + 1 > total) plySkillBulIndex = 0;
        else plySkillBulIndex++;

        return mPlySkillBulletList[plySkillBulIndex];
    }

    public void DisableEnemyBullets(bool isDisableSpawnBullet)
    {
        bool isPlaySfx = false;
        for (int i = 0; i < mEnemyBulletList.Count; i++)
        {
            List<Transform> sameBulletList = mEnemyBulletList[i];
            for (int j = 0; j < sameBulletList.Count; j++)
            {
                Transform currBullet = sameBulletList[j];
                if (!currBullet.gameObject.activeSelf) continue;

                SpriteRenderer sr = currBullet.GetComponent<SpriteRenderer>();
                if (sr == null) sr = currBullet.GetComponentInChildren<SpriteRenderer>();

                currBullet.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(IEAlphaOutSequence(sr));

                ParticleSystem ps = EnvObjManager.sSingleton.GetDisappearingBulletPS();
                ps.transform.position = currBullet.position;
                ps.Play();
                isPlaySfx = true;
            }
        }

        mIsDisableSpawnBullet = isDisableSpawnBullet;
        mDisableSpawnBulletTime = GameManager.sSingleton.enemyDisBulletTime;

        if (AudioManager.sSingleton != null && isPlaySfx) AudioManager.sSingleton.PlayBulletDisappearSfx();
    }

    public void TransformEnemyBulsIntoScorePU()
    {
        for (int i = 0; i < mEnemyBulletList.Count; i++)
        {
            List<Transform> sameBulletList = mEnemyBulletList[i];
            for (int j = 0; j < sameBulletList.Count; j++)
            {
                Transform currBullet = sameBulletList[j];
                if (currBullet.gameObject.activeSelf)
                {
					currBullet.gameObject.SetActive (false);
                    Vector3 pos = currBullet.position;

                    int rand = -1;
                    if (GameManager.sSingleton.TotalNumOfPlayer () == 1) 
                    {
                        if (GameManager.sSingleton.IsThisPlayerActive(1)) rand = 0;
                        else rand = 1;
                    }
                    else rand = Random.Range(0, 2);

                    EnvObjManager.sSingleton.TransformBulletIntoScorePU(pos, rand);
                }
            }
        }
    }

    public void TransformEnemyBulIntoPlayerBul(int playerID, Transform other, int bulletIndex)
	{
        Vector2 dir = EnemyManager.sSingleton.GetClosestEnemyDir(other);

        if (dir != Vector2.zero)
        {
//            Transform trans = GetBulletTrans (GroupIndex.PLAYER_MAIN, bulletIndex);
            Transform trans = GetSkillBullet();
    		trans.position = other.position;

            if (playerID == 1) trans.tag = TagManager.sSingleton.player1BulletTag;
            else if (playerID == 2) trans.tag = TagManager.sSingleton.player2BulletTag;

            int dmg = BombManager.sSingleton.bombShieldReturnDmg;
            float spd = BombManager.sSingleton.bombShieldReturnSpd;

            BulletMove bulletMove = trans.GetComponent<BulletMove>();
            bulletMove.SetProperties(AttackPattern.Template.SINGLE_SHOT, dmg, spd);
            bulletMove.SetPlayer();

            bulletMove.SetDirection(dir);
            trans.gameObject.SetActive (true);
        }
	}

    IEnumerator IEAlphaOutSequence (SpriteRenderer sr)
    {
        Color color = Color.white;
        while(sr.color.a > 0)
        {
            float deltaTime = 0;
            if (BombManager.sSingleton.isTimeStopBomb) deltaTime = Time.unscaledDeltaTime;
            else deltaTime = Time.deltaTime;

            color = sr.color;
            color.a -= deltaTime * GameManager.sSingleton.bulletDisappearSpeed;
            sr.color = color;

            yield return null;
        }

        // Reset the values back to default.
        color = sr.color;
        color.a = 1;
        sr.color = color;

        if (sr.gameObject.GetComponent<Collider2D>() != null)
        {
            sr.gameObject.GetComponent<Collider2D>().enabled = true;
            sr.gameObject.SetActive(false);
        }
        else
        {
            sr.transform.parent.GetComponent<Collider2D>().enabled = true;
            sr.transform.parent.gameObject.SetActive(false);
        }
    }
}
