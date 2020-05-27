using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    #region Singleton
    private static SaveSystem instance = new SaveSystem();

    public static SaveSystem Instance => instance;
    #endregion

    private SaveSystem() { Load(); }

    // public string Path => Application.dataPath + "/data.json";  // セーブデータのファイルをAssetフォルダに置く（製作のしやすさから）.
    public string Path => Application.persistentDataPath+ "/data.json";  // セーブデータのファイルをAssetフォルダに置く（製作のしやすさから）.

    public string path => Application.streamingAssetsPath + "/data.json";


    // private UserData userData = new UserData();
    public UserData UserData{get; private set;}

    private PlayerManager Player => PlayerManager.instance;

    public void Save()
    {
        string jsonData = JsonUtility.ToJson(UserData);
        StreamWriter writer = new StreamWriter(Path, false);
        writer.WriteLine(jsonData);
        writer.Flush(); // 書き残し予防.
        writer.Close();
    }

    public void Load()
    {
        if(!File.Exists(Path))
        {
            Debug.Log("初回起動時");
            UserData = new UserData();

            // プレイヤーのレベルを初期化しておく.
            PlayerManager.instance.Init_playerParameter();

            UserData.level          = Player.Level;
            UserData.maxHP          = Player.MaxHP;
            UserData.hp             = Player.Hp;
            UserData.atk            = Player.Atk;
            UserData.spd            = Player.Spd;
            UserData.dodge          = Player.Dodge;
            UserData.critical       = Player.Critical;
            UserData.skill          = Player.Skill;
            UserData.nextEXP        = Player.NextEXP;
            UserData.nowEXP         = Player.NowEXP;
            UserData.kurikoshi      = Player.Kurikoshi;
            UserData.messageSpeed   = SettingManager.instance.MessageSpeed;
            UserData.BGMvolume      = SoundManager.instance.audioSourceBGM.volume;
            UserData.SEvolume       = SoundManager.instance.audioSourceSE.volume;

            // -----------------

            Save();
            return;
        }

        // StreamReader reader = new StreamReader(Path);
        StreamReader reader = new StreamReader(path);
        string jsonData = reader.ReadToEnd();
        UserData = JsonUtility.FromJson<UserData>(jsonData);
        reader.Close();
    }
}
