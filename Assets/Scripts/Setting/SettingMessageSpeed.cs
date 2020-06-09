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

    Status status;

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
        SwitchMessegaSpeed();
    }

    void SwitchMessegaSpeed()
    {
        if (num == 0)
        {
            SettingManager.MessageSpeed = Slow;
            status = Status.Slow;
            SettingManager.ParticlePlaybackSpeed = p_slow;
            buttonLabel.text = "遅い";
        }
        else if (num == 1) 
        {
            SettingManager.MessageSpeed = Normal;
            status = Status.Normal;
            SettingManager.ParticlePlaybackSpeed = p_normal;
            buttonLabel.text = "ふつう";
        }
        else if (num == 2)
        {
            SettingManager.MessageSpeed = Fast;
            status = Status.Fast;
            SettingManager.ParticlePlaybackSpeed = p_fast;
            buttonLabel.text = "早い";
        }

        SwitchParticleSpeed();
    }

   
    private void SwitchParticleSpeed()
    {
        if(status == Status.Slow)
        {
            for(int i=0; i<=PlayerManager.ParticleName.Length-1; i++)
            {
                Debug.Log(PlayerManager.ParticleName[i]);
                // PlayerManager.ParticleName[i].playbackSpeed = p_slow;
            }
        }
        else if(status == Status.Normal)
        {
            for (int i = 0; i <= PlayerManager.ParticleName.Length-1; i++)
            {
                Debug.Log(PlayerManager.ParticleName[i]);
                // PlayerManager.ParticleName[i].playbackSpeed = p_normal;
            }
        }
        else if(status == Status.Fast)
        {
            for (int i = 0; i <= PlayerManager.ParticleName.Length-1; i++)
            {
                Debug.Log(PlayerManager.ParticleName[i]);
                // PlayerManager.ParticleName[i].playbackSpeed = p_fast;
            }
        }
    }
    

    public void OnClick()
    {
        num++;

        if (num >= 3)
        {
            num = 0;
        }

        SwitchMessegaSpeed();

        // ボタンクリック音を鳴らす.
        SoundManager.instance.PlayButtonSE(0);
    }
}
