using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{
    public static UIManager sSingleton { get { return _sSingleton; } }
    static UIManager _sSingleton;

    public Transform player1_UI;
    public Transform player2_UI;
    public Transform timerUI;

    public float countUpScoreSpd = 10;
    public int maxPauseButton = 3;

    class PlayerInfo
    {
        public Transform lifePointTrans, bombTrans, pauseTrans;
        public Text powerLevel_UI, highScore_UI, score_UI, percent_UI;
        public Image linkBarImage;

        // Used for coroutine count-up animation.
        public bool isCoroutine = false;
        public int currScore, toReachScore;

        public PlayerInfo()
        {
            lifePointTrans = null;
            bombTrans = null;
            pauseTrans = null;
            powerLevel_UI = null;
            highScore_UI = null;
            score_UI = null;
            percent_UI = null;
            linkBarImage = null;
        }

        public PlayerInfo(Transform lifePointTrans, Transform bombTrans, Transform pauseTrans, Text powerLevel, Text highScore, Text score, 
            Image linkBarImage, Text percent_UI)
        {
            this.lifePointTrans = lifePointTrans;
            this.bombTrans = bombTrans;
            this.pauseTrans = pauseTrans;
            this.powerLevel_UI = powerLevel;
            this.highScore_UI = highScore;
            this.score_UI = score;
            this.linkBarImage = linkBarImage;
            this.percent_UI = percent_UI;
        }
    }
    List<PlayerInfo> playerUIList = new List<PlayerInfo>();

    Text mSecondText, mMilliSecondText;
    float mDuration = 0;

    int mCurrPlayerNum = 0, mPauseSelectIndex = 0;
    bool mIsPauseMenu = false;

    void Awake()
    {
        if (_sSingleton != null && _sSingleton != this) Destroy(this.gameObject);
        else _sSingleton = this;
    }

	void Start () 
    {
        int totalPlayer = GameManager.sSingleton.TotalNumOfPlayer();
        for (int i = 0; i < totalPlayer; i++)
        { 
            playerUIList.Add(new PlayerInfo()); 

            Transform playerUI = player1_UI;
            if (i == 1) playerUI = player2_UI;

            InitializeUI(playerUI, i);
        }

        for (int i = 0; i < timerUI.childCount; i++)
        {
            Text textScript = timerUI.GetChild(i).GetComponent<Text>();

            if (i == 0) mSecondText = textScript;
            else if(i == 1)mMilliSecondText = textScript;
        }
        timerUI.gameObject.SetActive(false);
	}

    void Update()
    {
        if (mDuration != 0)
        {
            mDuration -= Time.deltaTime;
            if (mDuration < 0) mDuration = 0;
            UpdateBossTimer(mDuration);
        }

        if (mIsPauseMenu)
        {
            Transform pauseTrans = playerUIList[mCurrPlayerNum].pauseTrans;
            if (Input.GetKeyDown(KeyCode.UpArrow) && mPauseSelectIndex > 0)
            {
                mPauseSelectIndex--;
                pauseTrans.GetChild(mPauseSelectIndex).GetComponent<Image>().color = Color.red;
                pauseTrans.GetChild(mPauseSelectIndex + 1).GetComponent<Image>().color = Color.white;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && mPauseSelectIndex < maxPauseButton - 1)
            {
                mPauseSelectIndex++;
                pauseTrans.GetChild(mPauseSelectIndex).GetComponent<Image>().color = Color.red;
                pauseTrans.GetChild(mPauseSelectIndex - 1).GetComponent<Image>().color = Color.white;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (mPauseSelectIndex == 0) DisablePauseScreen();
                else if (mPauseSelectIndex == 1) RestartLevel();
                else if (mPauseSelectIndex == 2) ReturnToTitleScreen();
            }
        }
    }

    public bool IsPauseMenu{ get { return mIsPauseMenu; } }

    public void EnablePauseScreen(int playerNum)
    {
        mIsPauseMenu = true;
        Time.timeScale = 0;
        mCurrPlayerNum = playerNum - 1;

        Transform pauseTrans = playerUIList[mCurrPlayerNum].pauseTrans;
        pauseTrans.GetChild(mPauseSelectIndex).GetComponent<Image>().color = Color.red;
        pauseTrans.gameObject.SetActive(true);
    }

    public void DisablePauseScreen()
    {
        Transform pauseTrans = playerUIList[mCurrPlayerNum].pauseTrans;
        pauseTrans.GetChild(mPauseSelectIndex).GetComponent<Image>().color = Color.white;
        pauseTrans.gameObject.SetActive(false);

        mCurrPlayerNum = 0;
        mPauseSelectIndex = 0;
        Time.timeScale = 1;
        mIsPauseMenu = false;
    }

    public void UpdateLife(int playerNum, int currLife)
    {
        for (int i = 0; i < GameManager.sSingleton.plyMaxLife; i++)
        {
            Transform currTrans = playerUIList[playerNum - 1].lifePointTrans.GetChild(i);

            if (currLife > 0) currTrans.gameObject.SetActive(true);
            else currTrans.gameObject.SetActive(false);

            currLife--;
        }
    }

    public void UpdatePower(int playerNum, float currPower, float maxPower)
    {
        string temp = currPower.ToString("F2") + " / " + maxPower.ToString("F2");
        playerUIList[playerNum - 1].powerLevel_UI.text = temp;
    }

    public void UpdateScore(int player, int score)
    {
        PlayerInfo currPlayer = playerUIList[player - 1];
        currPlayer.toReachScore = score;

        if(!currPlayer.isCoroutine) StartCoroutine(AddScoreSequence(player));
    }

    public void UpdateBomb(int player, int currBomb)
    {
        for (int i = 0; i < GameManager.sSingleton.plyMaxBomb; i++)
        {
            Transform currTrans = playerUIList[player - 1].bombTrans.GetChild(i);

            if (currBomb > 0) currTrans.gameObject.SetActive(true);
            else currTrans.gameObject.SetActive(false);

            currBomb--;
        }
    }

    public void UpdateLinkBar(int player, float linkVal)
    {
        PlayerInfo currPlayerInfo = playerUIList[player - 1];
        currPlayerInfo.linkBarImage.fillAmount = linkVal;

        if (linkVal >= 1) currPlayerInfo.percent_UI.text = "MAX";
        else
        {
            Text percent_UI = currPlayerInfo.percent_UI;
            string valStr = (linkVal / 1 * 100).ToString("F1");

            percent_UI.text = valStr + "%";

            int valInt = (int)float.Parse(valStr);
            if (valInt == 100) percent_UI.text = "99.9%";
        }
    }

    public void ActivateBossTimer(float duration)
    {
        timerUI.gameObject.SetActive(true);
        mDuration = duration;
        UpdateBossTimer(mDuration);
    }

    void InitializeUI(Transform playerUI, int index)
    {
        for (int i = 0; i < playerUI.childCount; i++)
        {
            Transform currChild = playerUI.GetChild(i);
            if (currChild.name == TagManager.sSingleton.UI_HighScoreName) playerUIList[index].highScore_UI = currChild.GetComponent<Text>();
            else if (currChild.name == TagManager.sSingleton.UI_ScoreName) playerUIList[index].score_UI = currChild.GetComponent<Text>();
            else if (currChild.name == TagManager.sSingleton.UI_LifePointName) playerUIList[index].lifePointTrans = currChild;
            else if (currChild.name == TagManager.sSingleton.UI_BombName) playerUIList[index].bombTrans = currChild;
            else if (currChild.name == TagManager.sSingleton.UI_PowerLevelName) playerUIList[index].powerLevel_UI = currChild.GetComponent<Text>();
            else if (currChild.name == TagManager.sSingleton.UI_LinkBarName) 
            {
                for (int j = 0; j < currChild.childCount; j++)
                {
                    Transform currGrandChild = currChild.GetChild(j);

                    if (currGrandChild.name == TagManager.sSingleton.UI_LinkBarInsideName)
                        playerUIList[index].linkBarImage = currGrandChild.GetComponent<Image>();
                    else if (currGrandChild.name == TagManager.sSingleton.UI_LinkMaxName)
                        playerUIList[index].percent_UI = currGrandChild.GetComponent<Text>();
                }
            }
            else if (currChild.name == TagManager.sSingleton.UI_PauseMenuName) playerUIList[index].pauseTrans = currChild;
        }

        for (int i = 0; i < GameManager.sSingleton.plyStartLife; i++)
        { playerUIList[index].lifePointTrans.GetChild(i).gameObject.SetActive(true); }

        for (int i = 0; i < GameManager.sSingleton.plyStartBomb; i++)
        { playerUIList[index].bombTrans.GetChild(i).gameObject.SetActive(true); }
    }

    void UpdateBossTimer(float duration)
    {
        float seconds = (int)duration;
        mSecondText.text = seconds.ToString();

        float milliSeconds = duration % 1;
        string milliSecStr = milliSeconds.ToString("F2");

        char c1 = milliSecStr[2];
        char c2 = milliSecStr[3];
        milliSecStr = c1.ToString() + c2.ToString();

        mMilliSecondText.text = milliSecStr;

        if(duration == 0) timerUI.gameObject.SetActive(false);
    }

    void RestartLevel()
    {

    }

    void ReturnToTitleScreen()
    {

    }

    string GetScoreWithZero(string score)
    {
        int addZero = playerUIList[0].score_UI.text.Length - score.Length;

        for (int i = 0; i < addZero; i++)
        { score = "0" + score; }

        return score;
    }

    IEnumerator AddScoreSequence(int player)
    {
        PlayerInfo currPlayer = playerUIList[player - 1];
        currPlayer.isCoroutine = true;

        while (currPlayer.currScore < currPlayer.toReachScore)
        {
            if (!mIsPauseMenu)
            {
                currPlayer.currScore += (int)(1 * countUpScoreSpd);
                if (currPlayer.currScore > currPlayer.toReachScore) currPlayer.currScore = currPlayer.toReachScore;

                currPlayer.score_UI.text = GetScoreWithZero(currPlayer.currScore.ToString());
                yield return new WaitForEndOfFrame();
            }
            else yield return null;
        }
        currPlayer.isCoroutine = false;
    }
}
