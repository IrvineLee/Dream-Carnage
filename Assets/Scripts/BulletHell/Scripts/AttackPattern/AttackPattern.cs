using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPattern : MonoBehaviour 
{
    // Basic stats.
    public float duration;
    public bool isShowDuration = false;
    public float bulletDamage;
    public float bulletSpeed;
    public float shootDelay;
    public float onceStartDelay;
    public bool isDisBulletAftDone = false;

    public Sprite charSprite;
    public Sprite spellCardSprite;

    protected Transform mOwner;
    protected float mTimer = 0.0f;
    protected bool mIsDelay = false;
    protected bool mIsCoroutine = false;

    public AttackPattern()
    {
        this.mOwner = null;
        this.duration = 0;
        this.bulletDamage = 1;
        this.bulletSpeed = 1;
        this.shootDelay = 0;
        this.onceStartDelay = 0;
    }

    public virtual void Start()
    {
        if (transform.parent.tag == TagManager.sSingleton.enemyTag)
            mOwner = transform.parent.transform;
        else
            mOwner = transform.parent.parent.transform;
    }

    public void ShowPotrait()
    {
        if (charSprite != null && spellCardSprite != null)
        {
            mOwner.GetComponent<EnemyBase>().EnableMagicCircle();
            PotraitShowManager.sSingleton.RunShowPotrait(charSprite, spellCardSprite);
        }
    }
}
