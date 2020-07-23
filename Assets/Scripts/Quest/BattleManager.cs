using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    public Transform playerDamagePanel;
    public QuestManager questManager;
    public PlayerUIManager playerUI;
    public EnemyUIManager enemyUI;

    private EnemyManager enemy;

    public GameObject poisonEffect;

    private bool playerDead = false;
    private bool poisonDirecting = false;

    public EnemyManager Enemy { get => enemy; set => enemy = value; }
    public bool PlayerDead { get => playerDead; set => playerDead = value; }
    public bool PoisonDirecting { get => poisonDirecting; set => poisonDirecting = value; }


    public PlayerManager Player => PlayerManager.instance;

    public DialogTextManager Dialog => DialogTextManager.instance;


    public static BattleManager instance;


    #region Singleton

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion


    // Start is called before the first frame update
    private void Start()
    {
        playerUI = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUIManager>();

        enemyUI.gameObject.SetActive(false);
    }

    public void Setup(EnemyManager enemyManager)
    {
        enemyUI.gameObject.SetActive(true);   // EnemyのHP等のUIを表示する.
        Enemy = enemyManager;               // ここでクエストでエンカウントして生成されたインスタンスを、敵として位置付けた.

        Enemy.AddEventListenerOnTap(Player.PlayerAttack);
    }

    public void SwitchBattleOpening(EnemyManager enemy)
    {
        // バトル開始時はプレイヤー、敵、どちらもターンを実行していないフラグをたてる.
        InitTurnFlag();

        // バフ効果をかける.
        CheckPlayerBuffer();

        // 敵プレハブのタグによってボスバトルかどうかを検出.
        switch (enemy.gameObject.tag)
        {
            case "Enemy":
                StartCoroutine(CommonBattleOpening());
                break;

            case "Boss":
                StartCoroutine(BossBattleOpening());
                break;
        }
    }

    IEnumerator CommonBattleOpening()
    {
        SoundManager.instance.PlayBGM("Battle");
        DialogTextManager.instance.SetScenarios(new string[] { "モンスターが現れた！" });


        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        int n = Random.Range(0, 7);
        if (n == 0)
        {
            Enemy.BackAttackBuff();
            DialogTextManager.instance.SetScenarios(new string[] { "不意打ちをうけてしまった！" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
        }
        else if (n == 1 || n == 2)
        {
            Player.BackAttackBuff();
            DialogTextManager.instance.SetScenarios(new string[] { "あなたは敵のすきをついた！" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
        }
        else
        {
            DialogTextManager.instance.SetScenarios(new string[] { Enemy.name + "  が\n襲いかかってきた" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
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

        CheckWhoseTurn();
    }

    IEnumerator BossBattleOpening()
    {
        SoundManager.instance.PlayBGM("BossBattle");
        DialogTextManager.instance.SetScenarios(new string[] { "ボスバトル！！" });
        yield return new WaitForSeconds(2.0f);
        DialogTextManager.instance.SetScenarios(new string[] { Enemy.name + "  が\n襲いかかってきた！" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);


        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        CheckWhoseTurn();
    }

    public void CheckWhoseTurn()
    {
        if (Player.Spd >= Enemy.spd)
        {
            StartCoroutine(Player.SelectPlayerAction());
        }
        else
        {
            Enemy.EnemyTurn();
        }
    }

    public void EndOfPlayerTurn()
    {
        Player.IsTurned = true;
        CheckEnemyAlive();
    }

    public void EndOfEnemyTurn()
    {
        Debug.Log("BattleManager > EndOfEnemyTurn()");

        enemy.IsTurned = true;
        enemy.Hitted = false;
        CheckPlayerAlive();             // プレイヤーのHPがまだ残っているかチェックする.
        CheckWhitchTurn();
    }

    public void CheckPlayerAlive()
    {
        if (Player.Hp <= 0)
        {
            Player.Dead = true;
        }
        else
        {
            Player.Dead = false;
        }
    }

    public void CheckWhitchTurn()
    {
        if (Player.Dead)
        {
            StartCoroutine(questManager.GameOver());
        }
        else if (Player.IsTurned && Enemy.IsTurned)
        {
            StartCoroutine(CheckTurnEndProcess());  // ターンエンド時の特殊処理.
        }
        else
        {
            StartCoroutine(Player.SelectPlayerAction());
        }
    }

    public void CheckEnemyAlive()
    {
        if (Enemy.hp <= 0)
        {
            StartCoroutine(EndBattle());
        }
        else if (Player.IsTurned && Enemy.IsTurned)
        {
            StartCoroutine(CheckTurnEndProcess());  // ターンエンド時の特殊処理.
        }
        else
        {
            Enemy.EnemyTurn();
        }
    }

    // ターンエンド時に何か特別に処理しなくてはならないこと.
    private IEnumerator CheckTurnEndProcess()
    {
        if (Player.Poison != null)  // 毒のインスタンスが生成されていたなら（毒状態なら）.
        {
            StartCoroutine(Player.Poison.PoisonDirection(Player));
        }

        if (Player.BackAttacking)
        {
            Player.BackAttacking = false;
            Player.Spd = Level_ParameterManager.playerLevel[Player.Level - 1, 4];
            Player.Dodge = Level_ParameterManager.playerLevel[Player.Level - 1, 5];
            Player.Critical = Level_ParameterManager.playerLevel[Player.Level - 1, 6];
        }

        if (Enemy.BackAttacking)
        {
            Enemy.BackAttacking = false;
            Enemy.critical -= Enemy.buffCritical;
            Enemy.spd -= Enemy.buffSpd;
        }

        yield return new WaitWhile(() => PoisonDirecting);

        InitTurnFlag();

        CheckWhoseTurn();
    }


    public IEnumerator EndBattle()
    {
        string TAG = Enemy.gameObject.tag;

        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        enemyUI.gameObject.SetActive(false);


        if (Enemy != null)
        {
            Destroy(Enemy.gameObject);
        }

        DialogTextManager.instance.SetScenarios(new string[] { Enemy.name + "を倒した！" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        if (TAG == "Boss")
        {
            questManager.GameClear();
        }
        else
        {
            // 経験値の取得.
            StartCoroutine(Player.GetEXP(Enemy.exp));
            playerUI.SetupUI(Player);

            // 経験値取得に対する演出が終了するまで通さない.
            yield return new WaitWhile(() => PlayerManager.instance.ExpDirecting);

            // バトル終了時に何か特殊な処理があれば.
            EndBattleProcess();

            questManager.ReturnToQuest();
        }
    }

    public void EndBattleProcess()
    {
        Player.Pwr = 0;

        // バフの初期化.
        // InitPlayerBuffer();

        Player.PlayerInitPerBattleEnd();    // バトル終了時ごとに初期化される値.
        playerUI.UpdateSpcUI(Player);

        playerUI.UpdateUI(Player);
    }

    public void InitTurnFlag()
    {
        Player.AllowedAction = false;
        Player.IsTurned = false;
        enemy.IsTurned = false;
        enemy.Hitted = false;
    }

    public void CheckPlayerBuffer()
    {
        if (Player.BuffStatus != null)
        {
            Player.Atk += Player.BuffStatus.BuffAtk;
            Player.Spd += Player.BuffStatus.BuffSpd;
            Player.CriticalCul(2);
            Player.DodgeCul(1);
        }
    }

    public void InitPlayerBuffer()
    {
        if(Player.BuffStatus != null)
        {
            // レベルに応じての初期化.
            Player.Atk      = Level_ParameterManager.playerLevel[Player.Level - 1, 3];
            Player.Spd      = Level_ParameterManager.playerLevel[Player.Level - 1, 4];
            Player.Dodge    = Level_ParameterManager.playerLevel[Player.Level - 1, 5];
            Player.Critical = Level_ParameterManager.playerLevel[Player.Level - 1, 6];
        }

        Destroy(Player.GetComponent<BuffStatus>());
    }
}
