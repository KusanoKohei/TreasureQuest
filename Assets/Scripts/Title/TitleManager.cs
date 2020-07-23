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
    [SerializeField]
    private GameObject newGameButton;


    private GameObject dialogWindow;
    private CanvasGroup dialogCanvas;

    public GameObject noticeBoard;
    public Text noticeText;

    bool active = false;

    SceneTransitionManager sceneManager => SceneTransitionManager.instance;

    SaveSystem SaveSystem => SaveSystem.instance;
    UserData Userdata => SaveSystem.UserData;
    PlayerManager Player => PlayerManager.instance;

    SettingManager SettingManager => SettingManager.instance;


    // ------------------------------------ //
    private void Start()
    {
        // ロード.
        SaveSystem.Load();

        SettingManager.MessageSpeed        = Userdata.messageSpeed;
        SoundManager.instance.audioSourceBGM.volume = Userdata.BGMvolume;
        SoundManager.instance.audioSourceSE.volume  = Userdata.SEvolume;

        CheckCleared(); // ゲームクリア判定関数.

        StartCoroutine(TapToStartImageAnimating());

        CheckNotice();

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

            // これ以前にセーブするとプレイヤーステータス初期化の前にセーブすることになるので体力が0になる.
            SaveSystem.instance.Save();

            SettingManager.MessageSpeed = Userdata.messageSpeed;
            SettingManager.BgmVolume = Userdata.BGMvolume;
            SettingManager.SeVolume = Userdata.SEvolume;


            SoundManager.instance.PlayButtonSE(0);  // ボタンのクリック音.

            sceneManager.LoadTo("Town");
        }
    }


    /// <summary>
    /// ゲームクリアしていたなら、ニューゲームボタンを表示させる関数.
    /// </summary>
    private void CheckCleared()
    {
        if (Userdata.isCleared)
        {
            newGameButton.SetActive(true);
        }
        else
        {
            newGameButton.SetActive(false);
        }
    }
    
    private void CheckNotice()
    {
        if (SoundManager.instance.audioSourceBGM.volume == 0 || 
            SoundManager.instance.audioSourceSE.volume == 0)
        {
            noticeBoard.SetActive(true);
            noticeText.text = ("セーブデータの設定を反映し\nBGMや効果音を無音にしています");
        }
        else
        {
            noticeBoard.SetActive(false);
        }
    }
}
