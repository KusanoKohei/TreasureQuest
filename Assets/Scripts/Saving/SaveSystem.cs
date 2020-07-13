using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System;

public class SaveSystem
{
    // 参照元: https://teratail.com/questions/196913?link=qa_related_pc
    //https://moon-bear.com/2019/03/23/%E3%80%90unity%E3%80%91json%E3%82%92%E4%BD%BF%E3%81%A3%E3%81%9F%E3%82%BB%E3%83%BC%E3%83%96%E3%83%BB%E3%83%AD%E3%83%BC%E3%83%89%E5%87%A6%E7%90%86/
  
    #region Singleton
    public static SaveSystem instance = new SaveSystem();
    #endregion

    public static string SAVE_KEY;

    private UserData userData;
    public UserData UserData { get => userData; set => userData = value; }

    private PlayerManager Player => PlayerManager.instance;

    // public string Path => Application.dataPath + "/data.json";  // セーブデータのファイルをAssetフォルダに置く（製作のしやすさから）.
    // public string Path => Application.persistentDataPath + "/"+".savedata.json";  // 保存場所は、Windows, C:/Users/xxxx/AppData/LocalLow/CompanyName/ProductName .


    void Awake()
    {
        userData = new UserData();
    }
    

    /// <summary>
    /// セーブ.
    /// </summary>
    public void Save()
    {
        UserData.level      = Player.Level;
        UserData.maxHP      = Player.MaxHP;
        UserData.hp         = Player.Hp;
        UserData.atk        = Player.Atk;
        UserData.spd        = Player.Spd;
        UserData.dodge      = Player.Dodge;
        UserData.critical   = Player.Critical;
        UserData.skill      = Player.Skill;
        UserData.nextEXP    = Player.NextEXP;
        UserData.nowEXP     = Player.NowEXP;
        UserData.kurikoshi  = Player.Kurikoshi;

        UserData.messageSpeed = SettingManager.instance.MessageSpeed;
        UserData.BGMvolume = SoundManager.instance.audioSourceBGM.volume;
        UserData.SEvolume = SoundManager.instance.audioSourceSE.volume;

        string jsonData = JsonUtility.ToJson(UserData);
        ES3.Save<string>("SAVE_KEY", jsonData);
    }

    /// <summary>
    /// ロード.
    /// </summary>
    public void Load()
    {
        if (!ES3.KeyExists("SAVE_KEY"))
        {
            Debug.Log("初回起動時");
            UserData = new UserData();

            // 状態異常の保持などに使っていたプレイヤーデータを削除する.
            PlayerPrefs.DeleteAll();    // 危険？.

            Player.Level = 1;
            // プレイヤーのレベルを初期化しておく.
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
            // -----------------

            Save();
            return;
        }
        else
        {
            Debug.Log("初回起動時ではない");
        }

        string jsonData = ES3.Load<string>("SAVE_KEY");

        UserData = JsonUtility.FromJson<UserData>(jsonData);
    }
}
