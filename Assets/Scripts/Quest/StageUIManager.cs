using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ゲーム上のUI（クエスト進行度/ 進むボタン / 戻るボタン）を管理する.
public class StageUIManager : MonoBehaviour
{
    public Text stageText;
    public GameObject nextButton;
    public GameObject toTownButton;
    public GameObject stageClearImage;
    public GameObject mushroomImage;
    public GameObject teateButton;
    public GameObject yesButton;
    public GameObject noButton;


    #region Singleton
    public static StageUIManager instance;

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


    private void Start()
    {
        stageClearImage.SetActive(false);
    }
    public void UpdateUI(int currentStage)
    {
        stageText.text = string.Format("ステージ : {0} / 10", currentStage+1);
    }

    public void ButtonUIAppearance(bool isTrue)
    {
        stageClearImage.SetActive(false);
        nextButton.SetActive(isTrue);
        toTownButton.SetActive(isTrue);

        // 『手当て』ボタンは一度実行するとそのクエストでは表示しない.
        if (QuestManager.instance.Teated)
        {
            teateButton.SetActive(false);
        }
        else
        {
            teateButton.SetActive(isTrue);
        }

        yesButton.SetActive(false);
        noButton.SetActive(false);
    }

    public void YesNoButtonAppearance(bool isTrue)
    {
        yesButton.SetActive(isTrue);
        noButton.SetActive(isTrue);

        nextButton.SetActive(false);
        toTownButton.SetActive(false);
    }

    public void ClearUIAppearance()
    {
        stageClearImage.SetActive(true);
        nextButton.SetActive(false);
        toTownButton.SetActive(false);
    }

    public void MushroomUIAppearance(bool isTrue)
    {
        mushroomImage.SetActive(isTrue);
        nextButton.SetActive(false);
        toTownButton.SetActive(false);
    }
}
