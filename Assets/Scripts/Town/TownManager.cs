using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    private PlayerManager Player => PlayerManager.instance;

    private FadeIOManager fadeManager;
    private CanvasGroup   dialogCanvas;

    public PlayerUIManager playerUI;
    public GameObject playerUIPanel;

    public GameObject settingIcon;
    public GameObject howToPlayButton;

    [SerializeField]
    private GameObject toQuestButton;
    [SerializeField]
    private GameObject saveButton;


    private void Start()
    {
        playerUIPanel.transform.localPosition = new Vector3(-300, -450, 0);
        playerUI.SwitchActivateButton(Player.AllowedAction);    // 『にげる』『ためる』など。falseがデフォルト.

        fadeManager = GameObject.Find("FadeCanvas").GetComponent<FadeIOManager>();

        SetActiveTownButton(true);


        // ゲームオーバー後に戻ってきたなら.
        if (Player.Dead || Player.Hp < Player.MaxHP)
        {
            StartCoroutine(TownRefresh());
        }
        else
        {
            StartCoroutine(GetTown());
        }

        // プレイヤーUIの更新.
        playerUI.SetupUI(Player);
        playerUI.UpdateSpcUI(Player);
    }

    public void OnTapToQuestButton()
    {
        SoundManager.instance.PlayButtonSE(0);
    }

    // -------- コルーチン -------------- //

    private IEnumerator GetTown()
    {
        yield return new WaitForSeconds(0.2f);

        // ダイアログが非表示になっていたら表示させておく.
        // GameObject dialogWindow = fadeCanvas.transform.Find("FadeCanvas/DialogUI").gameObject;
        fadeManager.dialogWindow.SetActive(true);
        dialogCanvas = fadeManager.dialogWindow.GetComponent<CanvasGroup>();
        dialogCanvas.ignoreParentGroups = true;

        DialogTextManager.instance.SetScenarios(new string[] { "街についた" });

        // 毒を無効化.
        if (Player.Poison != null)
        {
            Player.Poison.PoisonRefresh();
        }
    }

    private IEnumerator TownRefresh()
    {
        settingIcon.SetActive(false);
        howToPlayButton.SetActive(false);
        saveButton.SetActive(false);
        SetActiveTownButton(false);

        yield return new WaitForSeconds(0.2f);

        // ダイアログが非表示になっていたら表示させておく.
        fadeManager.dialogWindow.SetActive(true);
        dialogCanvas = fadeManager.dialogWindow.GetComponent<CanvasGroup>();
        dialogCanvas.ignoreParentGroups = true;

        DialogTextManager.instance.SetScenarios(new string[] { "街に帰りついた……" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed*2);


        if (Player.Level >= 2 && Player.Dead)
        {
            Player.Dead = false;

            // レベルを１下げる.
            Player.Level -= 1;

            playerUI.UpdateUI(Player);
            playerUI.ToDownLevelPanel();

            DialogTextManager.instance.SetScenarios(new string[] { "あなたは負けて自信をなくし\nレベルが１下がった" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed*3);

            Player.Init_playerParameter(); // 経験値などもリセットされる(HPなどもここで回復).
        }
        else
        {
            Player.UndoParameter();      // 経験値などまでリセットはされない.
        }

        Player.PlayerInitPerBattleEnd();    // バトル終了ごとの初期化処理.

        // 毒を無効化.
        if (Player.Poison != null)
        {
            Player.Poison.PoisonRefresh();
        }


        HPGage hpGage = GameObject.Find("HPGage").GetComponent<HPGage>();
        GameObject obj = Player.transform.root.gameObject;

        hpGage.SetHPBar(Player.transform.root.gameObject);

        playerUI.UpdateUI(Player);

        DialogTextManager.instance.SetScenarios(new string[] { "あなたはケガがなおるまで休み\n体力を回復させた" });


        // 回復エフェクト.
        GameObject healEffect = Resources.Load<GameObject>("HealEffect");
        healEffect.transform.Translate(0, 1, 1);
        healEffect.transform.localScale = new Vector3(5, 5, 0);
        Instantiate(healEffect, new Vector3(0,0,0), Quaternion.identity);

        yield return new WaitForSeconds(3.0f);

        settingIcon.SetActive(true);
        howToPlayButton.SetActive(true);
        saveButton.SetActive(true);
        SetActiveTownButton(true);
    }

    public void SetActiveTownButton(bool ready)
    {
        toQuestButton.SetActive(ready);
        // saveButton.SetActive(ready);
    }
}
