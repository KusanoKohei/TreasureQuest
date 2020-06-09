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

    // private UserData userData = new UserData();

    private UserData userData;
    public UserData UserData { get => userData; set => userData = value; }

    private PlayerManager Player => PlayerManager.instance;

    //public string Path => Application.dataPath + "/data.json";  // セーブデータのファイルをAssetフォルダに置く（製作のしやすさから）.
    public string Path => Application.persistentDataPath + "/"+".savedata.json";  // セーブデータのファイルをAssetフォルダに置く（製作のしやすさから）.

    void Awake()
    {
        userData = new UserData();
    }
    
    public void Save()
    {
#if UNITY_EDITOR

        string jsonData = JsonUtility.ToJson(UserData);
        StreamWriter writer = new StreamWriter(Path, false);
        writer.WriteLine(jsonData);
        writer.Flush(); // 書き残し予防.
        writer.Close();

#elif UNITY_ANDROID
        // ファイル書き出し.
        StreamWriter sw;

        if (System.IO.File.Exists(UnityEngine.Application.persistentDataPath + "/saveData.txt"))
        {
            // 第二引数をfalseにしているので、ファイルは上書きされる.
            sw = new StreamWriter(UnityEngine.Application.persistentDataPath + "/saveData.txt", false, Encoding.GetEncoding("Shift_JIS"));
            // System.Text.Encoding.UTF8.
        }
        else
        {
            sw = new StreamWriter(UnityEngine.Application.persistentDataPath + "/saveData.txt", true, Encoding.GetEncoding("Shift_JIS"));
        }

        string jsonData = JsonUtility.ToJson(UserData);

        // 書き込み.
        sw.WriteLine(jsonData);
        // 閉じる.
        sw.Flush();
        sw.Close();
#endif
    }

    public void Load()
    {
    #if UNITY_EDITOR
        if (!File.Exists(Path))
        {
            Debug.Log("初回起動時");
            UserData = new UserData();

            // 状態異常の保持などに使っていたプレイヤーデータを削除する.
            PlayerPrefs.DeleteAll();    // 危険？.

            Player.Level = 1;
            // プレイヤーのレベルを初期化しておく.
            PlayerManager.instance.Init_playerParameter();

            UserData.level  = Player.Level;
            UserData.maxHP  = Player.MaxHP;
            UserData.hp     = Player.Hp;
            UserData.atk    = Player.Atk;
            UserData.spd    = Player.Spd;
            UserData.dodge  = Player.Dodge;
            UserData.critical   = Player.Critical;
            UserData.skill      = Player.Skill;
            UserData.nextEXP    = Player.NextEXP;
            UserData.nowEXP     = Player.NowEXP;
            UserData.kurikoshi  = Player.Kurikoshi;
            UserData.messageSpeed   = SettingManager.instance.MessageSpeed;
            UserData.BGMvolume      = SoundManager.instance.audioSourceBGM.volume;
            UserData.SEvolume       = SoundManager.instance.audioSourceSE.volume;

            // -----------------

            Save();
            return;
        }
        else
        {
            Debug.Log("初回起動時ではない");
        }

        StreamReader reader = new StreamReader(Path);
        string jsonData = reader.ReadToEnd();
        reader.Close();

        UserData = JsonUtility.FromJson<UserData>(jsonData);


#elif UNITY_ANDROID
        // Debug.Log("Load関数");
        if (!System.IO.File.Exists(UnityEngine.Application.persistentDataPath + "/saveData.txt"))
        {
            // 初回起動時.
            Debug.Log("初回起動時");
            UserData = new UserData();

            Player.Level = 1;
            // プレイヤーのレベルを初期化しておく.
            PlayerManager.instance.Init_playerParameter();

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
            UserData.messageSpeed   = SettingManager.instance.MessageSpeed;
            UserData.BGMvolume      = SoundManager.instance.audioSourceBGM.volume;
            UserData.SEvolume       = SoundManager.instance.audioSourceSE.volume;

            // -----------------

            Save();
            return;

            // この初回起動時までの処理は上手くいっている。
            // \C:user/Kusano Kohei / AppData / LocalLow / Kabakamon(CompanyNama)/ TreasureQuese(ProductName)
            // に実際、saveData.txtが作成される現象も確認した.
            // しかしこれ以降の読み込みが上手くいかないのか？
            // 初回起動ではない場合は上手くいかない.
        }

        // ファイル読み込み.
        StreamReader sr = new StreamReader(UnityEngine.Application.persistentDataPath, Encoding.GetEncoding("Shift_JIS"));
        string jsonData = sr.ReadToEnd();
        Debug.Log(jsonData);
        UserData = JsonUtility.FromJson<UserData>(jsonData);
        sr.Close();
#endif
    }

    /*
    private SaveSystem() { Load(); }

    public string Path => Application.dataPath + "/data.json";  // セーブデータのファイルをAssetフォルダに置く（製作のしやすさから）.
    // public string Path => Application.persistentDataPath+ "/data.json";  // WebGL版のセーブデータの保管先.

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

        StreamReader reader = new StreamReader(Path);
        // StreamReader reader = new StreamReader(path);
        string jsonData = reader.ReadToEnd();
        UserData = JsonUtility.FromJson<UserData>(jsonData);
        reader.Close();
    }
    */
}
