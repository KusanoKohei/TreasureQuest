using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class QuestEvent : QuestManager
{
    private bool questEventEnded = false;

    QuestManager QuestManager => QuestManager.instance;
    BattleManager BattleManager => BattleManager.instance;
    PlayerManager Player => PlayerManager.instance;
    PlayerUIManager PlayerUI => PlayerUIManager.instance;
    DialogTextManager Dialog => DialogTextManager.instance;
    StageUIManager StageUI => StageUIManager.instance;
    SoundManager SoundManager => SoundManager.instance;
    SettingManager SettingManager => SettingManager.instance;

    public bool QuestEventEnded { get => questEventEnded; set => questEventEnded = value; }

    protected override void Start()
    {
        base.Start();
    }

    public void EventRandom(int currentStage, int encountTableLength)
    {
        int n = Random.Range(0, 10);
        // int n = 6;  // デバッグ用.

        // ステージ終盤かつプレイヤーの残りHPが少なければ、またはステージ終盤なのに毒状態で手当ても使ってしまったならば.
        if(((currentStage>=(encountTableLength/10)*7) && (Player.Hp <= (Player.MaxHP / 3)*2))||(Player.Poison!=null)&&QuestManager.Teated)
        {
            Debug.Log("お助けあまあまエンカ");
            n = Random.Range(0, 3);

            switch (n)
            {
                case 0:
                    StartCoroutine(NothingEvent(currentStage, encountTableLength));
                    break;

                case 1:
                case 2:
                    StartCoroutine(HealAreaEvent());
                    break;
            }
        }
        else
        {
            // 体力が低い時は少し優しめに.
            if (Player.Hp <= (Player.MaxHP / 5) * 3)
            {
                Debug.Log("体力が低い時のイベント設定");
                n = Random.Range(0, 4);

                switch (n)
                {
                    case 0:
                    case 1:
                        StartCoroutine(NothingEvent(currentStage, encountTableLength));
                        break;

                    case 2:
                        StartCoroutine(MushroomEvent());
                        break;

                    case 3:
                        StartCoroutine(HealAreaEvent());
                        break;

                    default:
                        StartCoroutine(NothingEvent(currentStage, encountTableLength));
                        break;
                }
            }
            else
            {
                switch (n)
                {
                    case 0:
                    case 1:
                    case 2:

                        StartCoroutine(NothingEvent(currentStage, encountTableLength));

                        break;

                    case 3:
                    case 4:
                    case 5:
                        if (QuestManager.trapCount < 3)
                        {
                            // 再抽選.
                            EventRandom(currentStage, encountTableLength);
                        }
                        else
                        {
                            if (Player.Level >= 5)
                            {
                                if (currentStage < 2 || currentStage <= 8)
                                {
                                    StartCoroutine(NothingEvent(currentStage, encountTableLength));
                                }
                                else
                                {
                                    StartCoroutine(TrapEvent(currentStage));
                                }
                            }
                            else
                            {
                                if (currentStage < 2 || currentStage >= 8)
                                {
                                    StartCoroutine(NothingEvent(currentStage, encountTableLength));
                                }
                                else
                                {
                                    // トラップイベント.
                                    StartCoroutine(TrapEvent(currentStage));
                                }
                            }
                        }

                        break;


                    case 6:
                    case 7:
                        // キノコイベントです.
                        StartCoroutine(MushroomEvent());

                        break;

                    case 8:
                    case 9:

                        // もしHPが4/5より大きければ.
                        if (Player.Hp >= (Player.MaxHP * 4 / 5))
                        {
                            if (Player.Poison)  // しかしながら毒状態ならば.
                            {
                                // 回復の泉イベントです.
                                StartCoroutine(HealAreaEvent());
                            }
                            else
                            {
                                EventRandom(currentStage, encountTableLength);  // 再抽選. 
                            }
                        }
                        else
                        {
                            // 回復の泉イベントです.
                            StartCoroutine(HealAreaEvent());
                        }

                        break;

                    default:
                        Debug.Log("default");
                        StartCoroutine(NothingEvent(currentStage, encountTableLength));
                        break;
                }

            }

        }
    }


    private IEnumerator NothingEvent(int currentStage, int encountTableLength)
    {
        if (currentStage >= encountTableLength - 3)
        {
            DialogTextManager.instance.SetScenarios(new string[] { "強い敵の気配がする……" });
        }
        else
        {
            DialogTextManager.instance.SetScenarios(new string[] { "お宝も敵も見当たらない" });
        }
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // 毒ダメージ演出.
        if(Player.Poison != null)
        {
            StartCoroutine(Player.Poison.PoisonDirection(Player));
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
        }

        QuestEventEnded = true;
        yield return new WaitUntil(() => QuestEventEnded);  // QuestEventEndedが真になるまで待機.

        EndQuestEvent();
    }

    // トラップイベント関数です.
    private IEnumerator TrapEvent(int currentStage)
    {
        QuestManager.trapCount = 0;

        // エンカウントをリセットする（トラップでステージを戻っても同じパターンにはまらないように).
        SetEncount();


        SoundManager.instance.PlayButtonSE(16); // ゴゴゴゴというSE.
        BattleManager.playerDamagePanel.DOShakePosition(2.0f, 1.0f, 20, 30f, false, true);

        DialogTextManager.instance.SetScenarios(new string[] { "仕掛けられた罠だ！" });
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


        DialogTextManager.instance.SetScenarios(new string[] { "大岩が転がり迫ってくる！" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);


        // 効果音を停めておく.
        SoundManager.audioSourceSE.Stop();

        // 背景画像を拡大する.それを完了後に元の大きさに戻す.
        QuestManager.questBG.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 2.0f)
            .OnComplete(() => QuestManager.questBG.transform.localScale = new Vector3(1, 1, 1));
        // 背景画像をフェードアウトさせる.完了後に元の大きさに戻す.
        SpriteRenderer questBGSpriterenderer = QuestManager.questBG.GetComponent<SpriteRenderer>();
        questBGSpriterenderer.DOFade(0, 2f)
            .OnComplete(() => questBGSpriterenderer.DOFade(1, 0));

        DialogTextManager.instance.SetScenarios(new string[] { "押しつぶされないように\n必死に逃げた！" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // 画面がクリックされるまで次の処理を待つ.
        if (!DialogTextManager.instance.IsEnd)
        {
            DialogTextManager.instance.EnableClickIcon();
        }

        DialogTextManager.instance.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        DialogTextManager.instance.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        DialogTextManager.instance.SetScenarios(new string[] { "逃げ切れたが、いくぶんか来た道を\n戻ってしまったみたいだ……" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // 画面がクリックされるまで次の処理を待つ.
        if (!DialogTextManager.instance.IsEnd)
        {
            DialogTextManager.instance.EnableClickIcon();
        }

        DialogTextManager.instance.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        DialogTextManager.instance.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;


        // ランダムで1～2ステージを後退する.
        int r = Random.Range(1, 3);
        QuestManager.ModoruStage(r);    // ステージUIの更新もここでやってくれます.

        // ちょっと待つ.
        yield return new WaitForSeconds(1.0f);

        QuestEventEnded = true;
        yield return new WaitUntil(() => QuestEventEnded);  // QuestEventEndedが真になるまで待機.

        EndQuestEvent();
    }

    // キノコイベント関数です.
    private IEnumerator MushroomEvent()
    {
        // キノコUI出現.
        StageUI.MushroomUIAppearance(true);
        SoundManager.instance.PlayButtonSE(18);  // チャージエフェクトSE.


        DialogTextManager.instance.SetScenarios(new string[] { "なんだか不思議な見た目の\nキノコを見つけた……" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // 画面がクリックされるまで次の処理を待つ.
        if (!DialogTextManager.instance.IsEnd)
        {
            DialogTextManager.instance.EnableClickIcon();
        }

        DialogTextManager.instance.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        DialogTextManager.instance.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;

        DialogTextManager.instance.SetScenarios(new string[] { "食べてみますか？" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // 選択肢が現れ、選択するまで待機する.
        StageUI.ButtonUIAppearance(false);
        StageUI.YesNoButtonAppearance(true);

        yield return new WaitUntil(() => QuestManager.Selected);

        StageUI.YesNoButtonAppearance(false);
        StageUI.MushroomUIAppearance(false);    // キノコが消える.


        if (QuestManager.Selection == 1)
        {
            int n = Random.Range(0, 3);
            // int n = 1;  // デバッグ用;
            if (n == 0)
            {
                // 毒.
                if (!Player.Poison)
                {
                    Player.Poison = Player.gameObject.AddComponent<PoisonStatus>();
                    yield return new WaitForSeconds(SettingManager.MessageSpeed);
                }
                else
                {
                    DialogTextManager.instance.SetScenarios(new string[] { "別にうまくも まずくもなかった" });
                    yield return new WaitForSeconds(SettingManager.MessageSpeed);
                }
            }
            else
            {
                if (Player.Poison)
                {
                    DialogTextManager.instance.SetScenarios(new string[] { "毒消しキノコだったようだ" });
                    yield return new WaitForSeconds(SettingManager.MessageSpeed);

                    // 画面がクリックされるまで次の処理を待つ.
                    if (!DialogTextManager.instance.IsEnd)
                    {
                        DialogTextManager.instance.EnableClickIcon();
                    }

                    DialogTextManager.instance.ClickIconEnableAppear = true;
                    yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
                    DialogTextManager.instance.ClickIconEnableAppear = false;
                    DialogTextManager.instance.clickImage.enabled = false;

                    StageUI.YesNoButtonAppearance(true);
                    yield return new WaitUntil(() => QuestManager.Selected);
                    StageUI.YesNoButtonAppearance(false);

                    // 毒状態を治す.
                    StartCoroutine(Player.Poison.PoisonRefresh());
                }
                else
                {
                    if (!Player.BuffStatus)
                    {
                        DialogTextManager.instance.SetScenarios(new string[] { "キノコを食べたら体に力がみなぎってきた！" });
                        Player.BuffStatus = Player.gameObject.AddComponent<BuffStatus>();
                        yield return new WaitForSeconds(SettingManager.MessageSpeed);
                    }
                    else
                    {
                        if (Player.Poison)
                        {
                            DialogTextManager.instance.SetScenarios(new string[] { "毒消しキノコだったようだ" });
                            yield return new WaitForSeconds(SettingManager.MessageSpeed);

                            // 画面がクリックされるまで次の処理を待つ.
                            if (!DialogTextManager.instance.IsEnd)
                            {
                                DialogTextManager.instance.EnableClickIcon();
                            }

                            DialogTextManager.instance.ClickIconEnableAppear = true;
                            yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
                            DialogTextManager.instance.ClickIconEnableAppear = false;
                            DialogTextManager.instance.clickImage.enabled = false;

                            StageUI.YesNoButtonAppearance(true);
                            yield return new WaitUntil(() => QuestManager.Selected);
                            StageUI.YesNoButtonAppearance(false);

                            // 毒状態を治す.
                            StartCoroutine(Player.Poison.PoisonRefresh());
                        }
                        else
                        {
                            DialogTextManager.instance.SetScenarios(new string[] { "別にうまくも まずくもなかった" });
                            yield return new WaitForSeconds(SettingManager.MessageSpeed);
                        }
                    }
                }
            }
        }
        else if (QuestManager.Selection == 2)
        {
            DialogTextManager.instance.SetScenarios(new string[] { "キノコを無視して\nあなたは去った" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
        }


        QuestEventEnded = true;
        yield return new WaitUntil(() => QuestEventEnded);  // QuestEventEndedが真になるまで待機.

        EndQuestEvent();
    }

    // 回復の泉関数です.
    private IEnumerator HealAreaEvent()
    {
        SoundManager.instance.PlayButtonSE(17); // チャポンというSE.
        DialogTextManager.instance.SetScenarios(new string[] { "不思議な光を放つ泉が\nこんこんとわき出ている" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed*2);

        DialogTextManager.instance.SetScenarios(new string[] { "少し休んでいきますか？" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // 画面がクリックされるまで次の処理を待つ.
        if (!DialogTextManager.instance.IsEnd)
        {
            DialogTextManager.instance.EnableClickIcon();
        }

        DialogTextManager.instance.ClickIconEnableAppear = true;
        yield return new WaitUntil(() => DialogTextManager.instance.IsEnd);
        DialogTextManager.instance.ClickIconEnableAppear = false;
        DialogTextManager.instance.clickImage.enabled = false;

        StageUI.YesNoButtonAppearance(true);
        yield return new WaitUntil(() => QuestManager.Selected);
        StageUI.YesNoButtonAppearance(false);


        if (QuestManager.Selection == 1)
        {
            Teated = true;
            StartCoroutine(HealDirecting());
        }
        else if (QuestManager.Selection == 2)
        {
            DialogTextManager.instance.SetScenarios(new string[] { "あなたは泉から去った" });
        }

        yield return new WaitForSeconds(SettingManager.MessageSpeed);


        QuestEventEnded = true;
        yield return new WaitUntil(() => QuestEventEnded);  // QuestEventEndedが真になるまで待機.

        EndQuestEvent();
    }

    private void EndQuestEvent()
    {
        QuestManager.Selection = 0;
        QuestManager.Selected = false;
        QuestEventEnded = false;

        // ステージUIの出現。入力待ち.
        StageUI.YesNoButtonAppearance(false);
        StageUI.ButtonUIAppearance(true);
    }
}
