﻿using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUIManager : MonoBehaviour
{
    #region Singleton
    public static PlayerUIManager instance;

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
    
    
    public Text hpText;
    public Text levelText;

    public GameObject hpGage;
    public GameObject spcButton;
    public GameObject runButton;

    public GameObject playerUIPanelShade;

    PlayerManager Player => PlayerManager.instance;


    public void SetupUI(PlayerManager player)
    {
        GameObject obj = player.gameObject;
        hpGage.GetComponent<HPGage>().SetHPBar(obj);

        levelText.text = string.Format("LEVEL : {0}", player.Level);
        hpText.text = string.Format("HP : {0}", player.Hp);
    }

    public void UpdateUI(PlayerManager player)
    {
        GameObject obj = player.gameObject;
        hpGage.GetComponent<HPGage>().SetHPBar(obj);

        levelText.text = string.Format("LEVEL : {0}", player.Level);
        hpText.text = string.Format("HP : {0}", player.Hp);

        if (player.Hp<=0)
        {
            ToDeadPanel();
        }
    }

    public void UpdateSpcUI(PlayerManager player)
    {
        Text spcText = spcButton.GetComponentInChildren<Text>();

        if (player.Pwr == 0)
        {
            spcText.text = "かまえる";
        }
        else if(player.Pwr == 1)
        {
            spcText.text = "ためる";
        }
        else if(player.Pwr == 2)
        {
            spcText.text = "ひっさつ";
        }
    }

    public void TapRunButton()
    {
        if (!Player.AllowedAction) return;

        StopAllCoroutines();                          // 連打によるバグり防止の為.(108.参照).

        StartCoroutine(Player.RunDirecting());
    }

    public void TapSPCommandButton()
    {
        if (!Player.AllowedAction) return;

        StopAllCoroutines();                          // 連打によるバグり防止の為.(108.参照).

        StartCoroutine(Player.SpcDirecting());
    }

    public void SwitchActivateButton(bool activeButton)
    {
        if (Player.Level >= 2)
        {
            spcButton.SetActive(activeButton);

        }
        else
        {
            spcButton.SetActive(false);
        }
        runButton.SetActive(activeButton);    
    }

    public void ToNeutralPanel()
    {
        var neutralUIColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        Image playerUIPanelImage = playerUIPanelShade.GetComponent<Image>();
        playerUIPanelImage.DOColor(neutralUIColor, 1.0f);
    }

    public void ToDeadPanel()
    {
        var deadUIColor = new Color(1.0f, 0.2f, 0.2f, 0.5f);

        Image playerUIPanelImage = playerUIPanelShade.GetComponent<Image>();
        playerUIPanelImage.DOColor(deadUIColor, 1.0f);
    }
    public void ToDownLevelPanel()
    {
        var deadUIColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);

        Image playerUIPanelImage = playerUIPanelShade.GetComponent<Image>();
        playerUIPanelImage.DOColor(deadUIColor, 1.0f);
    }

    public void ToPoisonPanel()
    {
        var poisonUIColor = new Color(1.0f, 0.0f, 1.0f, 0.5f);

        Image playerUIPanelImage = playerUIPanelShade.GetComponent<Image>();
        playerUIPanelImage.DOColor(poisonUIColor, 1.0f);
    }
}
