using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [SerializeField]
    private float messageSpeed;
    public static float MessageSpeed { get => SettingManager.instance.messageSpeed; set => SettingManager.instance.messageSpeed = value; }

    private int messageSpeedNum;
    public int MessageSpeedNum { get => messageSpeedNum; set => messageSpeedNum = value; }

    [SerializeField]
    private float bgmVolume;
    public float BgmVolume { get => bgmVolume; set => bgmVolume = value; }

    [SerializeField]
    private float seVolume; 
    public float SeVolume { get => seVolume; set => seVolume = value; }


    private SettingMessageSpeed SettingMessage => SettingMessageSpeed.instance;
    private SettingBGM SettingBGM => SettingBGM.instance;
    
    private float particlePlaybackSpeed;
    public float ParticlePlaybackSpeed { get => particlePlaybackSpeed; set => particlePlaybackSpeed = value; }


    #region Singleton

    public static SettingManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion
}
