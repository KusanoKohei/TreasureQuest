using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    private SaveSystem SaveSystem => SaveSystem.instance;
    private UserData UserData => SaveSystem.UserData;
    private PlayerManager Player => PlayerManager.instance;
    SceneTransitionManager sceneManager => SceneTransitionManager.instance;


    public void OnClick()
    {
        Player.Level = 1;

        PlayerManager.instance.Init_playerParameter();

        UserData.level = Player.Level;
        UserData.maxHP = Player.MaxHP;
        UserData.hp = Player.Hp;
        UserData.atk = Player.Atk;
        UserData.spd = Player.Spd;
        UserData.dodge = Player.Dodge;
        UserData.critical = Player.Critical;
        UserData.skill = Player.Skill;
        UserData.nextEXP = Player.NextEXP;
        UserData.nowEXP = Player.NowEXP;
        UserData.kurikoshi = Player.Kurikoshi;

        UserData.messageSpeed = SettingManager.instance.MessageSpeed;
        UserData.BGMvolume = SoundManager.instance.audioSourceBGM.volume;
        UserData.SEvolume = SoundManager.instance.audioSourceSE.volume;

        // ゲームクリアフラグを無効にする.
        UserData.isCleared = false;

        // -----------------

        SaveSystem.Save();



        SoundManager.instance.PlayButtonSE(0);  // ボタンのクリック音.

        sceneManager.LoadTo("Town");

        Debug.Log("ニューゲームボタンが押されている");
    }
}
