using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    public Transform                playerDamagePanel;
    public QuestManager             questManager;
    public PlayerUIManager          playerUI;
    public EnemyUIManager           enemyUI;
    
    private EnemyManager enemy;
    
    public GameObject poisonEffect;

    private bool playerDead = false;
    private bool poisonDirecting = false;

    public EnemyManager Enemy { get => enemy; set => enemy = value; }
    public bool PlayerDead { get => playerDead; set => playerDead = value; }
    public bool PoisonDirecting { get => poisonDirecting; set => poisonDirecting = value; }

    public PlayerManager Player => PlayerManager.instance;


    public static BattleManager instance;


    #region Singleton

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
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        int n = Random.Range(0, 7);
        if (n == 0)
        {
            Enemy.BackAttackBuff();
            DialogTextManager.instance.SetScenarios(new string[] { "不意打ちをうけてしまった！" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed*2);
        }
        else if (n == 1 || n == 2)
        {
            Player.BackAttackBuff();
            DialogTextManager.instance.SetScenarios(new string[] { "あなたは敵のすきをついた！" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed*2);
         }
        else
        {
            DialogTextManager.instance.SetScenarios(new string[] { Enemy.name + "  が\n襲いかかってきた" });
            yield return new WaitForSeconds(2f);
        }

        CheckWhoseTurn();
    }

    IEnumerator BossBattleOpening()
    {
        SoundManager.instance.PlayBGM("BossBattle");
        DialogTextManager.instance.SetScenarios(new string[] { "ボスバトル！！" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed+1.0f);
        DialogTextManager.instance.SetScenarios(new string[] { Enemy.name + "  が\n襲いかかってきた！" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
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
        enemy.IsTurned = true;
        enemy.Hitted = false;
        CheckPlayerAlive();             // プレイヤーのHPがまだ残っているかチェックする.
    }

    public void CheckPlayerAlive()
    {
        if (Player.Hp <= 0)
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
            StartCoroutine(Player.Poison.PoisonDirection(Player, (Player.MaxHP / 10)));
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

        // yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
    
        yield return new WaitWhile(() => PoisonDirecting);

        InitTurnFlag();

        CheckWhoseTurn();
    }


    public IEnumerator EndBattle()
    {
        string TAG = Enemy.gameObject.tag;

        yield return new WaitForSeconds(1.0f);

        enemyUI.gameObject.SetActive(false);


        if(Enemy!=null)
        {
            Destroy(Enemy.gameObject);
        }

        DialogTextManager.instance.SetScenarios(new string[] { Enemy.name + "を倒した！" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed*2);

        if(TAG == "Boss")
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

        if (Player.Poison != null)
        {
            Player.Poison.PoisonRefresh();  // 毒状態を治す.
        }

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
}
