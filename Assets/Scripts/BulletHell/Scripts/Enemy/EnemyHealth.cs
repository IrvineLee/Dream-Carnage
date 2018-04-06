using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour 
{
    public Transform enemyTrans;
    public float refillBarDuration = 1.5f;

    float mRefillBarTimer;
    Image mHpBar;

	void Start () 
    {
        if (enemyTrans == null)
            Debug.Log("Enemy health transform is null.");

        mHpBar = GetComponent<Image>();
        StartCoroutine(RefillBarSequence(refillBarDuration));
	}
	
	void Update () 
    {
        if (enemyTrans == null) return;
        transform.position = enemyTrans.position;
	}

    public void UpdateHpBarUI(int currHp, int totalHp)
    {
        float val = (float)currHp / (float)totalHp;
        mHpBar.fillAmount = val;
    }

    public void RefillHpBarUI()
    {
        RefillBarSequence(refillBarDuration);
    }

    public void RefillHpBarUI(float duration)
    {
        RefillBarSequence(duration);
    }

    IEnumerator RefillBarSequence(float duration)
    {
        float val = 0;
        while(mRefillBarTimer < duration)
        {
            mRefillBarTimer += Time.deltaTime;
            val = mRefillBarTimer / duration;
            if (val > 1) val = 1;
            mHpBar.fillAmount = val;
            yield return null;
        }
    }
}
