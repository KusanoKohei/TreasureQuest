﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectionStatus : MonoBehaviour
{
    private EnemyManager enemy;
    private BattleManager battleManager;

    DialogTextManager Dialog => DialogTextManager.instance;

    private void Awake()
    {
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        enemy = battleManager.Enemy.GetComponent<EnemyManager>();
    }

    public IEnumerator ProtectionDirecting()
    {
        enemy.NowProtection = true;

        // 空振り音(SE).
        SoundManager.instance.PlayButtonSE(3);
        
        DialogTextManager.instance.SetScenarios(new string[] { "あなたの攻撃" });
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


        // reflection(SE).
        SoundManager.instance.PlayButtonSE(11);

        // 防御エフェクト.
        GameObject defenceEffect = Resources.Load<GameObject>("DefenceEffect");
        defenceEffect.transform.Translate(0, 0, 0);
        defenceEffect.transform.localScale = new Vector3(30, 30, 0);
        Instantiate(defenceEffect, enemy.transform, false);

        DialogTextManager.instance.SetScenarios(new string[] { "しかし氷の壁にはばまれた！" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        enemy.Protection = null;
        Destroy(GetComponent<ProtectionStatus>());  // プロテクションコンポーネントを削除.

        // breaking_a_glass(SE).
        SoundManager.instance.PlayButtonSE(12);

        DialogTextManager.instance.SetScenarios(new string[] { "氷の壁はくずれさった……" });
       

        // 画面がクリックされるまで次の処理を待つ.
        if (!Dialog.IsEnd)
        {
            Dialog.EnableClickIcon();
        }

        Dialog.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        Dialog.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        enemy.NowProtection = false;
    }
}
