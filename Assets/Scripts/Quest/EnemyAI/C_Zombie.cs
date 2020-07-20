using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class C_Zombie : EnemyManager
{
    EnemyManager enemy;

    public GameObject poisonEffect;

    private BattleManager BattleManager => BattleManager.instance;
    private DialogTextManager Dialog => DialogTextManager.instance;

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
        enemy.AllowedAction = true;

        enemy.exp = this.exp;

        // -------------------------------------------------------------- //

        enemyUI.SetupUI(enemy);
    }

    public void SelectAction()
    {
        int r;
        r = Random.Range(0, 7);

        if ((r >= 5) && (r < 7))
        {
            StartCoroutine(ZombieConfused());
        }
        else
        {
            StartCoroutine(ZombieCommonAttack());
        }
    }

    public IEnumerator ZombieCommonAttack()
    {
        StartCoroutine(enemy.Attacks());
        yield return new WaitWhile(() => enemy.NowActive);

        if (enemy.Hitted && (!Player.Poison))   // 攻撃が当たったときかつ、毒状態ではない時にのみ毒状態にかかるかの判定.
        {
            PoisonProbability();
            // HittedはBattleManagerで初期化する.
        }

        // 毒状態にかかった演出が終了するまで待機させる.
        yield return new WaitWhile(() => BattleManager.PoisonDirecting);
        BattleManager.EndOfEnemyTurn();
    }

    public void PoisonProbability()
    {
        if (enemy.Hitted)   // 攻撃が当たっていなければ呼ばない.
        {
            int r;
            r = Random.Range(0, 10);
            if (r > 4)
            {
                Player.Poison = Player.gameObject.AddComponent<PoisonStatus>();
                Debug.Log(Player.Poison);
            }
            else
            {

            }
        }        
    }

    public IEnumerator ZombieConfused()
    {
        DialogTextManager.instance.SetScenarios(new string[] {this.name + "は ただ\nウアウアとうなっている……" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        enemy.IsTurned = true;
        BattleManager.EndOfEnemyTurn();
    }
}
