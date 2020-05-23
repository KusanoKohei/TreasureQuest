using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMessageSpeed : MonoBehaviour
{
    public Text buttonLabel;
    private int num;

    [SerializeField]
    private float slow   = 1.0f;
    [SerializeField]
    private float normal = 1.5f;
    [SerializeField]
    private float fast   = 1.0f;
    public enum Status
    {
        Slow,
        Normal,
        Fast
    }

    Status status;

    SettingManager settingManager => SettingManager.instance;

    public float Slow { get => slow; set => slow = value; }
    public float Normal { get => normal; set => normal = value; }
    public float Fast { get => fast; set => fast = value; }


    // Start is called before the first frame update
    void Start()
    {
        CheckNum();
    }

    void CheckNum()
    {
        if (SoundManager.instance.audioSourceBGM.volume == Slow)
        {
            num = 0;
            status = Status.Slow;
            buttonLabel.text = "遅い";
        }
        else if (settingManager.MessageSpeed == Normal) 
        {
            num = 1;
            status = Status.Normal;
            buttonLabel.text = "ふつう";
        }
        else if (settingManager.MessageSpeed == Fast)
        {
            num = 2;
            status = Status.Fast;
            buttonLabel.text = "早い";
        }
    }


    private void switchLabel()
    {
        switch (num)
        {
            case 0:
                status = Status.Slow;
                buttonLabel.text = "遅い";
                settingManager.MessageSpeed = 2.0f;
                break;

            case 1:
                status = Status.Normal;
                buttonLabel.text = "ふつう";
                settingManager.MessageSpeed = 1.5f;
                break;

            case 2:
                status = Status.Fast;
                buttonLabel.text = "早い";
                settingManager.MessageSpeed = 1.0f;
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
