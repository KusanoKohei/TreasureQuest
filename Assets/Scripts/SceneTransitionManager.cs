using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitionManager : MonoBehaviour
{
    #region Singleton

    public static SceneTransitionManager instance;

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
    public void LoadTo(string sceneName)
    {
        // フェードアウトする.
        // ダイアログウィンドウを消す.
        // シーン遷移する.
        // フェードインする.
        // ダイアログウィンドウを表示する.
        FadeIOManager.instance.FadeOutToIn(() => Load(sceneName));

    }
    void Load(string sceneName)
    {
        SoundManager.instance.PlayBGM(sceneName);
        SceneManager.LoadScene(sceneName);
    }
}