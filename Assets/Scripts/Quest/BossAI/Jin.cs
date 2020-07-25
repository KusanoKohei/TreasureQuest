using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Jin : EnemyManager
{
    EnemyManager enemy;

    public GameObject poisonEffect;

    private bool charged = false;

    private bool berserkMessaged = false;   // バーサーク状態になったかのメッセージを表示したかどうか（一度のみの起動）.
    private bool berserkMessaging = false;
    private bool berserkAtOnce = false;

    private bool iceBurnDirecting = false;
    private bool jinDirecting = false;
    private int waited = 0;

    public BattleManager BattleManager => BattleManager.instance;
    private DialogTextManager Dialog => DialogTextManager.instance;

    public bool JinDirecting { get => jinDirecting; set => jinDirecting = value; }

    enum Status
    {
        normal,
        berserk
    }

    Status status;


    override protected void Start()
    {
        base.Start();   // 継承元のStart()が書き換えられないようにする。（参照元"SmileMeBaby!2.0  Player.cs").

        enemy = this.GetComponent<EnemyManager>();
        enemyUI = GameObject.Find("EnemyUICanvas").GetComponent<EnemyUIManager>();
        enemyUI.nameObj.transform.localPosition = new Vector3(-66, 516, 0);
        enemyUI.hpObj.transform.localPosition = new Vector3(0, 431, 0);

        // レベル３くらいでも上手いことやれば勝てるように.
        if (Player.Level <= 3)
        {
            this.maxHP = 350;
            this.hp = 350;
            this.atk = 25;
        }
 
        //-- 敵プレハブのパラメータをEnemyManagerに引き渡し反映させる. -- //
        enemy.name = this.name;
        enemy.hp = this.hp;
        enemy.maxHP = this.maxHP;
        enemy.atk = this.atk;
        enemy.spd = this.spd;
        enemy.critical = this.critical;
        enemy.dodge = this.dodge;
        enemy.AllowedAction = true;

        enemy.exp = this.exp;

        // ボスモンスターの状態.
        status = Status.normal;

        // -------------------------------------------------------------- //

        enemyUI.SetupUI(enemy);

    }

    public void SelectAction()
    {
        berserkAtOnce = false;
        // ステータス状態をチェック.
        SwitchStatus();

        switch (status)
        {
            case Status.normal:
                NormalActionSelect();
                break;

            case Status.berserk:
                BerserkActionSelect();
                break;

            default:
                NormalActionSelect();
                break;
        }
    }

    public void SwitchStatus()
    {
        if(enemy.hp <= (enemy.maxHP / 2))
        {
            status = Status.berserk;

            if (berserkMessaged)return;
        }

        else status = Status.normal;
    }

    public IEnumerator BerserkMessageAtOnce()
    {
        Debug.Log("BerserkMessageAtOnce()");

        berserkMessaged = true;
        berserkMessaging = true;
        berserkAtOnce = true;

        // ジンを震えさせる.
        this.transform.DOShakePosition(0.3f, 0.3f, 20, 0, false, true);

        SoundManager.instance.PlayButtonSE(14); // スチーム音.

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は怒り出した！" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);
        DialogTextManager.instance.SetScenarios(new string[] { "危険な攻撃をくりだしてくる" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // メッセージが表示しきるまで待つ.
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        
        berserkMessaging = false;

        BerserkActionAtOnce();
    }

    public void BerserkActionAtOnce()
    {
        Debug.Log("BerserkActionAtOnce()");


        if(Player.Level >= 5)
        {
            if(enemy.hp <= enemy.maxHP / 4)
            {
                StartCoroutine(JinHealDirecting());
            }
            else
            {
                StartCoroutine(IceProtect());
            }
        }
        else 
        {
            if(enemy.hp <= enemy.maxHP / 6)
            {
                StartCoroutine(JinHealDirecting());
            }
            else
            {
                if (Player.Level == 4)
                {
                    StartCoroutine(IceProtect());
                }
                else
                {
                    BerserkActionSelect();
                }
            }
        }
    }

    public void NormalActionSelect()
    {
        int r;
        r = Random.Range(0, 11);
        // r = 1; // デバッグ用.

        // レベル３でも倒せそうに優遇する.
        if (Player.Level <= 3)
        {
            if (enemy.hp <= (enemy.hp / 7))
            {
                int d = Random.Range(0, 7);
                if (d < 4)
                {
                    StartCoroutine(JinHealDirecting());
                }
                else
                {
                    StartCoroutine(IceNiddle());  // 通常攻撃.
                }
            }
            else if (r == 10)
            {
                StartCoroutine(IceNiddle());   // アイスニードル.
            }
            
            else if ((6 <= r) && (r <= 9))
            {
                if (waited > 0)
                {
                    waited = 0;
                    StartCoroutine(JinCommonAttack());   // 通常攻撃.
                }
                else
                {
                    StartCoroutine(JinWaiting());
                }
            }
            else
            {
                StartCoroutine(JinCommonAttack());       // 通常攻撃.
            }
        }
        else
        {
            if (enemy.hp <= (enemy.hp / 4))
            {
                int d = Random.Range(0, 7);
                if (d < 4)
                {
                    StartCoroutine(JinHealDirecting());
                }
                else
                {
                    StartCoroutine(JinCommonAttack());  // 通常攻撃.
                }
            }
            else if ((r >= 8) && (r <= 10))
            {
                StartCoroutine(IceNiddle());    // アイスニードル.
            }
            else
            {
                StartCoroutine(JinCommonAttack());       // 通常攻撃.
            }
        }
    }

    public void BerserkActionSelect()
    {
        if (berserkMessaged == false)
        {
            StartCoroutine(BerserkMessageAtOnce());
        }
        else
        {
            if (Player.Level <= 3)
            {
                BASverWeak();
            }
            else
            {
                BASverStrong();
            }
        }
    }

    public void BASverWeak()
    {
        int r;
        r = Random.Range(0, 11);

        if(enemy.hp <= enemy.maxHP / 6)
        {
            int n;
            n = Random.Range(0, 3);

            if(n == 0)
            {
                Debug.Log("BASverWeak > 再抽選");
                BASverWeak();   // 再抽選.
            }
            else
            {
                StartCoroutine(JinHealDirecting());
            }
        }
        else
        {
            if (charged)
            {
                StartCoroutine(IceBurn());
            }
            else
            {
                switch (r)
                {
                    case 11:
                    case 10:
                        StartCoroutine(ChargeDirecting());
                        break;

                    case 9:
                    case 8:
                        StartCoroutine(IceProtect());
                        break;

                    case 7:
                    case 6:
                        StartCoroutine(IceNiddle());
                        break;

                    case 5:
                    case 4:
                        StartCoroutine(JinWaiting());
                        break;

                    case 3:
                    case 2:
                    case 1:
                    case 0:
                        StartCoroutine(JinCommonAttack());
                        break;
                }
            } 
        }
    }

    public void BASverStrong()
    {
        int r;
        r = Random.Range(0, 11);

        if (enemy.hp <= enemy.maxHP / 4)
        {
            int n;
            n = Random.Range(0, 3);

            if (n == 0)
            {
                Debug.Log("BASverStrong > 再抽選");
                BASverStrong();   // 再抽選.
            }
            else
            {
                StartCoroutine(JinHealDirecting());
            }
        }
        else
        {
            if (charged)
            {
                StartCoroutine(IceBurn());
            }
            else
            {
                switch (r)
                {
                    case 11:
                    case 10:
                    case 9:
                        StartCoroutine(ChargeDirecting());
                        break;

                    case 8:
                    case 7:
                        StartCoroutine(IceProtect());
                        break;

                    case 6:
                    case 5:
                    case 4:
                        StartCoroutine(IceNiddle());
                        break;

                    case 3:
                    case 2:
                    case 1:
                    case 0:
                        StartCoroutine(JinCommonAttack());
                        break;
                }
            }
        }
    }
 

    public IEnumerator JinCommonAttack()
    {
        yield return new WaitWhile(() => JinDirecting);
        JinDirecting = true;
        
        yield return new WaitWhile(() => berserkMessaging);     // バーサークモード切り替わり時のメッセージ中は止めておく.

        StartCoroutine(enemy.Attacks());
        yield return new WaitWhile(() => enemy.NowActive);

        // プレイヤーレベル4以上かつ、攻撃が当たったときかつ、毒状態ではない時にのみ毒状態にかかるかの判定.
        if (Player.Level>=5 && enemy.Hitted && (!Player.Poison))           {
            PoisonProbability();
            // HittedはBattleManagerで初期化する.
            // 毒状態にかかった演出が終了するまで待機させる.
            yield return new WaitWhile(() => BattleManager.PoisonDirecting);
        }

        JinDirecting = false;

        BattleManager.EndOfEnemyTurn();
    }

    public IEnumerator IceBurn()
    {
        berserkAtOnce = false;
        iceBurnDirecting = true;

        yield return new WaitWhile(() => JinDirecting);
        JinDirecting = true;

        yield return new WaitWhile(() => berserkMessaging);     // バーサークモード切り替わり時のメッセージ中は止めておく.

        // IceNiddle(SE).
        SoundManager.instance.PlayButtonSE(9);

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は 氷塊をつくり\n大爆発させた" });
        yield return new WaitForSeconds(1.5f);

        // breaking_a_glass(SE).
        SoundManager.instance.PlayButtonSE(12);
        DialogTextManager.instance.SetScenarios(new string[] { "氷のかたまりが迫る！"});

        yield return new WaitForSeconds(1.5f);


        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        // 実質的な『アイスバーン』の処理.
        StartCoroutine(IceBurnDirecting());
        yield return new WaitWhile(() => iceBurnDirecting);


        enemy.buff = 0;     // かけておいたデバフを戻しておく.
        Player.DodgeCul(-2);
        charged = false;


        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;

        JinDirecting = false;

        BattleManager.EndOfEnemyTurn();
    }

    private IEnumerator IceBurnDirecting()
    {
        for (int i = 0; i < 5; i++)
        {
            int damage;

            if (Player.DefenceMode == false)
            {
                Player.DodgeCul(1);    // 回避率を上げる. 『かまえる』で既に3上がっている可能性があるので上昇は2に抑える.
            }

            enemy.buff -= 15;

            // もし敵のHPが０ならば、処理を抜け出す.
            if (Player.Hp <= 0)
            {
                break;
            }


            if (Player.DodgeRate(Player.Dodge))
            {
                // 空振りのSE
                SoundManager.instance.PlayButtonSE(3);

                DialogTextManager.instance.SetScenarios(new string[] { "あなたは 見事にかわした！" });
                yield return new WaitForSeconds(SettingManager.MessageSpeed);

            }
            else if (enemy.CriticalHit(enemy.critical))
            {
                // クリティカルヒット(SE).
                SoundManager.instance.PlayButtonSE(2);

                damage = enemy.CriticalAttack(Player, buff);

                DialogTextManager.instance.SetScenarios(new string[] { "会心の一撃！" });
                yield return new WaitForSeconds(SettingManager.MessageSpeed / 2);
                DialogTextManager.instance.SetScenarios(new string[] { "あなたは" + damage + "のダメージをうけた" });
                yield return new WaitForSeconds(SettingManager.MessageSpeed / 2);
            }

            else
            {
                // ヒット音(SE).
                SoundManager.instance.PlayButtonSE(1);

                damage = Attack(Player, buff);

                DialogTextManager.instance.SetScenarios(new string[] { "あなたは" + damage + "のダメージをうけた" });
                yield return new WaitForSeconds(SettingManager.MessageSpeed);
            }

            playerUI.UpdateUI(Player);  // PlayerのUIを更新.    
        }

        // 下げておいたクリティカル率を戻す.
        enemy.critical = this.critical;

        BattleManager.CheckPlayerAlive();   // Playerが死んでいないか？

        if (!Player.Dead)
        {
            DialogTextManager.instance.SetScenarios(new string[] { "せまりくる氷をしのぎ切った！" });
        }

        iceBurnDirecting = false;

        yield return new WaitForSeconds(2.0f);
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
    }

    public IEnumerator ChargeDirecting()
    {
        Debug.Log("ChargeDirecting()");

        JinDirecting = true;

        yield return new WaitWhile(() => berserkMessaging);     // バーサークモード切り替わり時のメッセージ中は止めておく.

        charged = true;

        // chargeEffect(SE).
        SoundManager.instance.PlayButtonSE(6);


        // チャージエフェクト.
        GameObject pwrEffect = Resources.Load<GameObject>("PwrEffect");
        pwrEffect.transform.localPosition = this.transform.position;
        pwrEffect.transform.localScale = new Vector3(3, 3, 0);
        Instantiate(pwrEffect, this.transform, false);

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は\n力をためだした……" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // メッセージが表示しきるまで待つ.
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);

        JinDirecting = false;

        BattleManager.EndOfEnemyTurn();
    }

    public IEnumerator IceProtect()
    {
        // 既にプロテクションが張られていたら行動の再抽選.
        if (enemy.Protection != null)
        {
            Debug.Log("既にプロテクションが張られていたから行動の再抽選.");
            SelectAction();
        }
        else
        {
            yield return new WaitWhile(() => berserkMessaging);     // バーサークモード切り替わり時のメッセージ中は止めておく.

            yield return new WaitWhile(() => JinDirecting);
            JinDirecting = true;

            // this（ジン）にProtectionStatusコンポーネントを追加する.
            enemy.Protection = enemy.gameObject.AddComponent<ProtectionStatus>();

            // voice_of_light 防御エフェクト(SE).
            SoundManager.instance.PlayButtonSE(4);

            // 防御エフェクト.
            GameObject defenceEffect = Resources.Load<GameObject>("DefenceEffect");
            defenceEffect.transform.localPosition = new Vector3(0, 0, 0);
            defenceEffect.transform.localScale = new Vector3(3, 3, 0);
            Instantiate(defenceEffect, enemy.transform, false);

            DialogTextManager.instance.SetScenarios(new string[] { this.name + "は\n氷の壁をつくりだした" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed + 1.0f);
            DialogTextManager.instance.SetScenarios(new string[] { "あなたの次の『こうげき』は\n 無効になります" });
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

            JinDirecting = false;


            if (berserkAtOnce && Player.Level >= 5)
            {
                StartCoroutine(ChargeDirecting());
            }
            else if (berserkAtOnce && Player.Level == 4)
            {
                BerserkActionSelect();
            }
            else
            {
                BattleManager.EndOfEnemyTurn();
            }
        }
    }

    public IEnumerator IceNiddle()
    {
        yield return new WaitWhile(() => JinDirecting);
        JinDirecting = true;


        yield return new WaitWhile(() => berserkMessaging);     // バーサークモード切り替わり時のメッセージ中は止めておく.

        // IceNiddle(SE).
        SoundManager.instance.PlayButtonSE(9);

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は\n巨大なツララを発射した！" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        enemy.buff += 20;
        int damage = enemy.Attack(Player, enemy.buff);
        BattleManager.playerDamagePanel.DOShakePosition(0.3f, 0.5f, 20, 0, false, true);
        playerUI.UpdateUI(Player);  // PlayerのUIを更新.
        DialogTextManager.instance.SetScenarios(new string[] {"あなたは"+damage+" のダメージをうけた" });


        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;

        JinDirecting = false;

        enemy.buff = 0;
        BattleManager.EndOfEnemyTurn();
    }

    private IEnumerator JinHealDirecting()
    {
        Debug.Log("JinHealDirecting()");

        yield return new WaitWhile(() => JinDirecting);
        JinDirecting = true;


        // healEffect(SE).
        SoundManager.instance.PlayButtonSE(5);

        // 回復エフェクト.
        GameObject healEffect = Resources.Load<GameObject>("HealEffect");
        healEffect.transform.localPosition = new Vector3(0, 0, 0);
        healEffect.transform.localScale = new Vector3(5, 5, 0);
        Instantiate(healEffect, this.transform, false);

        int healPoint;

        // プレイヤーレベルに応じて回復量を変化させる.
        if(Player.Level<= 3)
        {
            healPoint = (int)this.maxHP / 8; 
        }
        else
        {
            healPoint = (int)this.maxHP / 5;
        }
        
        enemy.hp += healPoint;
        if(enemy.hp>= enemy.maxHP)
        {
            enemy.hp = enemy.maxHP;
        }
        
        enemyUI.UpdateUI(enemy);

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は回復呪文を唱えた" });
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

        JinDirecting = false;


        BattleManager.EndOfEnemyTurn();
    }

    public IEnumerator JinWaiting()
    {
        Debug.Log("JinWaiting()");

        yield return new WaitWhile(() => JinDirecting);
        JinDirecting = true;

        waited++;

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は 様子をみている" });
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

        JinDirecting = false;
        enemy.IsTurned = true;
        BattleManager.EndOfEnemyTurn();
    }

    public void PoisonProbability()
    {
        int r;
        r = Random.Range(0, 3);
        if (r == 0)
        {
            Player.Poison = Player.gameObject.AddComponent<PoisonStatus>();
        }
        else
        {

        }
    }
}

