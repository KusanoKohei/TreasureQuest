using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMessageSpeed : MonoBehaviour
{
    public Text buttonLabel;

    private int num;

    [SerializeField]
    private float slow;
    [SerializeField]
    private float normal;
    [SerializeField]
    private float fast;
    public enum Status
    {
        Slow,
        Normal,
        Fast
    }

    Status status;

    SettingManager SettingManager => SettingManager.instance;

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
        if (SettingManager.instance.MessageSpeed == 1.5f)
        {
            num = 0;
            SettingManager.MessageSpeed = Slow;
            buttonLabel.text = "遅い";
        }
        else if (SettingManager.instance.MessageSpeed == 1.25f) 
        {
            num = 1;
            SettingManager.MessageSpeed = Normal;
            buttonLabel.text = "ふつう";
        }
        else if (SettingManager.instance.MessageSpeed == 1.0f)
        {
            num = 2;
            SettingManager.MessageSpeed = Fast;
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
                SettingManager.MessageSpeed = Slow;
                break;

            case 1:
                status = Status.Normal;
                buttonLabel.text = "ふつう";
                SettingManager.MessageSpeed = Normal;
                break;

            case 2:
                status = Status.Fast;
                buttonLabel.text = "早い";
                SettingManager.MessageSpeed = Fast;
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
