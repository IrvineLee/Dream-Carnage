using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackPattern : MonoBehaviour 
{
    // Base stats.
    public bool isPlayer = false;
    public Transform owner;
    public int mainBulletDamage = 1, secondaryBulletDamage = 1;
    public float mainBulletSpeed = 5, secondaryBulletSpeed = 5, shootDelay = 0.2f;
    public bool isMainPiercing = false, isSecondaryPiercing = false;

    // Bullet stats.
    public BulletPrefabData bulletPrefabData;
    public int mainBulletIndex, secondaryBulletIndex;

    // Player stats.
    public Vector2 bulletDirection, mainBulletOffset, secondaryBulletOffset;
    public float secondaryX_OffsetBetBul;

    // Enemy stats.
    public float duration = 10, onceStartDelay;
    public bool isShowDuration = false, isPotraitShow = false;
    public Sprite charSprite, spellCardSprite;
    //    public bool isDisBulletAftDone = false;

    // Pattern template.
    public enum Template
    {
        SINGLE_SHOT = 0,
        ANGLE_SHOT,
        SHOOT_AROUND_IN_CIRCLE,
        DOUBLE_SINE_WAVE,
    }
    public Template template = Template.SINGLE_SHOT;

    public enum SecondaryMoveTemplate
    {
        FROM_BACK = 0,
        CIRCLE_AROUND_PLAYER,
    }
    public SecondaryMoveTemplate secondaryMoveTemplate = SecondaryMoveTemplate.FROM_BACK;

    // Shoot at player values.
    public float initialSpacing;
    public int viewAngle = 90, segments;

    // Shoot around in circle values.
    public bool isClockwise = true;
    public float distance, startTurnDelay, turningRate, increaseTR, increaseTRTime, maxTR;
    public float xOffset, yOffset;

    [System.Serializable]
    public class UpdateSpeed
    {
        public float changeSpeedTime, toSpeed, toSpeedQuickness;
        public UpdateSpeed() { changeSpeedTime = toSpeed = toSpeedQuickness = 0; }
    }
    public List<UpdateSpeed> speedChangeList = new List<UpdateSpeed>();

    // Sine wave values.
    public Vector2 offsetPosition;
    public float frequency, magnitude, magExpandMult, sineWaveBullets, cooldown;

    // Values that are to be sent over to individual bullets.
    public class Properties
    {
        // Fixed value.
        public bool isPlayer, isMainPiercing, isSecondaryPiercing;
        public Template template;
        public int damage;
        public float speed, frequency, magnitude, magExpandMult;

        // Values that will be changed real-time.
        public Vector2 direction;
        public Vector2 curveAxis;

        public Properties()
        {
            isPlayer = isMainPiercing = isSecondaryPiercing = false;
            template = Template.SINGLE_SHOT;
            damage = 0;
            speed = frequency = magnitude = magExpandMult = 0;
            direction = curveAxis = Vector2.zero;
        }
    }
    Properties properties = new Properties();

    List<Transform> mSameMainBulletList = new List<Transform>();
    List<Transform> mSameSecondaryBulletList = new List<Transform>();

    float mTimer, mAngle, mIncreaseTRTimer, mSlowDownTimer;
    bool mIsCoroutine = false;

    List<IEnumerator> mAttackCoList = new List<IEnumerator>();
    PlayerController mPlayerController;
    SecondaryAttackType mSecondaryAttackType;

    // Used for testing purposes in editor-mode.
    Template mSavedTemplateList;
    int savedMainBulletIndex, savedSecondaryBulletIndex;

    void Start()
    {
        GetNewestSameBulletList();
        UpdateProperties();
        mSavedTemplateList = template;
    }

    void GetNewestSameBulletList()
    {
        savedMainBulletIndex = mainBulletIndex;

        if (isPlayer)
        {
            savedSecondaryBulletIndex = secondaryBulletIndex;

            mPlayerController = GetComponent<PlayerController>();
            mSecondaryAttackType = GetComponent<SecondaryAttackType>();

            string p1Tag = TagManager.sSingleton.player1Tag;
            string p2Tag = TagManager.sSingleton.player2Tag;

            if (transform.tag == p1Tag)
            {
                BulletManager.sSingleton.SetBulletTag(BulletManager.GroupIndex.PLAYER_MAIN, mainBulletIndex, TagManager.sSingleton.player1BulletTag);
                BulletManager.sSingleton.SetBulletTag(BulletManager.GroupIndex.PLAYER_SECONDARY, secondaryBulletIndex, TagManager.sSingleton.player1BulletTag);
            }
            else if (transform.tag == p2Tag)
            {
                BulletManager.sSingleton.SetBulletTag(BulletManager.GroupIndex.PLAYER_MAIN, mainBulletIndex, TagManager.sSingleton.player2BulletTag);
                BulletManager.sSingleton.SetBulletTag(BulletManager.GroupIndex.PLAYER_SECONDARY, secondaryBulletIndex, TagManager.sSingleton.player2BulletTag);
            }
        }
    }

    void Update()
    {
        if(Application.isEditor)
        {
            // Will only enter when user change the attack pattern in editor during play-mode.
            if (mSavedTemplateList != template)
            {
                for (int j = 0; j < mAttackCoList.Count; j++)
                {
                    StopCoroutine(mAttackCoList[j]);
                }
                mAttackCoList.Clear();
                mIsCoroutine = false;
                mTimer = 0;
                mSavedTemplateList = template;
            }
            if (savedMainBulletIndex != mainBulletIndex || savedSecondaryBulletIndex != secondaryBulletIndex)
                GetNewestSameBulletList();
            
            UpdateProperties();
        }
    }

    public void StartAttack(Transform target, Action doLast)
    {
        if (!mIsCoroutine)
        {
            if (isPotraitShow)
            {
                if (charSprite != null) owner.GetComponent<EnemyBase>().EnableMagicCircle();
                if (spellCardSprite != null) PotraitShowManager.sSingleton.RunShowPotrait(charSprite, spellCardSprite);
            }
            if (isShowDuration) UIManager.sSingleton.ActivateBossTimer(duration);

            if (template == AttackPattern.Template.SINGLE_SHOT || template == AttackPattern.Template.ANGLE_SHOT ||
                template == AttackPattern.Template.SHOOT_AROUND_IN_CIRCLE)
                mAttackCoList.Add(Shoot(target, doLast));
            else if (template == AttackPattern.Template.DOUBLE_SINE_WAVE)
                mAttackCoList = DoubleSineWaveShot(target, doLast);

            for (int i = 0; i < mAttackCoList.Count; i++)
            {
                StartCoroutine(mAttackCoList[i]);
            }
        }
    }

    public void PrimaryWeaponShoot()
    {
        if (template == Template.SINGLE_SHOT) ShootSingleShot(bulletDirection);
        else if (template == AttackPattern.Template.ANGLE_SHOT) ShootAngleShot(bulletDirection);
        else if (template == AttackPattern.Template.SHOOT_AROUND_IN_CIRCLE) ShootAroundInCirlce();
        else if (template == AttackPattern.Template.DOUBLE_SINE_WAVE)
        {
            SineWaveShoot(true);
            SineWaveShoot(false);
        }
    }

    public void SecondaryWeaponShoot()
    {
        int numOfMissle = Mathf.FloorToInt(mPlayerController.powerLevel);
        List<Vector3> posList = mSecondaryAttackType.GetPos(numOfMissle, secondaryBulletOffset, secondaryX_OffsetBetBul);

        for (int i = 0; i < numOfMissle; i++)
        {
            Transform currBullet = BulletManager.sSingleton.GetBulletTrans(BulletManager.GroupIndex.PLAYER_SECONDARY, secondaryBulletIndex);
            currBullet.position = posList[i];
            currBullet.gameObject.SetActive(true);

            BulletMove bulletMove = currBullet.GetComponent<BulletMove>();
            bulletMove.SetBaseProperties(properties);
            bulletMove.SetDirection(bulletDirection);
            bulletMove.SetProperties(Template.SINGLE_SHOT, secondaryBulletDamage, secondaryBulletSpeed);
        }
    }

    IEnumerator Shoot(Transform targetTrans, Action doLast)
    {
        mIsCoroutine = true;
        while (mTimer < duration)
        {
            while (onceStartDelay > 0)
            {
                if (!BombManager.sSingleton.isTimeStopBomb)
                {
                    mTimer += Time.deltaTime;
                    onceStartDelay -= Time.deltaTime;
                }
                yield return null;
            }

            if (!BulletManager.sSingleton.IsDisableSpawnBullet)
            {
                if(template == Template.SINGLE_SHOT) ShootSingleShot(targetTrans.position);
                else if(template == Template.ANGLE_SHOT) ShootAngleShot(targetTrans.position);
                else if(template == Template.SHOOT_AROUND_IN_CIRCLE) ShootAroundInCirlce();
            }

            mTimer += shootDelay + Time.deltaTime;
            yield return new WaitForSeconds(shootDelay);
        }
        doLast();
        mIsCoroutine = false;
    }

    List<IEnumerator> DoubleSineWaveShot(Transform targetTrans, Action doLast)
    {
        List<IEnumerator> ienumeratorList = new List<IEnumerator>();
        ienumeratorList.Add(SineWaveShootRoutine(targetTrans, true, () => {} )); 
        ienumeratorList.Add(SineWaveShootRoutine(targetTrans, false, doLast)); 
        return ienumeratorList;
    }

    IEnumerator SineWaveShootRoutine(Transform targetTrans, bool isStartLeft, Action doLast)
    {
        mIsCoroutine = true;
        float timer = 0;

        while (mTimer < duration)
        {
            while (onceStartDelay > 0)
            {
              if (!BombManager.sSingleton.isTimeStopBomb)
                {
                    mTimer += Time.deltaTime;
                    onceStartDelay -= Time.deltaTime;
                }
                yield return null;
            }

            for (int i = 0; i < sineWaveBullets; i++)
            {
                if (!BulletManager.sSingleton.IsDisableSpawnBullet) SineWaveShoot(targetTrans.position, isStartLeft);

                timer += shootDelay;
                yield return new WaitForSeconds(shootDelay);
            }
            yield return new WaitForSeconds(cooldown);

            timer += cooldown + Time.deltaTime;
            mTimer = timer;
        }
        doLast();
        mIsCoroutine = false;
    }

    void UpdateProperties()
    {
        properties.isPlayer = isPlayer;
        properties.isMainPiercing = isMainPiercing;
        properties.isSecondaryPiercing = isSecondaryPiercing;
        properties.template = template;
        properties.damage = mainBulletDamage;
        properties.speed = mainBulletSpeed;
        properties.frequency = frequency;
        properties.magnitude = magnitude;
        properties.magExpandMult = magExpandMult;
    }

    void ShootSingleShot(Vector3 targetPos)
    {
//        int bulIndex = 0;
        Transform bulletTrans = null;
        Vector2 dir = Vector2.zero;
        Vector2 startPos = (Vector2) owner.position;

        if (isPlayer)
        {
            bulletTrans = BulletManager.sSingleton.GetBulletTrans(BulletManager.GroupIndex.PLAYER_MAIN, mainBulletIndex);

            startPos.x += mainBulletOffset.x;
            startPos.y += (mPlayerController.PlayerSize.y / 2) + mainBulletOffset.y;
            dir = bulletDirection;
        }
        else
        {
            bulletTrans = BulletManager.sSingleton.GetBulletTrans(BulletManager.GroupIndex.ENEMY, mainBulletIndex);

            dir = (Vector2)(targetPos - owner.position).normalized;
            startPos += dir * initialSpacing;
        }

        Transform currBullet = bulletTrans;
        currBullet.position = (Vector3)startPos;

        float bulletAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        currBullet.rotation = Quaternion.AngleAxis(-90 + bulletAngle, Vector3.forward);
        currBullet.gameObject.SetActive(true);

        BulletMove bulletMove = currBullet.GetComponent<BulletMove>();
        SetProperties(ref bulletMove, dir);
    }

    void ShootAngleShot(Vector3 targetPos)
    {
        Vector2 targetDir = Vector2.zero;
        if (isPlayer) 
        {
            Vector3 temp = transform.position;
            temp.y = bulletDirection.y;
            targetDir = (Vector2)(temp - owner.transform.position).normalized;

            if (targetDir.y < 0) targetDir.y = Mathf.Abs(targetDir.y);
        }
        else targetDir = (Vector2)(targetPos - owner.transform.position).normalized;

        mAngle = Vector2.Angle(targetDir, transform.up) * Mathf.Deg2Rad;
        if (targetDir.x < 0)
            mAngle = -mAngle;

        float halfViewAngle = ((viewAngle * Mathf.Deg2Rad) / 2);
        float startAngle = mAngle - halfViewAngle;
        float endAngle = mAngle + halfViewAngle;

        //------------------------------Testing purposes---------------------------------
        //        float x = Mathf.Sin(startAngle);
        //        float y = Mathf.Cos(startAngle);
        //        Vector2 target = new Vector3(transform.position.x + x, transform.position.y + y);
        //        Debug.DrawLine (transform.position, target, Color.green);
        //
        //        x = Mathf.Sin(endAngle);
        //        y = Mathf.Cos(endAngle);
        //        target = new Vector3(transform.position.x + x, transform.position.y + y);
        //        Debug.DrawLine (transform.position, target, Color.green);
        //--------------------------------------------------------------------------------

        float inc = (viewAngle * Mathf.Deg2Rad) / segments;
        float total = startAngle;
        for (float i = 0; i < segments + 1; i++)
        {
            float angle = 0;
            if (i == 0) angle = startAngle;
            else if (i == segments) angle = endAngle;
            else 
            {
                total += inc;
                angle = total;
            }

            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);
            Vector2 target = new Vector3(owner.position.x + x, owner.position.y + y);
//            Debug.DrawLine(transform.position, target, Color.red);

            Transform currBullet = null;
            if (isPlayer) currBullet = BulletManager.sSingleton.GetBulletTrans(BulletManager.GroupIndex.PLAYER_MAIN, mainBulletIndex);
            else currBullet = BulletManager.sSingleton.GetBulletTrans(BulletManager.GroupIndex.ENEMY, mainBulletIndex);

            Vector3 dir = (target - (Vector2)transform.position).normalized;
            currBullet.position = (Vector3)target + (Vector3)dir * initialSpacing;

            float bulletAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            currBullet.rotation = Quaternion.AngleAxis(-90 + bulletAngle, Vector3.forward);
            currBullet.gameObject.SetActive(true);

            BulletMove bulletMove = currBullet.GetComponent<BulletMove>();
            SetProperties(ref bulletMove, dir);
        }
    }

    void ShootAroundInCirlce()
    {
        if (startTurnDelay <= 0)
        {
            mIncreaseTRTimer += Time.deltaTime;
            if (turningRate < maxTR && mIncreaseTRTimer >= increaseTRTime)
            {
                turningRate += increaseTR;
                if (turningRate > maxTR) turningRate = maxTR;
                mIncreaseTRTimer = 0;
            }
        }
        else startTurnDelay -= shootDelay - Time.deltaTime;

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Sin (mAngle + (xOffset * Mathf.Deg2Rad)) * distance;
            float y = Mathf.Cos (mAngle + (yOffset * Mathf.Deg2Rad)) * distance;
            mAngle += (Mathf.PI * 2 / segments);

            Vector2 dir = Vector2.zero;
            if (isClockwise) { dir.x += x;  dir.y += y; }
            else { dir.x += y; dir.y += x; }

            Transform currBullet = null;
            if (isPlayer) currBullet = BulletManager.sSingleton.GetBulletTrans(BulletManager.GroupIndex.PLAYER_MAIN, mainBulletIndex);
            else currBullet = BulletManager.sSingleton.GetBulletTrans(BulletManager.GroupIndex.ENEMY, mainBulletIndex);

            currBullet.position = owner.position;

            float bulletAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            currBullet.rotation = Quaternion.AngleAxis(-90 + bulletAngle, Vector3.forward);
            currBullet.gameObject.SetActive(true);

            BulletMove bulletMove = currBullet.GetComponent<BulletMove>();
            SetProperties(ref bulletMove, dir);
        }

        mAngle += (turningRate * Mathf.Deg2Rad);
    }

    void SineWaveShoot(bool isStartLeft) { SineWaveShoot(Vector3.zero, isStartLeft); }
    void SineWaveShoot(Vector3 targetPos, bool isStartLeft)
    {
        Transform currBullet = null;
        if (isPlayer) currBullet = BulletManager.sSingleton.GetBulletTrans(BulletManager.GroupIndex.PLAYER_MAIN, mainBulletIndex);
        else currBullet = BulletManager.sSingleton.GetBulletTrans(BulletManager.GroupIndex.ENEMY, mainBulletIndex);

        Vector3 temp = owner.position;
        temp.x += offsetPosition.x;
        temp.y += offsetPosition.y;
        currBullet.position = temp;
        currBullet.gameObject.SetActive(true);

        Vector2 dir = Vector2.zero;
        if (isPlayer) dir = bulletDirection;
        else dir = (Vector2) (targetPos - owner.transform.position).normalized;

        BulletMove bulletMove = currBullet.GetComponent<BulletMove>();
        SetProperties(ref bulletMove, dir);
        bulletMove.SetCurveAxis(GetCurveAxis(dir, isStartLeft));
    }

    void SetProperties(ref BulletMove bulletMove, Vector2 dir)
    {
        bulletMove.SetBaseProperties(properties);
        bulletMove.SetDirection(dir);
        bulletMove.SetNewBulletSpeed(speedChangeList);
    }

    Vector2 GetCurveAxis(Vector2 dir, bool isStartLeft)
    {
        Vector2 curveAxis = Vector2.zero;
        float xVal = 0, yVal = 0;

        if (dir.y < 0) xVal = -Mathf.Abs(dir.y);
        else xVal = Mathf.Abs(dir.y);

        if (dir.x < 0) yVal = Mathf.Abs(dir.x);
        else yVal = -Mathf.Abs(dir.x);

        if (isStartLeft) curveAxis = new Vector2(xVal, yVal);
        else curveAxis = new Vector2(-xVal, -yVal);

        return curveAxis;
    }
}
