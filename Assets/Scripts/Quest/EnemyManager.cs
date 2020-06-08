using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// 敵を管理するもの（ステータス/クリック検出）
public class EnemyManager : MonoBehaviour
{
    Action tapAction;           // クリックされたときに実行したい関数を登録する変数（ただし外部の関数）.

    public new string name;
    public int hp;
    public int maxHP;
    public int atk;
    public int spd;
    public int critical;
    public int dodge;
    public int buff=0;

    public int buffSpd;
    public int buffCritical;

    public int exp;
    public GameObject hitEffect;

    private bool hitted = false;
    private bool allowedAction = false;
    private bool isTurned = false;      // ターン内の行動を終わらせているか.
    private bool nowActive = false;
    private bool nowProtection = false;
    private bool backAttacking = false;
    private bool enemyDead = false;

    public bool Hitted { get => hitted; set => hitted = value; }
    public bool AllowedAction { get => allowedAction; set => allowedAction = value; }
    public bool IsTurned { get => isTurned; set => isTurned = value; }
    public bool NowActive { get => nowActive; set => nowActive = value; }
    public bool NowProtection { get => nowProtection; set => nowProtection = value; }
    public bool BackAttacking { get => backAttacking; set => backAttacking = value; }
    public bool EnemyDead { get => enemyDead; set => enemyDead = value; }

    public ProtectionStatus Protection { get; set; }

    // public PlayerManager Player;

    public PlayerUIManager playerUI;

    public EnemyUIManager enemyUI;

    public QuestManager questManager;

    public List<GameObject> enemyPrefabs;

    public Arles        arles;
    public C_Zombie     c_zombie;
    public Jin          jin;
    public Gladiator    gladiator;

    string enemyType;

    public PlayerManager Player => PlayerManager.instance;
    private BattleManager BattleManager => BattleManager.instance;
    private DialogTextManager Dialog => DialogTextManager.instance;



    virtual protected void Start()
    {
        questManager = GameObject.Find("QuestManager").GetComponent<QuestManager>();
        playerUI = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUIManager>();
    }

    public void EnemyTurn()
    {
        AllowedAction = true;

        switch (name)
        {
            case "ばけねこ":
                arles = enemyPrefabs[0].GetComponent<Arles>();
                arles.SelectAction();
                break;

            case "クレイジーゾンビ":
                c_zombie = enemyPrefabs[1].GetComponent<C_Zombie>();
                c_zombie.SelectAction();
                break;

            case "秘宝をまもるマジン":
                jin = enemyPrefabs[2].GetComponent<Jin>();
                jin.SelectAction();
                break;
            /*
            case "闇の剣闘士":
                gladiator = enemyPrefabs[2].GetComponent<Gladiator>();
                gladiator.SelectAction();
                break;
            */

            default:
                break;
        }
    }

    public int Attack(PlayerManager player, int buff)
    {
        int damage = player.Damage(atk+buff);
        return damage;
    }

    public int CriticalAttack(PlayerManager player, int buff)
    {
        int damage = player.Damage((atk/2*3)+buff);
        return damage;
    }

    public int Damage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            this.EnemyDead = true;
        }

        return damage;
    }

    public bool CriticalHit(int criticalRate)
    {
        int n = UnityEngine.Random.Range(0, 10);

        if (n <= criticalRate)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DodgeRate(int dodgeRate)
    {
        int n = UnityEngine.Random.Range(0, 10);
        return  n < dodgeRate;
    }

    public void BackAttackBuff()
    {
        this.BackAttacking = true;

        buffSpd = (this.spd /3)*2;
        buffCritical = 3;

        this.critical += buffCritical;
        this.spd += buffSpd;
    }

    public void AddEventListenerOnTap(Action action)
    {
        tapAction += action;
    }

    public void OnTap()
    {
        tapAction();
    }

    public IEnumerator Attacks()
    {
        NowActive = true;

        int damage;

        if (Player.DodgeRate(Player.Dodge))
        {
            Hitted = false;

            // プレイヤーの回避率をレベルに応じて初期化.
            Player.Dodge = Level_ParameterManager.playerLevel[Player.Level - 1, 5];
            
            // 回避音(SE).
            SoundManager.instance.PlayButtonSE(3);


            // ダメージを与えられず、回避のダイアログのコルーチン.
            DialogTextManager.instance.SetScenarios(new string[] { this.name + "の攻撃" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
            DialogTextManager.instance.SetScenarios(new string[] { "あなたは見事にかわした！" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        }
        else
        {
            Hitted = true;

            // 画面の振動.
            BattleManager.playerDamagePanel.DOShakePosition(0.3f, 0.5f, 20, 0, false, true);

            if (this.CriticalHit(this.critical))
            {
                damage = CriticalAttack(Player, buff);

                // クリティカルヒット(SE).
                SoundManager.instance.PlayButtonSE(2);

                DialogTextManager.instance.SetScenarios(new string[] { this.name + "の攻撃" });
                yield return new WaitForSeconds(SettingManager.instance.MessageSpeed/2);
                DialogTextManager.instance.SetScenarios(new string[] { "会心の一撃！" });
                yield return new WaitForSeconds(SettingManager.instance.MessageSpeed/2);
            }
            // falseならば
            else
            {
                Hitted = true;

                // ヒット音(SE).
                SoundManager.instance.PlayButtonSE(1);

                damage = Attack(Player, buff);

                DialogTextManager.instance.SetScenarios(new string[] { this.name + "の攻撃！" });
                yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
            }

            playerUI.UpdateUI(Player);  // PlayerのUIを更新.

            DialogTextManager.instance.SetScenarios(new string[] { "あなたは" + damage + "のダメージをうけた" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
        }

        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        NowActive = false;
    }
}
