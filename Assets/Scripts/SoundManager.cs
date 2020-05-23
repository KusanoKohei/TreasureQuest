using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSourceBGM;  // BGMのためのスピーカー.
    public AudioClip[] audioClipsBGM;      // BGMのための配列(0:title, 1:Town, 2:Quest, 3:Battle).

    public AudioSource audioSourceSE;   // SEのためのスピーカー.
    public AudioClip[] audioClipsSE;

    #region Singleton
    public static SoundManager instance;

    private void Awake()
    {
        if(instance == null)
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

    // Start is called before the first frame update

    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }

    public void PlayBGM(string caseName)
    {
        audioSourceBGM.Stop();  // 一旦BGMを止める.

        switch (caseName)
        {
            default:
            case "Title":
                audioSourceBGM.clip = audioClipsBGM[0];
                break;

            case "Town":
                audioSourceBGM.clip = audioClipsBGM[1];
                break;

            case "Quest":
                audioSourceBGM.clip = audioClipsBGM[2];
                break;

            case "Battle":
                audioSourceBGM.clip = audioClipsBGM[3];
                break;

            case "BossBattle":
                audioSourceBGM.clip = audioClipsBGM[4];
                break;
        }

        audioSourceBGM.Play();

    }

    public void PlayButtonSE(int index)
    {
        audioSourceSE.PlayOneShot(audioClipsSE[index]);
    }
}
