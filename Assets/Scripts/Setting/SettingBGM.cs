using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingBGM : MonoBehaviour
{
    public Text buttonLabel;

    private static int num;
    public int Num { get => num; set => num = value; }

    public AudioSource audioSource;

    [SerializeField]
    private float on = 0.5f;

    [SerializeField]
    private float off = 0.0f;

    [SerializeField]
    private float max = 1.0f;


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
        SettingManager.instance.BgmVolume = SaveSystem.instance.UserData.BGMvolume;

        CheckStatus();
    }

    private void CheckStatus()
    {
        Debug.Log(SettingManager.instance.BgmVolume);


        if(SettingManager.instance.BgmVolume == off)
        {
            status = Status.OFF;
        }
        else if(SettingManager.instance.BgmVolume == on)
        {
            status = Status.ON;
        }
        else if(SettingManager.instance.BgmVolume == max)
        {
            status = Status.MAX;
        }
        else
        {
            status = Status.ON;
        }

        ChangeVolume();
    }

    private void ChangeVolume()
    {
        Debug.Log(status);

        switch (status)
        {
            case Status.ON:
                SoundManager.instance.audioSourceBGM.volume = on;
                buttonLabel.text = "O N";
                break;

            case Status.OFF:
                SoundManager.instance.audioSourceBGM.volume = off;
                buttonLabel.text = "OFF"; 
                break;

            case Status.MAX:
                SoundManager.instance.audioSourceBGM.volume = max;
                buttonLabel.text = "MAX"; 
                break;

            default:
                SoundManager.instance.audioSourceBGM.volume = on;
                buttonLabel.text = "O N"; 
                break;
        }

        SettingManager.instance.BgmVolume = SoundManager.instance.audioSourceBGM.volume;
        SaveSystem.instance.UserData.BGMvolume = SettingManager.instance.BgmVolume;
    }

    private void CheckNum()
    {
        Debug.Log("CheckNum > " + Num);

        switch (Num)
        {
            case 0:
                status = Status.ON;
                break;

            case 1:
                status = Status.OFF;
                break;

            case 2:
                status = Status.MAX;
                break;

            default:
                status = Status.ON;
                break;
        }

        ChangeVolume();
    }

    public void OnClick()
    {
        Debug.Log("bgmButton > OnClick()");
        Debug.Log("OnClick >" + Num);

        // Num = PlayerPrefs.GetInt("BGMNum", Num);
        // Debug.Log(Num);

        Num++;

            if (Num >= 3)
            {
                Num = 0;
            }

            // PlayerPrefs.SetInt("BGMvolumeNum", Num);
            // PlayerPrefs.Save();

        CheckNum();

        // ボタンクリック音を鳴らす.
        SoundManager.instance.PlayButtonSE(0);
    }
}
