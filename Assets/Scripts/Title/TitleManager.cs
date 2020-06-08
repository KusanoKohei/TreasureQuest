using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    public GameObject tapToStartPanel;
    [SerializeField]
    private GameObject tapToStartImage;
    [SerializeField]
    private CanvasGroup tapToStartCanvasGroup;

    private GameObject dialogWindow;
    private CanvasGroup dialogCanvas;

    // public GameObject noticeBoard;
    // public Text noticeText;

    bool active = false;

    SceneTransitionManager sceneManager => SceneTransitionManager.instance;

    UserData Userdata => SaveSystem.instance.UserData;
    PlayerManager Player => PlayerManager.instance;


    // ------------------------------------ //
    private void Start()
    {
        // SettingManager.instance.MessageSpeed        = Userdata.messageSpeed;
        // SoundManager.instance.audioSourceBGM.volume = Userdata.BGMvolume;
        // SoundManager.instance.audioSourceSE.volume  = Userdata.SEvolume;

        StartCoroutine(TapToStartImageAnimating());

        // CheckNotice();

        // ダイアログウィンドウを非表示にしておく.
        dialogWindow = GameObject.Find("FadeCanvas/DialogUI");
        dialogCanvas = dialogWindow.GetComponent<CanvasGroup>();
        dialogCanvas.ignoreParentGroups = false;
        dialogWindow.SetActive(false);
    }

    private IEnumerator TapToStartImageAnimating()
    {
        tapToStartCanvasGroup = tapToStartImage.GetComponent<CanvasGroup>();
        tapToStartCanvasGroup.DOFade(0.0f, 1.0f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

        // LoadTo関数を起動する時間を遅延させて、ゲーム立ち上げ時にいきなりシーン遷移しないように.
        yield return new WaitForSeconds(1.0f);  
        active = true;
    }

    public void TapToStartPanel()
    {
        if (active)
        {
            /*
            // 状態異常の保持などに使っていたプレイヤーデータを削除する.
            PlayerPrefs.DeleteAll();    // 危険？.

            Player.Level = 1;
            Player.Init_playerParameter();
            SoundManager.instance.PlayButtonSE(0);

            sceneManager.LoadTo("Town");
            */

            /*
            Userdata.messageSpeed = SettingManager.instance.MessageSpeed;
            Userdata.BGMvolume = SoundManager.instance.audioSourceBGM.volume;
            Userdata.SEvolume = SoundManager.instance.audioSourceSE.volume;

            SaveSystem.instance.Save();
            */

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

            SettingManager.instance.MessageSpeed = Userdata.messageSpeed;
            SoundManager.instance.audioSourceBGM.volume = Userdata.BGMvolume;
            SoundManager.instance.audioSourceSE.volume = Userdata.SEvolume;
            

            SoundManager.instance.PlayButtonSE(0);  // ボタンのクリック音.

            sceneManager.LoadTo("Town");
        }
    }

    /*
    public void OnTapNewGameButton()
    {
        Userdata.messageSpeed   = SettingManager.instance.MessageSpeed;
        Userdata.BGMvolume      = SoundManager.instance.audioSourceBGM.volume;
        Userdata.SEvolume       = SoundManager.instance.audioSourceSE.volume;

        // 状態異常の保持などに使っていたプレイヤーデータを削除する.
        PlayerPrefs.DeleteAll();    // 危険？.

        player.Level = 1;
        player.Init_playerParameter();
        SoundManager.instance.PlayButtonSE(0);
    }

    private void CheckNotice()
    {
        if (SettingManager.instance.MessageSpeed == 2.0f || 
            SoundManager.instance.audioSourceBGM.volume == 0 || 
            SoundManager.instance.audioSourceSE.volume == 0)
        {
            noticeBoard.SetActive(true);
            noticeText.text = ("セーブデータの設定を反映し\nメッセージ速度や音量を調整しています");
        }
        else
        {
            noticeBoard.SetActive(false);
        }
    }
    */
}
