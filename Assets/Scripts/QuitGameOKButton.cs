using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameOKButton : MonoBehaviour
{
    public void OnClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
		Application.OpenURL("http://www.yahoo.co.jp/");
        #else
        UnityEngine.Application.Quit();
        #endif
    }
}
