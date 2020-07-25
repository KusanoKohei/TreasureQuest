using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName ="MyGame/Create ParameterTable", fileName="ParameterTable")]
public class Level_ParameterManager : ScriptableObject
{
    /*
    // キャラクターの名前.
    [SerializeField]
    private string characterName = "プレイヤー";
    // キャラクターのレベル.
    [SerializeField]
    private int level = 1;
    // 最大HP.
    [SerializeField]
    private int maxHP = 50;
    // HP.
    [SerializeField]
    private int hp = 50;
    // ATK.
    [SerializeField]
    private int atk = 12;
    */

    public static int[,] playerLevel = new int[,]
    {
        //  0      1     2   3    4    5      6         7      8        9       10.
        // level, maxHP, HP, ATK, SPD, dodge, critical, skill, nextEXP, nowEXP, kurikoshi
        {   1,    100,  100,  15, 5,   0,     0,        0,     20,      0,      0},
        {   2,    150,  150,  22, 7,   1,     1,        1,     25,      0,      0},
        {   3,    200,  200,  30, 10,  1,     1,        2,     50,      0,      0},
        {   4,    250,  250,  48, 15,  2,     2,        2,     100,     0,      0},
        {   5,    300,  300,  65, 20,  2,     2,        2,     0,       0,      0},
    };
}
