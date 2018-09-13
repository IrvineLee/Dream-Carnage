using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LevelController : MonoBehaviour 
{
    public Image blackBg;
    public Image blackBgUnscaled;
    public Animator anim;
    public Animator animUnscaled;
    public bool isDestroyOnLoad = false;

    void Awake()
    {
        if (!isDestroyOnLoad) DontDestroyOnLoad(this.gameObject);
    }

    public IEnumerator Fading(string sceneName)
    {
        if (Time.timeScale == 1)
        {
            anim.SetBool("IsFade", true);
            yield return new WaitUntil(() => blackBg.color.a == 1);
        }
        else if (Time.timeScale == 0 && animUnscaled != null)
        {
            animUnscaled.SetBool("IsFade", true);
            yield return new WaitUntil(() => blackBgUnscaled.color.a == 1);
        }
        SceneManager.LoadSceneAsync(sceneName);
    }

    public IEnumerator Fading(string sceneName, Action doLast)
    {
        if (Time.timeScale == 1)
        {
            anim.SetBool("IsFade", true);
            yield return new WaitUntil(() => blackBg.color.a == 1);
        }
        else if (Time.timeScale == 0 && animUnscaled != null)
        {
            animUnscaled.SetBool("IsFade", true);
            yield return new WaitUntil(() => blackBgUnscaled.color.a == 1);
        }
        SceneManager.LoadSceneAsync(sceneName);
        doLast();
    }

    void OnDisable()
    {
        if (anim != null) anim.SetBool("IsFade", false);
    }
}
