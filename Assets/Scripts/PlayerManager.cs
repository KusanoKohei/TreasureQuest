using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class PlayerManager : MonoBehaviour
{
    private int level;
    private int maxHP;
    private int hp;
    private int atk;
    private int spd;
    private int dodge;
    private int critical;
    private int skill;
    private int nextEXP;
    private int nowEXP;
    private int kurikoshi;

    private int pwr;

    private bool allowedAction = false;         // SelectPlayerAction() でtrueに、各行動直後にfalseにしています.
    private bool isTurned = false;
    private bool nowActive = false;
    private bool mitokorogiriAction = false;
    private bool backAttacking = false;

    private bool defenceMode = false;
    private bool charging = false;
    private bool expDirecting = false;

    private PlayerUIManager playerUI;
    private EnemyManager enemy;
    private EnemyUIManager enemyUI;
    private BattleManager battleManager;


    private Button runButton;
    private Button spcButton;

    private bool dead = false;

    public int MaxHP { get => maxHP; set => maxHP = value; }
    public int Level { get => level; set => level = value; }
    public int Hp { get => hp; set => hp = value; }
    public int Atk { get => atk; set => atk = value; }
    public int Spd { get => spd; set => spd = value; }
    public int NextEXP { get => nextEXP; set => nextEXP = value; }
    public int NowEXP { get => nowEXP; set => nowEXP = value; }
    public int Pwr { get => pwr; set => pwr = value; }
    public int Dodge { get => dodge; set => dodge = value; }
    public int Critical { get => critical; set => critical = value; }
    public int Skill { get => skill; set => skill = value; }
    public int Kurikoshi { get => kurikoshi; set => kurikoshi = value; }



    public bool DefenceMode { get => defenceMode; set => defenceMode = value; }

    public bool AllowedAction { get => allowedAction; set => allowedAction = value; }
    
    public bool IsTurned { get => isTurned; set => isTurned = value; }  // プレイヤーの行動許可.
    public bool NowActive { get => nowActive; set => nowActive = value; }
    public bool BackAttacking { get => backAttacking; set => backAttacking = value; }
    public bool ExpDirecting { get => expDirecting; set => expDirecting = value; }


    public PoisonStatus Poison { get; set; }
    public bool Dead { get => dead; set => dead = value; }



    public PlayerUIManager PlayerUI { get => playerUI; set => playerUI = value; }

    public EnemyManager Enemy { get => enemy; set => enemy = value; }
    
    public EnemyUIManager EnemyUI { get => enemyUI; set => enemyUI = value; }

    public BattleManager BattleManager => BattleManager.instance;

    private SettingManager SettingManager => SettingManager.instance;


    public GameObject tapToAttack;


    #region Singleton
    public static PlayerManager instance;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    #endregion


    // ------------------------------------------------------ //


    public void Init_playerParameter()  // ニューゲーム時の初期化.
    {
        // this.Level = 3; // デバッグ用.

        this.MaxHP   = Level_ParameterManager.playerLevel[this.Level-1, 1];
        this.Hp      = Level_ParameterManager.playerLevel[this.Level-1, 2];
        this.Atk     = Level_ParameterManager.playerLevel[this.Level-1, 3];
        this.Spd     = Level_ParameterManager.playerLevel[this.Level-1, 4];
        this.Dodge   = Level_ParameterManager.playerLevel[this.Level-1, 5];
        this.Critical= Level_ParameterManager.playerLevel[this.Level-1, 6];
        this.Skill   = Level_ParameterManager.playerLevel[this.Level-1, 7];
        this.NextEXP = Level_ParameterManager.playerLevel[this.Level-1, 8];
        this.nowEXP  = Level_ParameterManager.playerLevel[this.Level-1, 9];

        this.Pwr = 0;
    }

    public void UndoParameter()         // ゲーム途中でのパラメータのリフレッシュ（宿屋に泊まったときとか）.
    {
        this.MaxHP      = Level_ParameterManager.playerLevel[this.Level - 1, 1];
        this.Hp         = Level_ParameterManager.playerLevel[this.Level - 1, 2];
        this.Atk        = Level_ParameterManager.playerLevel[this.Level - 1, 3];
        this.Spd        = Level_ParameterManager.playerLevel[this.Level - 1, 4];
        this.Dodge      = Level_ParameterManager.playerLevel[this.Level - 1, 5];
        this.Critical   = Level_ParameterManager.playerLevel[this.Level - 1, 6];
        this.Skill      = Level_ParameterManager.playerLevel[this.Level - 1, 7];
     
        this.Pwr = 0;
    }

    // バトル終了時ごとに初期化される処理.
    public void PlayerInitPerBattleEnd()
    {
        if (Poison != null)
        {
            GameObject.Destroy(Poison);
            PlayerPrefs.DeleteKey("Poisoned");
        }

        Pwr = 0;
        DefenceMode = false;
        charging = false;
        NowActive = false;
        AllowedAction = false;
        IsTurned = false;

        this.Dodge = Level_ParameterManager.playerLevel[this.Level - 1, 5];
        this.Critical = Level_ParameterManager.playerLevel[this.Level - 1, 6];
    }

    public int HpAdd(int add)
    {
        this.Hp += add;

        if (Hp >= MaxHP) Hp = MaxHP;

        return Hp;
    }

    public int Attack(EnemyManager enemy)
    {
        int damage = BattleManager.Enemy.Damage(Atk);
        return damage;
    }

    public int CriticalAttack(EnemyManager enemy)
    {
        int damage = BattleManager.Enemy.Damage(Atk/2*3);
        return damage;
    }

    public int Damage(int damage)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
        }

        return damage;
    }

    public bool CriticalHit(int criticalRate)
    {
        int n = Random.Range(0, 11);
        // criticalRate = 10;  // デバッグ用.

        if (n < criticalRate)
        {
            // Critical を適正値に戻しておく.
            Critical = Level_ParameterManager.playerLevel[this.Level - 1, 6];
            return true;
        }
        
        return n <= criticalRate;
    }

    public bool DodgeRate(int dodgeRate)
    {
        int n = Random.Range(0, 10);

        if (n < dodgeRate)
        {
            // 回避率を元に戻しておく.
            Dodge = Level_ParameterManager.playerLevel[this.Level - 1, 5];
            DefenceMode = false;
            return true;
        }
        else return false;
    }

    public int CriticalCul(int add)
    {
        Critical += add;

        if (Critical >= 10) Critical = 10;
        return Critical;
    }

    public int DodgeCul(int add)
    {
        Dodge += add;

        if (Dodge >= 10)
        { 
            Dodge = 10; 
        }
        else if (Dodge <= 0)
        {
            Dodge = 0;
        }

            return Dodge;
    }

    private void InitPwr()
    {
        if (this.Skill == 1 && Pwr == 2)
        {
            this.Pwr = 0;
        }
        else if(this.Skill>=2 && Pwr ==3)
        {
            this.Pwr = 0;
        }
    }

    public void BackAttackBuff()
    {
        this.BackAttacking = true;
        this.Spd += this.Spd / 2;   // 素早さを上昇させる.
        this.DodgeCul(3);           // 回避率を上昇させる.
        this.CriticalCul(3);        // クリティカル率を上昇させる.
    }

    // -------------  コルーチン ----------------------- //

    public IEnumerator SelectPlayerAction()
    {
        // プレイヤーUIの表示.
        if (PlayerUI == null)
        {
            PlayerUI = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUIManager>();
        }

        PlayerUI.SwitchActivateButton(true);

        DialogTextManager.instance.SetScenarios(new string[] { "あなたのターンです" });

        // TapToAttack を表示.
        tapToAttack = playerUI.transform.Find("TapToAttack").gameObject;
        tapToAttack.SetActive(true);

        yield return new WaitForSeconds(SettingManager.MessageSpeed/2);

        // 一定秒数待ってからプレイヤーの行動を許可する.
        AllowedAction = true;
    }

    public void PlayerAttack()
    {
        if (!AllowedAction) return;
        // プレイヤーUIの非表示.
        AllowedAction = false;
        playerUI.SwitchActivateButton(false);
        // TapToAttack を非表示.
        tapToAttack.SetActive(false);
        
        NowActive = true;

        StopAllCoroutines();                          // 連打によるバグり防止の為.(108.参照).

        StartCoroutine(PlayerAttackDirecting());
    }

    public IEnumerator PlayerAttackDirecting()
    {
        Enemy = BattleManager.instance.Enemy.GetComponent<EnemyManager>();

        BattleManager.instance = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        
        if (Enemy.Protection != null)
            {
                StartCoroutine(Enemy.Protection.ProtectionDirecting());
                yield return new WaitWhile(() => Enemy.NowProtection);
                PlayerManager.instance.NowActive = false;
            }
        else
            {
                StartCoroutine(PlayerCommonAttack());
            }

        yield return new WaitWhile(() => NowActive);            // nowActiveがfalseになったら通す.
        BattleManager.EndOfPlayerTurn();
    }

    private IEnumerator PlayerCommonAttack()
    {
        int damage;

        BattleManager.instance = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        Enemy = BattleManager.Enemy.GetComponent<EnemyManager>();

        if (Enemy == null)
        {
            Enemy = BattleManager.Enemy.GetComponent<EnemyManager>();
        }

        if (EnemyUI == null)
        {
            EnemyUI = GameObject.Find("EnemyUICanvas").GetComponent<EnemyUIManager>();
        }

        // 敵の攻撃回避関数がTrueならば
        if (Enemy.DodgeRate(Enemy.dodge))
        {
            // 空振り音(SE).
            SoundManager.instance.PlayButtonSE(3);

            // ダメージを与えられず、回避のダイアログのコルーチン.
            DialogTextManager.instance.SetScenarios(new string[] { "あなたの攻撃" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
            DialogTextManager.instance.SetScenarios(new string[] { "だが、かわされてしまった！" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed * 1.0f);
            NowActive = false;
        }
        else if (this.CriticalHit(Critical))
        {
            // クリティカルヒット(SE).
            SoundManager.instance.PlayButtonSE(2);

            damage = CriticalAttack(BattleManager.Enemy);

            // クリティカル率の初期化.
            this.Critical = Level_ParameterManager.playerLevel[this.Level - 1, 6];

            // 会心の一撃のエフェクト.
            GameObject criticalhitEffect = Resources.Load<GameObject>("CriticalHitEffect");
            Instantiate(criticalhitEffect, Enemy.transform.position, Quaternion.identity);

            // 画面の振動.
            Enemy.transform.DOShakePosition(0.7f, 2.0f, 20, 0, false, true);

            DialogTextManager.instance.SetScenarios(new string[] { "あなたの攻撃" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed/2);
            DialogTextManager.instance.SetScenarios(new string[] { "会心の一撃！" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed/2);
            EnemyUI.UpdateUI(Enemy);
            DialogTextManager.instance.SetScenarios(new string[] { BattleManager.Enemy.name + "に" + damage + "のダメージを与えた" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
        }
        // falseならば
        else
        {
            damage = Attack(BattleManager.Enemy);

            // ヒット音(SE).
            SoundManager.instance.PlayButtonSE(1);

            // 通常の攻撃エフェクト.
            GameObject attackhitEffect = Resources.Load<GameObject>("AttackHitEffect");
            Instantiate(attackhitEffect, Enemy.transform.position, Quaternion.identity);
            // 画面の振動.
            Enemy.transform.DOShakePosition(0.3f, 0.3f, 20, 0, false, true);

            DialogTextManager.instance.SetScenarios(new string[] { "あなたの攻撃！" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
            EnemyUI.UpdateUI(Enemy);
            DialogTextManager.instance.SetScenarios(new string[] { BattleManager.Enemy.name + "に" + damage + "のダメージを与えた" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
        }

        NowActive = false;
    }


    public IEnumerator RunDirecting()
    {
        if (!AllowedAction) yield break;

        // プレイヤーUIの非表示.
        AllowedAction = false;
        playerUI.SwitchActivateButton(false);
        // TapToAttack を非表示.
        tapToAttack.SetActive(false);

        BattleManager.instance = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        int n = UnityEngine.Random.Range(-1, 2);

        if (n == -1)
        {
            DodgeCul(5);
            DialogTextManager.instance.SetScenarios(new string[] { "あなたは戦闘から逃げだした" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
            DialogTextManager.instance.SetScenarios(new string[] { "しかし逃げ切れなかった！" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed * 2.0f);

            BattleManager.EndOfPlayerTurn();
        }
        else
        {
            DialogTextManager.instance.SetScenarios(new string[] { "あなたは戦闘から逃げだした" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed * 2);

            BattleManager.EndBattleProcess();   // バトル終了時のやるべきことはやっておく（毒状態の解消とか）.
            BattleManager.InitTurnFlag();       // フラグを戻しておく.

            SceneTransitionManager sceneManager = GameObject.Find("SceneManager").GetComponent<SceneTransitionManager>();
            sceneManager.LoadTo("Town");    // 街へ戻る.
        }
    }

    public IEnumerator SpcDirecting()
    {
        if (!AllowedAction) yield break;

        // プレイヤーUIの非表示.
        AllowedAction = false;
        playerUI.SwitchActivateButton(false);
        // TapToAttack を非表示.
        tapToAttack.SetActive(false);
        nowActive = true;

        if (BattleManager == null)
        {
            BattleManager.instance = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        }

        if (PlayerUI == null)
        {
            PlayerUI = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUIManager>();
        }

        if (Enemy == null)
        {
            Enemy = BattleManager.instance.Enemy.GetComponent<EnemyManager>();
        }

        Pwr++;

        if (Pwr == 1)
        {
            DialogTextManager.instance.SetScenarios(new string[] { "あなたは みがまえた！" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);

            // 効果.
            if (!this.DefenceMode)
            {
                DodgeCul(3);            // 回避率を上げ、上限に達していたら上限までに抑える処理.
                DefenceMode = true;
            }

            // voice_of_light(SE).
            SoundManager.instance.PlayButtonSE(4);

            // 防御エフェクト.
            GameObject defenceEffect = Resources.Load<GameObject>("DefenceEffect");
            defenceEffect.transform.localPosition = new Vector3(0,-2,0);
            defenceEffect.transform.localScale = new Vector3(2, 2, 0);
            Instantiate(defenceEffect, this.transform, false);

            DialogTextManager.instance.SetScenarios(new string[] { Enemy.name + "の\n次の攻撃にそなえた" });
            yield return new WaitForSeconds(2.0f);

            // healEffect(SE).
            SoundManager.instance.PlayButtonSE(5);

            // 回復エフェクト.
            GameObject healEffect = Resources.Load<GameObject>("HealEffect");
            healEffect.transform.localPosition = new Vector3(0,-2,0);
            healEffect.transform.localScale = new Vector3(5, 5, 0);
            Instantiate(healEffect, this.transform, false);

            int healPoint = (int)MaxHP / 5;
            HpAdd(healPoint);                   // 回復量を計算し、上限を超えないように管理する関数に引き渡す.
            PlayerUI.UpdateUI(this);            // HP回復量をPlayerUIに反映させる.

            DialogTextManager.instance.SetScenarios(new string[] { "体力が少し回復した" });
            yield return new WaitForSeconds(2.0f);
            nowActive = false;
        }
        else if (Pwr == 2)
        {
            charging = true;

            InitPwr();  // 条件を満たしていればPwrを0にする.

            if (!charging)
            {
                CriticalCul(3); // クリティカル率を上げる.
            }

            // chargeEffect(SE).
            SoundManager.instance.PlayButtonSE(6);

            // 防御エフェクト.
            GameObject pwrEffect = Resources.Load<GameObject>("PwrEffect");
            pwrEffect.transform.localPosition = new Vector3(0,-2,0);
            pwrEffect.transform.localScale = new Vector3(5, 5, 0);
            Instantiate(pwrEffect, this.transform, false);


            DialogTextManager.instance.SetScenarios(new string[] { "あなたは体中に力をためた！" });
            yield return new WaitForSeconds(2.0f);
            nowActive = false;
        }
        else if (Pwr == 3)
        {
            InitPwr();  // 条件を満たしていればPwrを0にする.
            StartCoroutine(PlayerSkillDirecting());
        }
        else
        {
            Pwr = 1;
        }

        PlayerUI.UpdateSpcUI(this);     // PlayerManagerからPwrを引数として渡し、UIを変更する.

        yield return new WaitWhile(() => mitokorogiriAction);
        yield return new WaitWhile(() => nowActive);
        BattleManager.EndOfPlayerTurn();

    }

    public IEnumerator PlayerSkillDirecting()
    {
        mitokorogiriAction = true;

        // クリティカルヒット音(SE).
        SoundManager.instance.PlayButtonSE(2);

        DialogTextManager.instance.SetScenarios(new string[] { "ひっさつ技！！" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);
        DialogTextManager.instance.SetScenarios(new string[] { "『 三所斬り 』\nの三連続攻撃！" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        StartCoroutine(MitokorogiriDirecting());

        yield return new WaitWhile(() => mitokorogiriAction);

        // バフ効果等を下げる.
        this.Dodge = Level_ParameterManager.playerLevel[this.Level - 1, 5];
        this.Critical = Level_ParameterManager.playerLevel[this.Level - 1, 6];

        PlayerUI.UpdateSpcUI(this);

        DialogTextManager.instance.SetScenarios(new string[] { "あなたは ためた力を 使いはたした" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);
        nowActive = false;
    }

    private IEnumerator MitokorogiriDirecting()
    {
        // 攻撃力を3/4にする.
        this.Atk = this.Atk / 4 * 3;

        int i = 0;
        for (i=0; i < 3; i++)
        {
            Enemy = BattleManager.instance.Enemy.GetComponent<EnemyManager>();

            BattleManager.instance = GameObject.Find("BattleManager").GetComponent<BattleManager>();

            if (Enemy.Protection != null)
            {
                StartCoroutine(Enemy.Protection.ProtectionDirecting());
                // プロテクションの演出が終わるまで待機.
                yield return new WaitWhile(() => Enemy.NowProtection);
            }
            else
            {
                StartCoroutine(PlayerCommonAttack());
            }

            CriticalCul(3);

            // もし敵のHPが０ならば、処理を抜け出す.
            if (Enemy.hp <= 0)
            {
                yield return new WaitForSeconds(SettingManager.instance.MessageSpeed * 2);
                break;
            }

            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed * 2);
        
        }

        // 攻撃力を元に戻す.
        this.Atk = Level_ParameterManager.playerLevel[this.Level - 1, 3];

        mitokorogiriAction = false;
    }

    public IEnumerator GetEXP(int exp)
    {
        ExpDirecting = true;

        if (this.Level >= 5)
        {
            DialogTextManager.instance.SetScenarios(new string[] { "レベルは上限に達しています\nこれ以上経験値は入りません" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
            ExpDirecting = false;
        }
        else
        {
            this.NowEXP += exp;

            DialogTextManager.instance.SetScenarios(new string[] { exp + "の経験値を手に入れた" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

            if (this.NextEXP <= this.NowEXP)
            {
                if (this.Level <= 5)
                {
                    StartCoroutine(LevelUP());
                }
            }
            else
            {
                DialogTextManager.instance.SetScenarios(new string[] { "次のレベルまでの経験値は" + (NextEXP - NowEXP) + "です" });
                yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
                ExpDirecting = false;
            }
        } 
    }

    private IEnumerator LevelUP()
    {
        // BGMを止めておく.
        SoundManager.instance.audioSourceBGM.Stop();
        
        this.Level++;   // レベルを一つ上げる.

        this.Kurikoshi = (this.NowEXP - NextEXP);
        this.NowEXP = 0;   // NowEXPが0以下にならないように.

        Init_playerParameter(); // レベルに合わせての初期化処理.

        this.NowEXP += this.Kurikoshi;  //くりこした経験値分を足してあげる.
        this.Kurikoshi = 0;             // くりこし経験値初期化.

        // SE
        SoundManager.instance.PlayButtonSE(13);

        yield return new WaitForSeconds(SettingManager.MessageSpeed/2); // ちょっと待つ.

        DialogTextManager.instance.SetScenarios(new string[] { "レベルが" + this.Level + "に上がった！" });
        // UIの更新.
        playerUI.UpdateUI(this);

        yield return new WaitForSeconds(SettingManager.MessageSpeed*2);

        if(this.Level == 2)
        {
            DialogTextManager.instance.SetScenarios(new string[] { "『かまえる』『ためる』コマンドを\n使えるようになりました" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed*2);
            DialogTextManager.instance.SetScenarios(new string[] { "あたなのターンの行動選択時に\n使うことができます" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed*2);
        }

        if (this.Level == 3)
        {
            DialogTextManager.instance.SetScenarios(new string[] { "『ひっさつ』コマンドを\n使えるようになりました" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed*2);
            DialogTextManager.instance.SetScenarios(new string[] { "『ためる』コマンドの後に\n使えるようになります" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed*2);
        }

        // エンカウントを振り直す.
        QuestManager.instance.SetEncount();
        
        ExpDirecting = false;
    }
}
