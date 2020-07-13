using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData 
{
    SettingManager SettingManager => SettingManager.instance;


    public int level;
    public int maxHP;
    public int hp;
    public int atk;
    public int spd;
    public int dodge;
    public int critical;
    public int skill;

    public int nextEXP;
    public int nowEXP;
    public int kurikoshi;

    public bool isCleared = false;

    public float messageSpeed;
    public float BGMvolume;
    public float SEvolume;
}
