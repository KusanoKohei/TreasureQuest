using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    UserData Userdata => SaveSystem.instance.UserData;
    PlayerManager Player => PlayerManager.instance;
    public void OnTapSaveButton()
    {
        Userdata.level          = Player.Level;
        Userdata.maxHP          = Player.MaxHP;
        Userdata.hp             = Player.Hp;
        Userdata.atk            = Player.Atk;
        Userdata.spd            = Player.Spd;
        Userdata.dodge          = Player.Dodge;
        Userdata.critical       = Player.Critical;
        Userdata.skill          = Player.Skill;
        Userdata.nextEXP        = Player.NextEXP;
        Userdata.nowEXP         = Player.NowEXP;
        Userdata.kurikoshi      = Player.Kurikoshi;

        SaveSystem.instance.Save();

        SoundManager.instance.PlayButtonSE(0);  // ボタンのクリック音.
        DialogTextManager.instance.SetScenarios(new string[] { "セーブしました" });
    }
}
