using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingButton : MonoBehaviour
{
    [SerializeField]
    private GameObject settingIcon;
    [SerializeField]
    private GameObject settingBoard;

    private RectTransform settingBoardPos;
    private Vector3 Pos = new Vector3(0,0,0);

    private void Start()
    {
        settingBoardPos = settingBoard.GetComponent<RectTransform>();
    }


    public void OnClick()
    {
        // 設定ボードのアクティブがfalseならtrue、trueならfalseにする.
        if (settingBoard.activeSelf)
        {
            StartCoroutine(CloseAction());
        }
        else if(!settingBoard.activeSelf)
        {
            settingBoard.SetActive(true);
            settingBoardPos.DOScale(new Vector3(1, 1, 1), 0.5f);
            settingBoardPos.DOMove(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0), 0.5f);
        }

        // ボタンクリック音を鳴らす.
        SoundManager.instance.PlayButtonSE(0);
    }

    private IEnumerator CloseAction()
    {
        // 移動.
        settingBoardPos.DOMove(new Vector3(settingIcon.transform.position.x, settingIcon.transform.position.y, 0), 0.5f);

        // 縮小.
        settingBoardPos.DOScale(new Vector3(0, 0, 0), 0.5f);

        // settingBoardの位置を、settingBoard本来の位置に.

        yield return new WaitForSeconds(0.5f);
        settingBoard.SetActive(false);
    }

    private IEnumerator OpenAction()
    {
        yield return null;
    }
}
