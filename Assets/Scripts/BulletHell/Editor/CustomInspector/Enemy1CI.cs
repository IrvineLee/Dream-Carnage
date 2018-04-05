//using UnityEngine;
//using UnityEditor;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//
//[CustomEditor(typeof(Enemy1))]
//public class Enemy1CI : Editor 
//{
//    Enemy1 mSelf;
//
//    bool mIsShowAP = true;
//    List<bool> mIsShowAPList = new List<bool>();
//    bool mIsChangedAP = false;
//
//    void OnEnable () 
//    {
//        mSelf = (Enemy1)target;
//        Initialize();
//    }
//
//    public override void OnInspectorGUI()
//    {
//        mSelf.hitPoint = EditorGUILayout.IntField("Hit Point", mSelf.hitPoint);
//        mSelf.moveSpeed = EditorGUILayout.FloatField("Move Speed", mSelf.moveSpeed);
//        mSelf.anim = (Animator) EditorGUILayout.ObjectField("Animator", mSelf.anim, typeof(Animator), false);
//
//        mIsShowAP = EditorGUILayout.Foldout(mIsShowAP, "Attack Pattern List", true);
//        if (mIsShowAP)
//        {
//            EditorGUI.indentLevel++;
//
//            List<Enemy1.AttackPattern> attackPatternList = mSelf.attackPatternList;
//            int count = attackPatternList.Count;
//
//            for (int i = 0; i < count; i++)
//            {
////                EditorGUI.indentLevel++;
//                mIsShowAPList[i] = EditorGUILayout.Foldout(mIsShowAPList[i], "Attack Pattern " + (i + 1), true);
//                if (mIsShowAPList[i])
//                {
//                    EditorGUI.indentLevel++;
//
////                    EditorGUI.BeginChangeCheck();
////                    attackPatternList[i].currState = (Enemy1.AttackPattern.State) EditorGUILayout.EnumPopup ("State", attackPatternList[i].currState);
////                    if (EditorGUI.EndChangeCheck()) mIsChangedAP = true;
////                    Debug.Log(attackPatternList[i].attackPattern);
////                    attackPatternList[i].attackPattern = (Enemy1.AttackPattern) EditorGUILayout.ObjectField("Type", attackPatternList[i].attackPattern, typeof(Enemy1.AttackPattern));
////                    mSelf.characterData = (CharacterData) EditorGUILayout.ObjectField("CharacterData : ",mSelf.characterData, typeof(CharacterData), true);
//                    attackPatternList[i].duration = EditorGUILayout.FloatField("Duration", attackPatternList[i].duration);
//
////                    if (attackPatternList[i].currState == Enemy1.AttackPattern.State.NONE)
////                    {
////                        if (mIsChangedAP)
////                        {
////                            Enemy1.AttackPattern none = new Enemy1.AttackPattern();
////                            mSelf.ChangeAttackPattern(i, none);
////                            mIsChangedAP = false;
////                        }
////                    }
////                    else if (attackPatternList[i].currState == Enemy1.AttackPattern.State.SHOOT_IN_CIRCLE)
////                    {
////                        if (mIsChangedAP)
////                        {
////                            Enemy1.ShootAroundInCirlce shootAroundCircle = new Enemy1.ShootAroundInCirlce();
////                            mSelf.ChangeAttackPattern(i, shootAroundCircle);
////                            mIsChangedAP = false;
////                        }
////
////                        if (attackPatternList[i] is Enemy1.ShootAroundInCirlce)
////                        {
////                            Enemy1.ShootAroundInCirlce downCast = attackPatternList[i] as Enemy1.ShootAroundInCirlce;
////                            downCast.bulletSpeed = EditorGUILayout.FloatField("BulletSpeed", downCast.bulletSpeed);
////                            downCast.distance = EditorGUILayout.FloatField("Distance", downCast.distance);
////                            downCast.segments = EditorGUILayout.IntField("Segments", downCast.segments);
////                            downCast.turningRate = EditorGUILayout.IntField("TurningRate", downCast.turningRate);
////                            downCast.shootDelay = EditorGUILayout.FloatField("ShootDelay", downCast.shootDelay);
////                            attackPatternList[i] = (Enemy1.AttackPattern) downCast;
////                        }
////                    }
////                    else if (attackPatternList[i].currState == Enemy1.AttackPattern.State.SINE_WAVE)
////                    {
////                        if (mIsChangedAP)
////                        {
////                            Enemy1.SineWave sineWave = new Enemy1.SineWave();
////                            mSelf.ChangeAttackPattern(i, sineWave);
////                            mIsChangedAP = false;
////                        }
////
////                        if (attackPatternList[i] is Enemy1.SineWave)
////                        {
////                            Enemy1.SineWave downCast = attackPatternList[i] as Enemy1.SineWave;
////                            downCast.bulletSpeed = EditorGUILayout.FloatField("BulletSpeed", downCast.bulletSpeed);
////                            downCast.distance = EditorGUILayout.FloatField("Distance", downCast.distance);
////                            downCast.frequency = EditorGUILayout.FloatField("Frequency", downCast.frequency);
////                            downCast.magnitude = EditorGUILayout.FloatField("Magnitude", downCast.magnitude);
////                            downCast.shootDelay = EditorGUILayout.FloatField("ShootDelay", downCast.shootDelay);
////                            attackPatternList[i] = (Enemy1.AttackPattern) downCast;
////                        }
////                    }
//
//                    EditorGUI.indentLevel--;
//                }
////                EditorGUI.indentLevel--;
//            }
//            EditorGUI.indentLevel--;
//        }
//
//
//        EditorGUILayout.BeginHorizontal ();
//        GUILayout.FlexibleSpace();
//        if (GUILayout.Button("Add", GUILayout.Width(40))) mSelf.AddAttackPattern();
//        else if (GUILayout.Button("Del", GUILayout.Width(40))) mSelf.DelAttackPattern();
//        EditorGUILayout.EndHorizontal ();
//    }
//
//    void Initialize()
//    {
//        for (int i = 0; i < mSelf.attackPatternList.Count; i++)
//        {
////            Debug.Log(mSelf.attackPatternList[i] is Enemy1.SineWave);
////            Debug.Log(mSelf.attackPatternList[i] is Enemy1.ShootAroundInCirlce);
//            mIsShowAPList.Add(true);
//        }
//    }
//}
