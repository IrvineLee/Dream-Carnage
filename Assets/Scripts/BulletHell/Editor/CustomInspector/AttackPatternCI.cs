using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AttackPattern))]
public class AttackPatternCI : Editor 
{
    AttackPattern mSelf;

	void OnEnable () 
	{
        mSelf = (AttackPattern)target;
	}

	public override void OnInspectorGUI()
	{
        serializedObject.Update();

        // ----------------------------------------------- Base stat -------------------------------------------------
        GUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField("Base Stat");
        GUILayout.EndVertical ();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("isPlayer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("owner"));

        if (mSelf.isPlayer)
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Primary Weapon");
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("isMainPiercing"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mainBulletDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mainBulletSpeed"));

        if (mSelf.isPlayer) EditorGUILayout.PropertyField(serializedObject.FindProperty("mainBulletOffset"));

        if (!mSelf.isPlayer) EditorGUILayout.PropertyField(serializedObject.FindProperty("shootDelay")); 
        if (mSelf.isPlayer)
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Secondary Weapon");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isSecondaryPiercing"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryBulletDamage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryBulletSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryBulletOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryX_OffsetBetBul"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletDirection"));
        }

        // --------------------------------------------- Bullet Preview -----------------------------------------------
        GUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField("Bullet Preview");
        GUILayout.EndVertical ();

        if (BulletManager.sSingleton != null)
        {
            BulletPrefabData currBulletData = BulletManager.sSingleton.bulletPrefabData;

            Texture2D mainTexture = null, secondaryTexture = null;
            if (mSelf.isPlayer)
            {
                Transform mainBullet = currBulletData.plyMainBulletTransList[mSelf.mainBulletIndex];
                Transform secondaryBullet = currBulletData.plySecondaryBulletTransList[mSelf.secondaryBulletIndex];

                mainTexture = mainBullet.GetComponent<SpriteRenderer>().sprite.texture;
                secondaryTexture = secondaryBullet.GetComponent<SpriteRenderer>().sprite.texture;
            }
            else mainTexture = currBulletData.enemyBulletTransList[mSelf.mainBulletIndex].GetComponent<SpriteRenderer>().sprite.texture;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (mSelf.isPlayer)
                {
                    GUILayout.Space(20);
                    GUILayout.Label("Primary bullet");
                    GUILayout.Space(45);
                    GUILayout.Label("Secondary bullet");
                }
                else
                {
                    GUILayout.Space(15);
                    GUILayout.Label("Primary bullet");
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Space(10);

                // --------------------------------------- Primary bullet. --------------------------------------- 
                GUILayout.BeginVertical();
                GUILayout.Space(20);
                if(mSelf.mainBulletIndex - 1 < 0) GUI.enabled = false;
                if (GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(30))) mSelf.mainBulletIndex--;
                GUILayout.EndVertical();

                EditorGUILayout.ObjectField(mainTexture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));

                int max = 0;
                if (mSelf.isPlayer) max = currBulletData.plyMainBulletTransList.Count - 1;
                else max = currBulletData.enemyBulletTransList.Count - 1;

                if(mSelf.mainBulletIndex + 1 > max) GUI.enabled = false;
                else GUI.enabled = true;

                GUILayout.BeginVertical();
                GUILayout.Space(20);
                if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(30))) mSelf.mainBulletIndex++;
                GUI.enabled = true;
                GUILayout.EndVertical();

                // --------------------------------------- Secondary bullet ---------------------------------------
                if (mSelf.isPlayer)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Space(20);
                    if(mSelf.secondaryBulletIndex - 1 < 0) GUI.enabled = false;
                    if (GUILayout.Button("<", GUILayout.Width(30), GUILayout.Height(30))) mSelf.secondaryBulletIndex--;
                    GUILayout.EndVertical();

                    EditorGUILayout.ObjectField(secondaryTexture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));

                    if(mSelf.secondaryBulletIndex + 1 > currBulletData.plySecondaryBulletTransList.Count - 1) GUI.enabled = false;
                    else GUI.enabled = true;

                    GUILayout.BeginVertical();
                    GUILayout.Space(20);
                    if (GUILayout.Button(">", GUILayout.Width(30), GUILayout.Height(30))) mSelf.secondaryBulletIndex++;
                    GUI.enabled = true;
                    GUILayout.EndVertical();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (mSelf.isPlayer)
                {
                    GUILayout.Space(12);
                    GUILayout.Label((mSelf.mainBulletIndex + 1).ToString() + " / " + currBulletData.plyMainBulletTransList.Count.ToString());
                    GUILayout.Space(110);
                    GUILayout.Label((mSelf.secondaryBulletIndex + 1).ToString() + " / " + currBulletData.plySecondaryBulletTransList.Count.ToString());
                }
                else
                {
                    GUILayout.Space(10);
                    GUILayout.Label((mSelf.mainBulletIndex + 1).ToString() + " / " + currBulletData.enemyBulletTransList.Count.ToString());
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            // ---------------------------------------------------------------------------------------------------------------------
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
        }

        // ----------------------------------------- Current attack pattern -------------------------------------------
        GUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField("Attack Template");
        GUILayout.EndVertical();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("template"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryMoveTemplate"));

        if (mSelf.template == AttackPattern.Template.SINGLE_SHOT)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("initialSpacing"));
        }
        else if (mSelf.template == AttackPattern.Template.ANGLE_SHOT)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("initialSpacing"));

            if (mSelf.viewAngle < 0) mSelf.viewAngle = 0; if (mSelf.viewAngle > 360) mSelf.viewAngle = 360;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("viewAngle"));

            if (mSelf.segments < 1) mSelf.segments = 1;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("segments"));
        }
        else if (mSelf.template == AttackPattern.Template.SHOOT_AROUND_IN_CIRCLE)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isClockwise"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("distance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("segments"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startTurnDelay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("turningRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("increaseTR"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("increaseTRTime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTR"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("xOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("yOffset"));
        }
        else if (mSelf.template == AttackPattern.Template.DOUBLE_SINE_WAVE)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("offsetPosition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frequency"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("magnitude"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("magExpandMult"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sineWaveBullets"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown"));
        }

        if (!mSelf.isPlayer)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speedChangeList"), true);
            GUILayout.BeginVertical("HelpBox");
            EditorGUILayout.LabelField("Enemy Stat");
            GUILayout.EndVertical ();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onceStartDelay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isPotraitShow"));

            if (mSelf.isPotraitShow)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("charSprite"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spellCardSprite"));
            }
        }

        serializedObject.ApplyModifiedProperties();
		if (GUI.changed) EditorUtility.SetDirty(target); 
	}
}