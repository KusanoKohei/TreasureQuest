using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    public Text text;

    UserData Userdata => SaveSystem.instance.UserData;
    PlayerManager Player => PlayerManager.instance;
    public void OnTapLoadButton()
    {
        Userdata.messageSpeed = SettingManager.instance.MessageSpeed;
        Userdata.BGMvolume = SoundManager.instance.audioSourceBGM.volume;
        Userdata.SEvolume = SoundManager.instance.audioSourceSE.volume;

        SaveSystem.instance.Save();

        SaveSystem.instance.Load();

        Player.Level    = Userdata.level;
        Player.MaxHP    = Userdata.maxHP;
        Player.Hp       = Userdata.hp;
        Player.Atk      = Userdata.atk;
        Player.Spd      = Userdata.spd;
        Player.Dodge    = Userdata.dodge;
        Player.Critical = Userdata.critical;
        Player.Skill    = Userdata.skill;
        Player.NextEXP  = Userdata.nextEXP;
        Player.NowEXP   = Userdata.nowEXP;
        Player.Kurikoshi = Userdata.kurikoshi;

        SettingManager.instance.MessageSpeed        = Userdata.messageSpeed;
        SoundManager.instance.audioSourceBGM.volume = Userdata.BGMvolume;
        SoundManager.instance.audioSourceSE.volume  = Userdata.SEvolume;

        SoundManager.instance.PlayButtonSE(0);  // ボタンのクリック音.
    }
}
