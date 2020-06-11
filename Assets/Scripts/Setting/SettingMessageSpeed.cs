using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMessageSpeed : MonoBehaviour
{
    public Text buttonLabel;

    private int num;
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
        SwitchMessagaSpeed();
    }

    void CheckNum()
    {
        if(SaveSystem.instance.UserData.messageSpeed == Slow)
        {
            Num = 0;
        }
        else if(SaveSystem.instance.UserData.messageSpeed == Normal)
        {
            Num = 1;
        }
        else if(SaveSystem.instance.UserData.messageSpeed == Fast)
        {
            Num = 2;
        }
        else
        {
            Num = 0;
        }

    }


    public void SwitchMessagaSpeed()
    {
        // Num = PlayerPrefs.GetInt("MessageSpeedNum", Num);

        if (Num == 0)
        {
            SettingManager.MessageSpeed = Slow;
            status = Status.Slow;
            // SettingManager.ParticlePlaybackSpeed = p_slow;
            buttonLabel.text = "遅い";
        }
        else if (Num == 1) 
        {
            SettingManager.MessageSpeed = Normal;
            status = Status.Normal;
            // SettingManager.ParticlePlaybackSpeed = p_normal;
            buttonLabel.text = "ふつう";
        }
        else if (Num == 2)
        {
            SettingManager.MessageSpeed = Fast;
            status = Status.Fast;
            // SettingManager.ParticlePlaybackSpeed = p_fast;
            buttonLabel.text = "早い";
        }

        SaveSystem.instance.UserData.messageSpeed = SettingManager.MessageSpeed;
    }

   
    private void SwitchParticleSpeed()
    {
        if(status == Status.Slow)
        {
            for(int i=0; i<=PlayerManager.ParticleName.Length-1; i++)
            {
                // PlayerManager.ParticleName[i].playbackSpeed = p_slow;
            }
        }
        else if(status == Status.Normal)
        {
            for (int i = 0; i <= PlayerManager.ParticleName.Length-1; i++)
            {
                // PlayerManager.ParticleName[i].playbackSpeed = p_normal;
            }
        }
        else if(status == Status.Fast)
        {
            for (int i = 0; i <= PlayerManager.ParticleName.Length-1; i++)
            {
                // PlayerManager.ParticleName[i].playbackSpeed = p_fast;
            }
        }
    }
    

    public void OnClick()
    {
        Num++;

        if (Num >= 3)
        {
            Num = 0;
        }

        PlayerPrefs.SetInt("MessageSpeedNum", Num);
        PlayerPrefs.Save();

        SwitchMessagaSpeed();

        // ボタンクリック音を鳴らす.
        SoundManager.instance.PlayButtonSE(0);
    }
}
