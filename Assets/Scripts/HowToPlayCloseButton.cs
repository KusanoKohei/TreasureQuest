using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayCloseButton : MonoBehaviour
{
    public GameObject[] howToPlayBoard;
    public GameObject settingIcon;
    public GameObject howToPlayButton;

    [SerializeField]
    private GameObject shadePanel;

    public static int nowBoardNum =0;


    private void Start()
    {

    }

    public void OnClickNextButton()
    {
        nowBoardNum++;

        if (nowBoardNum < howToPlayBoard.Length)
        {
            howToPlayBoard[nowBoardNum].SetActive(true);

            if (nowBoardNum > 0)
            {
                howToPlayBoard[nowBoardNum - 1].SetActive(false);
            }
        }
        else
        {
            shadePanel.SetActive(false);
            settingIcon.SetActive(true);
            howToPlayButton.SetActive(true);

            Debug.Log(howToPlayBoard.Length);

            for (int i = 0; i < howToPlayBoard.Length; i++)
            {
                howToPlayBoard[i].SetActive(false);
            }
        }
        // ページめくりSE.
        SoundManager.instance.PlayButtonSE(15);
    }
}
