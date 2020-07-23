using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMessageSpeed : MonoBehaviour
{
    public Text buttonLabel;

    private static int num;
    public int Num { get => num; set => num = value; }


    private float slow = 1.5f;
    private float normal = 1.0f;
    private float fast = 0.5f;

    [SerializeField]
    private float p_slow;
    [SerializeField]
    private float p_normal;
    [SerializeField]
    private float p_fast;


    public enum Status
    {
        Slow,
        Normal,
        Fast
    }

     public Status status;

    SettingManager SettingManager => SettingManager.instance;
    PlayerManager PlayerManager => PlayerManager.instance;


    public float Slow { get => slow; set => slow = value; }
    public float Normal { get => normal; set => normal = value; }
    public float Fast { get => fast; set => fast = value; }

    #region Singleton
    public static SettingMessageSpeed instance;

    private void Awake()
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
        CheckNum();
    }

    void CheckNum()
    {
        Num = PlayerPrefs.GetInt("MessageSpeedNum", Num);

        if(Num == 0)
        {
            SettingManager.MessageSpeed = Slow;
            status = Status.Slow;
            buttonLabel.text = "遅い";
        }
        else if(Num == 1)
        {
            SettingManager.MessageSpeed = Normal;
            status = Status.Normal;
            buttonLabel.text = "ふつう";
        }
        else if(Num == 2)
        {
            SettingManager.MessageSpeed = Fast;
            status = Status.Fast;
            buttonLabel.text = "早い";
        }
        else
        {
            SettingManager.MessageSpeed = Slow;
            status = Status.Slow;
            buttonLabel.text = "遅い";
        }

        SaveSystem.instance.UserData.messageSpeed = SettingManager.MessageSpeed;
        PlayerPrefs.SetInt("MessageSpeedNum", Num);
        PlayerPrefs.Save();
    }

    public void OnClick()
    {
        Num++;

        Debug.Log(Num);

        if (Num >= 3)
        {
            Num = 0;
        }

        PlayerPrefs.SetInt("MessageSpeedNum", Num);
        PlayerPrefs.Save();

        CheckNum();
        // SwitchMessagaSpeed();

        // ボタンクリック音を鳴らす.
        SoundManager.instance.PlayButtonSE(0);
    }
}
