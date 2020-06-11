using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingBGM : MonoBehaviour
{
    public Text buttonLabel;

    private int num;
    public int Num { get => num; set => num = value; }

    public AudioSource audioSource;


    private float on = 0.5f;
    public float On { get => on; set => on = value; }

    private float off = 0.0f;
    public float Off { get => off; set => off = value; }

    private float max = 1.0f;
    public float Max { get => max; set => max = value; }


    public enum Status
    {
        OFF,
        ON,
        MAX
    }

    Status status;

    SettingManager settingManager => SettingManager.instance;




    #region Singleton
    public static SettingBGM instance;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        // Num = PlayerPrefs.GetInt("BGMvolumeNum", Num);
        CheckNum();
        SwitchBGMvolume();
    }

    private void CheckNum()
    {
        if(SaveSystem.instance.UserData.BGMvolume == On)
        {
            Num = 0;
        }
        else if(SaveSystem.instance.UserData.BGMvolume == Off)
        {
            Num = 1;
        }
        else if(SaveSystem.instance.UserData.BGMvolume == Max)
        {
            Num = 2;
        }
        else
        {
            Num = 0;
        }
    }

    public void SwitchBGMvolume()
    {
        // Num = PlayerPrefs.GetInt("BGMvolumeNum", Num);

        switch (Num)
        {
            case 0:
                status = Status.ON;
                buttonLabel.text = "O N";
                SoundManager.instance.audioSourceBGM.volume = 0.5f;    // BGM用のスピーカーを最大にする.
                break;

            case 1:
                status = Status.OFF;
                buttonLabel.text = "OFF";
                SoundManager.instance.audioSourceBGM.volume = 0;    // BGM用のスピーカーを最小にする.
                break;

            case 2:
                status = Status.MAX;
                buttonLabel.text = "MAX";
                SoundManager.instance.audioSourceBGM.volume = 1f;    // BGM用のスピーカーを最大にする.
                break;

            default:
                SoundManager.instance.audioSourceBGM.volume = 1.0f;
                break;
        }

        SaveSystem.instance.UserData.BGMvolume = SoundManager.instance.audioSourceBGM.volume;
    }

    public void OnClick()
    {
        Num++;

        if (Num >= 3)
        {
            Num = 0;
        }

        PlayerPrefs.SetInt("BGMvolumeNum", Num);
        PlayerPrefs.Save();

        SwitchBGMvolume();

        // ボタンクリック音を鳴らす.
        SoundManager.instance.PlayButtonSE(0);
    }
}
