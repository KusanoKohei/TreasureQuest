using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OpenCloseBoard : MonoBehaviour
{
    public GameObject newGameBoard;
    public GameObject creditBoard;
    public GameObject settingBoard;
    public GameObject settingIcon;
    public GameObject quitGameBoard;
    public GameObject howToPlayBoard;
    public GameObject howToPlayButton;

    private GameObject board;

    [SerializeField]
    private GameObject shadePanel;

    private Image shadeColor;

    private RectTransform boardPos;


    private void Start()
    {
        shadeColor = shadePanel.GetComponent<Image>();
    }


    public void OnClick()
    {
        switch (this.gameObject.name)
        {
            case "NewGameButton":
            case "NewGameCancelButton":
                Debug.Log("ニューゲームボタン");
                board = newGameBoard;
                BoardAction();
                break;

            case "ToCreditButton":
            case "CreditBoardCloseButton":
                board = creditBoard;
                BoardAction();
                break;

            case "QuitGameButton":
            case "QuitGameCancelButton":
                board = quitGameBoard;
                BoardAction();
                break;

            case "HowToPlayButton":
                board = howToPlayBoard;
                BoardAction();
                break;
        }

        // ボタンクリック音を鳴らす.
        SoundManager.instance.PlayButtonSE(0);
    }

    private void BoardAction()
    {
        // activeSelfでは誤作動を起こす.
        Debug.Log(board.activeInHierarchy);

        if (board.activeInHierarchy)
        {
            StartCoroutine(CloseAction());
        }
        else if (!board.activeInHierarchy)
        {
            StartCoroutine(OpenAction());
        }
    }

    private IEnumerator CloseAction()
    {
        Debug.Log("閉じた");

        board.SetActive(false);

        if (board == creditBoard)
        {
            shadeColor.color = new Color(0, 0, 0, 0.5f);
        }

        shadePanel.SetActive(false);
        settingIcon.SetActive(true);
        howToPlayButton.SetActive(true);

        yield return null;
    }

    private IEnumerator OpenAction()
    {
        shadePanel.SetActive(true);

        if (board == newGameBoard || board == quitGameBoard)
        {
            shadeColor.color = new Color(0, 0, 0, 0.5f);
            yield return new WaitForSeconds(1.0f);
        }
        else if(board == creditBoard)
        {
            shadeColor.color = new Color(1,1,1,0);
        }
        else if (howToPlayButton)
        {
            // 表示するボードのナンバーを最初からにする.
            HowToPlayCloseButton.nowBoardNum = 0;
            howToPlayBoard.SetActive(true);
            howToPlayBoard.transform.Find("HowToPlayBoard_01").gameObject.SetActive(true);
        }

        Debug.Log("開いた");
        board.SetActive(true);

        // 色々閉じておく.
        settingIcon.SetActive(false);
        howToPlayButton.SetActive(false);
        // 設定ボードが開かれていたなら閉じておく.
        settingBoard.SetActive(false);

        yield return null;
    }
}
