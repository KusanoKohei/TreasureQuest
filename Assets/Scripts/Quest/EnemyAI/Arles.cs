using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arles : EnemyManager
{
    EnemyManager enemy;

    public GameObject poisonEffect;

    private BattleManager BattleManager => BattleManager.instance;
    override protected void Start()
    {
        base.Start();   // 継承元のStart()が書き換えられないようにする。（参照元"SmileMeBaby!2.0  Player.cs").



        enemy = this.GetComponent<EnemyManager>();
        enemyUI = GameObject.Find("EnemyUICanvas").GetComponent<EnemyUIManager>();
        enemyUI.nameObj.transform.localPosition = new Vector3(-66, 516, 0);
        enemyUI.hpObj.transform.localPosition = new Vector3(228, 431, 0);


        //-- 敵プレハブのパラメータをEnemyManagerに引き渡し反映させる. -- //
        enemy.name = this.name;
        enemy.hp = this.hp;
        enemy.maxHP = this.maxHP;
        enemy.atk = this.atk;
        enemy.spd = this.spd;
        enemy.critical = this.critical;
        enemy.dodge = this.dodge;
        enemy.buff = this.buff;
        enemy.AllowedAction = true;

        enemy.exp = this.exp;

        // -------------------------------------------------------------- //

        enemyUI.SetupUI(enemy);
    }

    public void SelectAction()
    {
        enemy.AllowedAction = false;

        int r;
        r = Random.Range(0, 6);
        // r = 6; // デバッグ用.

        if ((r >= 4) && (r < 6))
        {
            StartCoroutine(ArlesFireBreath());
        }
        else
        {
            StartCoroutine(ArlesCommonAttack());
        }
    }

    public IEnumerator ArlesCommonAttack()
    {
        StartCoroutine(enemy.Attacks());
        yield return new WaitWhile(() => enemy.NowActive);

        BattleManager.EndOfEnemyTurn();
    }

    public IEnumerator ArlesFireBreath()
    {
        // fireBreath(SE).
        SoundManager.instance.PlayButtonSE(8);

        DialogTextManager.instance.SetScenarios(new string[] { this.name + "は 燃えさかる炎をはいた！" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        if (Player.DodgeRate(Player.Dodge))
        {
            DialogTextManager.instance.SetScenarios(new string[] { "あなたは見事にかわした！" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
        }
        else
        {
            enemy.buff += (int)(enemy.atk/4);
            int damage = enemy.Attack(Player, enemy.buff);
            BattleManager.playerDamagePanel.DOShakePosition(0.3f, 0.5f, 20, 0, false, true);

            playerUI.UpdateUI(Player);  // PlayerのUIを更新.
            DialogTextManager.instance.SetScenarios(new string[] { "あなたは" + damage + " のダメージをうけた" });
            yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

            enemy.buff = 0;
        }

        BattleManager.EndOfEnemyTurn();
    }
}
