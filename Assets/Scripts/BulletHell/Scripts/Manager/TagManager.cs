using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour 
{
    public static TagManager sSingleton;

    // Visual novel tags.
    public string dialogueBoxTag = "DialogueBox";
    public string leftCharacterTag = "LeftCharacter";
    public string rightCharacterTag = "RightCharacter";
    public string answerBoxTag = "AnswerBox";
    public string answerBoxBgTag = "AnswerBoxBg";

    // Touhou tags.
    public string gmTag = "GameManager";
    public string player1Tag = "Player";
    public string player2Tag = "Player2";
    public string hitboxTag = "Hitbox";
    public string player1BulletTag = "Player1Bullet";
    public string player2BulletTag = "Player2Bullet";
    public string enemyTag = "Enemy";
    public string enemyBulletTag = "EnemyBullet";
    public string attackPatternTag = "AttackPattern";

    // Tags for UI.
    public string UI_LifePointTag = "LifePoint";
    public string UI_PowerLevelTag = "PowerLevel";
    public string UI_BombTag = "Bomb";
    public string UI_HighScoreTag = "HighScore";
    public string UI_ScoreTag = "Score";

    // Tags for environmental objects.
    public string ENV_OBJ_PowerUpTag = "PowerUp";
    public string ENV_OBJ_ScorePickUpTag = "ScorePickUp";
    public string ENV_OBJ_DamagePlayerTag = "DamagePlayer";

    // Touhou names.
    public string player1Bullet = "Player1Bullet";
    public string player2Bullet = "Player2Bullet";
    public string enemy1Bullet = "Enemy1Bullet";

    // Layers
    public string playerBulletLayer = "PlayerBullet";

    // Sorting layers.
    public string sortLayerTopG = "TopGround";

    void Awake()
    {
        sSingleton = this;
//        player1Name = GameManager.sSingleton.player1.name;
//        player2Name = GameManager.sSingleton.player2.name;
//        enemy1Name =
    }
}
