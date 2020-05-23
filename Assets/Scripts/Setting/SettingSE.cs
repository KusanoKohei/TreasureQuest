using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSE : MonoBehaviour
{
    public Text buttonLabel;
    private int num;
    public AudioSource audioSource;


    public enum Status
    {
        OFF,
        ON,
        MAX
    }

    Status status;

    SettingManager settingManager => SettingManager.instance;


    // Start is called before the first frame update
    void Start()
    {
        CheckNum();
    }

    private void CheckNum()
    {
        if(SoundManager.instance.audioSourceSE.volume == 0)
        {
            num = 0;
            status = Status.OFF;
            buttonLabel.text = "OFF";
        }
        else if(SoundManager.instance.audioSourceSE.volume == 0.75)
        {
            num = 1;
            status = Status.ON;
            buttonLabel.text = "ON";
        }
    }

    private void switchLabel()
    {
        switch (num)
        {
            case 0:
                status = Status.OFF;
                buttonLabel.text = "OFF";
                SoundManager.instance.audioSourceSE.volume = 0;    // BGM用のスピーカーを最小にする.
                break;

            case 1:
                status = Status.MAX;
                buttonLabel.text = "O N";
                SoundManager.instance.audioSourceSE.volume = 0.75f;    // BGM用のスピーカーを最大にする.
                break;
        }
    }

    public void OnClick()
    {
        num++;

        if (num >= 2)
        {
            num = 0;
        }

        switchLabel();

        // ボタンクリック音を鳴らす.
        SoundManager.instance.PlayButtonSE(0);
    }
}
