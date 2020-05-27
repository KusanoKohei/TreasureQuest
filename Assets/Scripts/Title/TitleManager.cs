using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public PlayerManager player;

    private GameObject dialogWindow;
    private CanvasGroup dialogCanvas;
    public GameObject noticeBoard;

    public Text noticeText;
    

    UserData Userdata => SaveSystem.Instance.UserData;


    // ------------------------------------ //
    private void Start()
    {
        SettingManager.instance.MessageSpeed        = Userdata.messageSpeed;
        SoundManager.instance.audioSourceBGM.volume = Userdata.BGMvolume;
        SoundManager.instance.audioSourceSE.volume  = Userdata.SEvolume;

        CheckNotice();

        // ダイアログウィンドウを非表示にしておく.
        dialogWindow = GameObject.Find("FadeCanvas/DialogUI");
        dialogCanvas = dialogWindow.GetComponent<CanvasGroup>();
        dialogCanvas.ignoreParentGroups = false;
        dialogWindow.SetActive(false);
    }

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
}
