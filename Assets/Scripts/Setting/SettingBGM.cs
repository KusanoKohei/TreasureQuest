using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingBGM : MonoBehaviour
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

    void CheckNum()
    {
        if(SoundManager.instance.audioSourceBGM.volume == 0)
        {
            num = 0;
            status = Status.OFF;
            buttonLabel.text = "OFF";
        }
        else if(SoundManager.instance.audioSourceBGM.volume == 0.5)
        {
            num = 1;
            status = Status.ON;
            buttonLabel.text = "O N";
        }
        else if(SoundManager.instance.audioSourceBGM.volume == 1)
        {
            num = 2;
            status = Status.MAX;
            buttonLabel.text = "MAX";
        }
    }

    private void switchLabel()
    {
        switch (num)
        {
            case 0:
                status = Status.OFF;
                buttonLabel.text = "OFF";
                SoundManager.instance.audioSourceBGM.volume = 0;    // BGM用のスピーカーを最小にする.
                // audioSource.Stop();
                break;

            case 1:
                status = Status.ON;
                buttonLabel.text = "O N";
                SoundManager.instance.audioSourceBGM.volume = 0.5f;    // BGM用のスピーカーを最大にする.
                // audioSource.Play();
                break;

            case 2:
                status = Status.MAX;
                buttonLabel.text = "MAX";
                SoundManager.instance.audioSourceBGM.volume = 1f;    // BGM用のスピーカーを最大にする.
                // audioSource.Play();
                break;
        }
    }

    public void OnClick()
    {
        num++;

        if (num >= 3)
        {
            num = 0;
        }

        switchLabel();

        // ボタンクリック音を鳴らす.
        SoundManager.instance.PlayButtonSE(0);
    }
}
