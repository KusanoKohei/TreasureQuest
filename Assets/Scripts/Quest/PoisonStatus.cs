using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonStatus : MonoBehaviour
{
    public int poisonCount = 0;

    private int poisonDamage;

    private PlayerManager player;

    private QuestManager questManager;

    public GameObject poisonEffect;
    private ParticleSystem poisonParticle;

    public GameObject poisonIcon;

    private GameObject poisonIconChild;

    private GameObject playerUIPanel;   // アイコンの親オブジェクトになるもの.


    // public PlayerManager Player { get => player; set => player = value; }
    public GameObject PoisonIconChild { get => poisonIconChild; set => poisonIconChild = value; }


    BattleManager BattleManager => BattleManager.instance;
    DialogTextManager Dialog => DialogTextManager.instance;

    PlayerManager Player => PlayerManager.instance;
    PlayerUIManager PlayerUI => PlayerUIManager.instance;

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

        playerUIPanel = GameObject.Find("PlayerUIPanel");

        // SaveDataInitialize();

        // Poison(SE).
        SoundManager.instance.PlayButtonSE(7);

        // 毒エフェクト.
        Instantiate(poisonEffect, this.transform, false);

        PlayerUIManager.instance.UpdateUI(Player);   // UIを毒状態表示にする.
        PlayerUI.ToPoisonPanel();

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

    public IEnumerator PoisonDirection(PlayerManager player)
    {
        poisonCount++;

        poisonDamage = Player.MaxHP / 20;

        BattleManager.instance.PoisonDirecting = true;

        Player.Damage(poisonDamage);

        questManager = GameObject.Find("QuestManager").GetComponent<QuestManager>();


        // ヒット音(SE).
        SoundManager.instance.PlayButtonSE(1);

        DialogTextManager.instance.SetScenarios(new string[] { "あなたは" + poisonDamage + "の毒のダメージをうけた" });
        PlayerUI.UpdateUI(Player);

        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // プレイヤーの生死判定.
        if (player.Hp <= 0)
        {
            StartCoroutine(questManager.GameOver());
        }
        else
        {
            if (poisonCount>=5)
            {
                // 一つのバトル中に連続して3回毒ダメージを受けるとリフレッシュ.
                StartCoroutine(PoisonRefresh());
                yield return new WaitForSeconds(SettingManager.MessageSpeed);
                BattleManager.PoisonDirecting = false;
            }
            else
            {
                BattleManager.PoisonDirecting = false;
            }
        }
    }

    public IEnumerator PoisonRefresh()
    {
        BattleManager.PoisonDirecting = true;

        Destroy(GetComponent<PoisonStatus>());  // 毒コンポーネントを削除.
        Player.Poison = null;

        PlayerUI.ToNeutralPanel();  // UIを毒状態から通常状態の表示へ.

        DialogTextManager.instance.SetScenarios(new string[] { "毒はしだいに治っていった" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        BattleManager.PoisonDirecting = false;
    }

    private void SaveDataInitialize()
    {
        PlayerPrefs.SetInt("Poisoned", 1);  // "Poisoned"のキーをint型の値(1)で保存.
    }
}
