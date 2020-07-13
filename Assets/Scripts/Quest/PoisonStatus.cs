using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonStatus : MonoBehaviour
{
    private int count = 0;

    private PlayerManager player;

    private PlayerUIManager playerUI;

    private QuestManager questManager;

    public GameObject poisonEffect;
    private ParticleSystem poisonParticle;

    public GameObject poisonIcon;

    private GameObject poisonIconChild;

    private GameObject playerUIPanel;   // アイコンの親オブジェクトになるもの.


    // public PlayerManager Player { get => player; set => player = value; }
    public PlayerUIManager PlayerUI { get => playerUI; set => playerUI = value; }
    public GameObject PoisonIconChild { get => poisonIconChild; set => poisonIconChild = value; }


    BattleManager BattleManager => BattleManager.instance;
    DialogTextManager Dialog => DialogTextManager.instance;

    PlayerManager Player => PlayerManager.instance;


    private void Awake()
    {
        // 生成されたら毒エフェクトをアタッチ.
        poisonEffect = Resources.Load<GameObject>("PoisonEffect");
        poisonParticle = poisonEffect.GetComponent<ParticleSystem>();

        StartCoroutine(PoisonAwake());

    }

    public IEnumerator PoisonAwake()
    {
        BattleManager.PoisonDirecting = true;

        PlayerUI = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUIManager>();
        playerUIPanel = GameObject.Find("PlayerUIPanel");

        SaveDataInitialize();

        // Poison(SE).
        SoundManager.instance.PlayButtonSE(7);

        // 毒エフェクト.
        Instantiate(poisonEffect, this.transform, false);

        Debug.Log(Player);
        playerUI.UpdateUI(Player);   // UIを毒状態表示にする.
        playerUI.ToPoisonPanel();

        DialogTextManager.instance.SetScenarios(new string[] { "あなたの体に『毒』がまわった！" });


        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        BattleManager.PoisonDirecting = false;
    }

    public IEnumerator PoisonDirection(PlayerManager player, int poisonDamage)
    {
        BattleManager.instance.PoisonDirecting = true;

        count++;

        Player.Damage(poisonDamage);

        questManager = GameObject.Find("QuestManager").GetComponent<QuestManager>();


        // ヒット音(SE).
        SoundManager.instance.PlayButtonSE(1);

        DialogTextManager.instance.SetScenarios(new string[] { "あなたは" + poisonDamage + "の毒のダメージをうけた" });
        PlayerUI.UpdateUI(BattleManager.Player);


        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        // プレイヤーの生死判定.
        if (player.Hp <= 0)
        {
            StartCoroutine(questManager.GameOver());
        }
        else
        {
            if (count >= 3)
            {

            }

            BattleManager.PoisonDirecting = false;
        }
    }

    public void PoisonRefresh()
    {
        // 毒状態を治す.

        PlayerPrefs.DeleteKey("Poisoned");
        Destroy(GetComponent<PoisonStatus>());  // 毒コンポーネントを削除.

        playerUI.ToNeutralPanel();  // UIを毒状態から通常状態の表示へ.

        DialogTextManager.instance.SetScenarios(new string[] { "毒はしだいに治っていった" });
    }

    private void SaveDataInitialize()
    {
        PlayerPrefs.SetInt("Poisoned", 1);  // "Poisoned"のキーをint型の値(1)で保存.
    }
}
