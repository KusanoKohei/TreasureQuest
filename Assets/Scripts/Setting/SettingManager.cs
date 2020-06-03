using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [SerializeField]
    private float messageSpeed;
    public float MessageSpeed { get => messageSpeed; set => messageSpeed = value; }

    private int messageSpeedNum;
    public int MessageSpeedNum { get => messageSpeedNum; set => messageSpeedNum = value; }


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
