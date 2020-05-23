using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gladiator : EnemyManager
{
    EnemyManager enemy;

    public GameObject healEffect;

    private BattleManager BattleManager => BattleManager.instance;

    override protected void Start()
    {
        base.Start();   // 継承元のStart()が書き換えられないようにする。（参照元"SmileMeBaby!2.0  Player.cs").

        enemy = this.GetComponent<EnemyManager>();

        enemyUI = GameObject.Find("EnemyUICanvas").GetComponent<EnemyUIManager>();
        enemyUI.nameObj.transform.localPosition = new Vector3(-66,516,0);
        enemyUI.hpObj.transform.localPosition = new Vector3(228, 431, 0);


        enemy.name = this.name;
        enemy.hp = this.hp;
        enemy.maxHP = this.maxHP;
        enemy.atk = this.atk;
        enemy.exp = this.exp;

        enemyUI.SetupUI(enemy);
    }

    public void SelectAction()
    {
        int r;
        r = Random.Range(0,8);

        if (r== 7)
        {
            StartCoroutine(GladiatorDeathBlow());
        }
        else if((r>=5)&&(r<7))
        {
            StartCoroutine(GladiatorHeals());
        }
        else
        {
            StartCoroutine(GladiatorAttacks());

        }
    }

    public IEnumerator GladiatorAttacks()
    {
        yield return new WaitForSeconds(2f);
        SoundManager.instance.PlayButtonSE(1);

        BattleManager.playerDamagePanel.DOShakePosition(0.3f, 0.5f, 20, 0, false, true);
        int damage = Attack(BattleManager.Player);
        BattleManager.playerUI.UpdateUI(BattleManager.Player);

        DialogTextManager.instance.SetScenarios(new string[] {
            this.name +"の攻撃！\nあなたは"+damage +"のダメージをうけた"});

        yield return new WaitForSeconds(1f);

        BattleManager.EndOfEnemyTurn();
    }

    public IEnumerator GladiatorHeals()
    {
        yield return new WaitForSeconds(2f);

        // エフェクトを生成.
        Instantiate(healEffect, this.transform, false);

        Heal(30);                   // 引数は回復文.
        enemyUI.UpdateUI(enemy);    // 敵UIの更新.

        // 回復処理とUIの更新処理はまだ.
        DialogTextManager.instance.SetScenarios(new string[] {
            this.name + "は呪文を唱えた\n"+ this.name + "は回復した！"});

        yield return new WaitForSeconds(1f);

        BattleManager.EndOfEnemyTurn();
    }

    public IEnumerator GladiatorDeathBlow()
    {
        yield return new WaitForSeconds(1f);
        DialogTextManager.instance.SetScenarios(new string[] {
            this.name + "は必殺技の\n『三所斬り』をくりだした！" });

        yield return new WaitForSeconds(1f);

        BattleManager.playerUI.UpdateUI(BattleManager.Player);
        SoundManager.instance.PlayButtonSE(1);
        int damage = Attack(BattleManager.Player);
        BattleManager.playerUI.UpdateUI(BattleManager.Player);
        BattleManager.playerDamagePanel.DOShakePosition(0.3f, 0.5f, 20, 0, false, true);
        DialogTextManager.instance.SetScenarios(new string[] {
            this.name +"の攻撃！\nあなたは"+ damage +"のダメージをうけた"});
        yield return new WaitForSeconds(0.5f);

        SoundManager.instance.PlayButtonSE(1);
        Attack(BattleManager.Player);
        BattleManager.playerUI.UpdateUI(BattleManager.Player);
        BattleManager.playerDamagePanel.DOShakePosition(0.3f, 0.5f, 20, 0, false, true);
        DialogTextManager.instance.SetScenarios(new string[] {
            this.name +"の攻撃！\nあなたは"+ damage +"のダメージをうけた"});
        
        yield return new WaitForSeconds(0.5f);

        SoundManager.instance.PlayButtonSE(1);
        Attack(BattleManager.Player);
        BattleManager.playerUI.UpdateUI(BattleManager.Player);
        BattleManager.playerDamagePanel.DOShakePosition(0.3f, 0.5f, 20, 0, false, true);
        DialogTextManager.instance.SetScenarios(new string[] {
            this.name +"の攻撃！\nあなたは"+ damage +"のダメージをうけた"});

        yield return new WaitForSeconds(2f);
        BattleManager.EndOfEnemyTurn();
    }

    public int Attack(PlayerManager player)
    {
        int damage = player.Damage(atk);
        return damage;
    } 
    
    private int Heal(int heal)
    {
        enemy.hp += heal;

        if(enemy.hp >= enemy.maxHP)
        {
            enemy.hp = enemy.maxHP;
        }
        
        return enemy.hp;
    }
}
