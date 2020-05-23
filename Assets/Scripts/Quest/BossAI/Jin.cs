﻿using System.Collections;
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

    public BattleManager BattleManager => BattleManager.instance;

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

            StartCoroutine(BerserkMessageAtOnce());
        }

        else status = Status.normal;
    }

    public IEnumerator BerserkMessageAtOnce()
    {
        berserkMessaged = true;
        berserkMessaging = true;
        berserkAtOnce = true;

        // ジンを震えさせる.
        this.transform.DOShakePosition(0.3f, 0.3f, 20, 0, false, true);

        SoundManager.instance.PlayButtonSE(14); // スチーム音.

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は怒り出した！" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
        DialogTextManager.instance.SetScenarios(new string[] { "危険な攻撃をくりだしてくる" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed*2);

        berserkMessaging = false;
    }


    public void NormalActionSelect()
    {
        int r;
        r = Random.Range(0, 8);
        r = 1; // デバッグ用.

        if (enemy.hp <= (enemy.hp / 4))
        {
            StartCoroutine(JinHealDirecting());
        }
        else if (r == 7)
        {
            StartCoroutine(IceProtect());   // 氷の壁の防御.
        }
        else if ((r == 5)||(r == 6))
        {
            StartCoroutine(IceNiddle());    // アイスニードル.
        }
        else
        {
            StartCoroutine(JinCommonAttack());              // 通常攻撃.
        }
    }

    public void BerserkActionSelect()
    {
        int r;
        r = Random.Range(0, 10);


        if (enemy.hp <= (enemy.hp / 4))
        {
            StartCoroutine(JinHealDirecting());
        }
        else if (berserkMessaging)
        {
            r = 6;
        }
        else if (berserkAtOnce && Player.Level>=5)
        {
            r = 8;
        }

        if (charged)    // 事前に力をためていたなら.
        {
            StartCoroutine(IceBurn());              // 必殺技『アイスバーン』.
        }
        else if ((r==8)||(r==9))
        {
            StartCoroutine(ChargeDirecting());      // 必殺技前の溜め.
        }
        else if ((r==6)||(r==7))
        {
            StartCoroutine(IceProtect());           // 氷の壁.
        }
        else if ((3 <= r) || (r <= 5))
        {
            StartCoroutine(IceNiddle());            // アイスニードル. 
        }
        else
        {
            Debug.Log("ここまで到達");

            StartCoroutine(JinCommonAttack());  // 通常攻撃.
        }
    }

    public IEnumerator JinCommonAttack()
    {
        yield return new WaitWhile(() => berserkMessaging);     // バーサークモード切り替わり時のメッセージ中は止めておく.

        StartCoroutine(enemy.Attacks());
        yield return new WaitWhile(() => enemy.NowActive);

        /*
        if (enemy.Hitted && (!Player.Poison))   // 攻撃が当たったときかつ、毒状態ではない時にのみ毒状態にかかるかの判定.
        {
            PoisonProbability();
            // HittedはBattleManagerで初期化する.
        }

        // 毒状態にかかった演出が終了するまで待機させる.
        yield return new WaitWhile(() => BattleManager.PoisonDirecting);
        */

        BattleManager.EndOfEnemyTurn();
    }

    public IEnumerator IceBurn()
    {
        berserkAtOnce = false;

        iceBurnDirecting = true;

        yield return new WaitWhile(() => berserkMessaging);     // バーサークモード切り替わり時のメッセージ中は止めておく.

        // IceNiddle(SE).
        SoundManager.instance.PlayButtonSE(9);

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は 氷塊をつくり\n大爆発させた" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        // breaking_a_glass(SE).
        SoundManager.instance.PlayButtonSE(12);
        DialogTextManager.instance.SetScenarios(new string[] { "氷のかたまりが迫る！"});
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        StartCoroutine(IceBurnDirecting());

        yield return new WaitWhile(() => iceBurnDirecting);

        enemy.buff = 0;     // かけておいたデバフを戻しておく.
        Player.DodgeCul(-2);
        charged = false;
        BattleManager.EndOfEnemyTurn();
    }

    private IEnumerator IceBurnDirecting()
    {
        for (int i = 0; i < 5; i++)
        {
            int damage;

            if (Player.DefenceMode == false)
            {
                Player.DodgeCul(2);    // 回避率を上げる. 『かまえる』で既に3上がっている可能性があるので上昇は2に抑える.
            }

            enemy.critical -= 1;
            Debug.Log(enemy.critical);

            enemy.buff -= 15;
            // player.Dodge = 0; // デバッグ用.

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
                yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

            }
            else if (enemy.CriticalHit(enemy.critical))
            {
                // クリティカルヒット(SE).
                SoundManager.instance.PlayButtonSE(2);

                damage = enemy.CriticalAttack(Player, buff);

                DialogTextManager.instance.SetScenarios(new string[] { "会心の一撃！" });
                yield return new WaitForSeconds(SettingManager.instance.MessageSpeed / 2);
                DialogTextManager.instance.SetScenarios(new string[] { "あなたは" + damage + "のダメージをうけた" });
                yield return new WaitForSeconds(SettingManager.instance.MessageSpeed / 2);
            }

            else
            {
                // ヒット音(SE).
                SoundManager.instance.PlayButtonSE(1);

                damage = Attack(Player, buff);

                DialogTextManager.instance.SetScenarios(new string[] { "あなたは" + damage + "のダメージをうけた" });
                yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
            }

            playerUI.UpdateUI(Player);  // PlayerのUIを更新.    
        }

        // 下げておいたクリティカル率を戻す.
        enemy.critical = this.critical;
        Debug.Log(enemy.critical);

        if (Player.Hp>=0)
        {
            DialogTextManager.instance.SetScenarios(new string[] { "せまりくる氷をしのぎ切った！" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
        }

        iceBurnDirecting = false;
    }

    public IEnumerator ChargeDirecting()
    {
        yield return new WaitWhile(() => berserkMessaging);     // バーサークモード切り替わり時のメッセージ中は止めておく.

        charged = true;

        // chargeEffect(SE).
        SoundManager.instance.PlayButtonSE(6);


        // 防御エフェクト.
        GameObject pwrEffect = Resources.Load<GameObject>("PwrEffect");
        pwrEffect.transform.localPosition = this.transform.position;
        pwrEffect.transform.localScale = new Vector3(3, 3, 0);
        Instantiate(pwrEffect, this.transform, false);

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は\n力をためだした……" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed + 1.0f);

        BattleManager.EndOfEnemyTurn();
    }

    public IEnumerator IceProtect()
    {
        yield return new WaitWhile(() => berserkMessaging);     // バーサークモード切り替わり時のメッセージ中は止めておく.

        if (!this.berserkAtOnce)
        {
            if (enemy.Protection != null) SelectAction();   // もし氷の壁が残っていたら、行動を改めて選択（バーサーク状態ではない場合).
        }

        // this（ジン）にProtectionStatusコンポーネントを追加する.
        enemy.Protection = enemy.gameObject.AddComponent<ProtectionStatus>();

        // voice_of_light 防御エフェクト(SE).
        SoundManager.instance.PlayButtonSE(4);

        // 防御エフェクト.
        GameObject defenceEffect = Resources.Load<GameObject>("DefenceEffect");
        defenceEffect.transform.localPosition = new Vector3 (0, 0, 0);
        defenceEffect.transform.localScale = new Vector3(30, 30, 0);
        Instantiate(defenceEffect, enemy.transform, false);

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は\n氷の壁をつくりだした" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed + 1.0f);
        DialogTextManager.instance.SetScenarios(new string[] { "あなたの次の『こうげき』は\n 無効になります" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed* + 1.0f);

        if (berserkAtOnce&&Player.Level>=5)
        {
            BerserkActionSelect();
        }
        else
        {
            BattleManager.EndOfEnemyTurn();
        }
    }

    public IEnumerator IceNiddle()
    {
        yield return new WaitWhile(() => berserkMessaging);     // バーサークモード切り替わり時のメッセージ中は止めておく.

        // IceNiddle(SE).
        SoundManager.instance.PlayButtonSE(9);

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は\n巨大なツララを発射した！" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        enemy.buff += 20;
        int damage = enemy.Attack(Player, enemy.buff);
        BattleManager.playerDamagePanel.DOShakePosition(0.3f, 0.5f, 20, 0, false, true);
        playerUI.UpdateUI(Player);  // PlayerのUIを更新.
        DialogTextManager.instance.SetScenarios(new string[] {"あなたは"+damage+" のダメージをうけた" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        enemy.buff = 0;
        BattleManager.EndOfEnemyTurn();
    }

    private IEnumerator JinHealDirecting()
    {
        // healEffect(SE).
        SoundManager.instance.PlayButtonSE(5);

        // 回復エフェクト.
        GameObject healEffect = Resources.Load<GameObject>("HealEffect");
        healEffect.transform.localPosition = new Vector3(0, 0, 0);
        healEffect.transform.localScale = new Vector3(5, 5, 0);
        Instantiate(healEffect, this.transform, false);

        int healPoint = (int)this.maxHP/3;
        enemy.hp += healPoint;
        if(enemy.hp>= enemy.maxHP)
        {
            enemy.hp = enemy.maxHP;
        }
        
        enemyUI.UpdateUI(enemy);

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は回復呪文を唱えた" });
        yield return new WaitForSeconds(2.0f);

        BattleManager.EndOfEnemyTurn();
    }

    public void PoisonProbability()
    {
        int r;
        r = Random.Range(0, 10);
        if (r > 7)
        {
            Player.Poison = Player.gameObject.AddComponent<PoisonStatus>();
        }
        else
        {

        }
    }
}

