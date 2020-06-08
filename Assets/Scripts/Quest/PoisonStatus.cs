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

    public GameObject poisonIcon;

    private GameObject poisonIconChild;

    private GameObject playerUIPanel;   // アイコンの親オブジェクトになるもの.


    public PlayerManager Player { get => player; set => player = value; }
    public PlayerUIManager PlayerUI { get => playerUI; set => playerUI = value; }
    public GameObject PoisonIconChild { get => poisonIconChild; set => poisonIconChild = value; }


    BattleManager BattleManager => BattleManager.instance;


    private void Awake()
    {
        // 生成されたら毒エフェクトをアタッチ.
        poisonEffect = Resources.Load<GameObject>("PoisonEffect");

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

        playerUI.ToPoisonPanel();   // UIを毒状態表示にする.

        DialogTextManager.instance.SetScenarios(new string[] { "あなたの体に『毒』がまわった！" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        BattleManager.PoisonDirecting = false;
    }

    public IEnumerator PoisonDirection(PlayerManager player, int poisonDamage)
    {
        BattleManager.instance.PoisonDirecting = true;

        count++;

        Player = player;
        Player.Damage(poisonDamage);

        questManager = GameObject.Find("QuestManager").GetComponent<QuestManager>();
        // battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        /*
        // 毒をうけた初回時にのみ表示させるダイアログ.
        if (!PlayerPrefs.HasKey("Poisoned"))    // PlayerPrefsがこのキーを持っていなかったら
        {
            SaveDataInitialize();
            Instantiate(poisonEffect, this.transform, false);   // 毒エフェクト.
            
            // 毒アイコンを生成するが、それはプレイヤーパネルの入れ子にする.
            PoisonIconChild = Instantiate(poisonIcon) as GameObject;
            PoisonIconChild.transform.SetParent(playerUIPanel.transform, false);
            PoisonIconChild.transform.Translate(-4, 4, 0);
            
            DialogTextManager.instance.SetScenarios(new string[] { "あなたの体に『毒』がまわった！" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
        }
        */

        // ヒット音(SE).
        SoundManager.instance.PlayButtonSE(1);

        DialogTextManager.instance.SetScenarios(new string[] { "あなたは" + poisonDamage + "の毒のダメージをうけた" });
        PlayerUI.UpdateUI(BattleManager.Player);

        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

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
