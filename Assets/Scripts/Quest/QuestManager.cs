using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// クエストの進行を管理する.
public class QuestManager : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    public GameObject[] bossPrefab;
    public BattleManager battleManager;
    public SceneTransitionManager sceneTransitionManager;
    public GameObject questBG;
    
    private GameObject dialogWindow;
    public GameObject playerUIPanel;
    private FadeIOManager fadeManager;

    public int questNumber = 1; // クエストの種類.
    public int MAX_STAGE;       // ステージ数.
    int[] encountTable;         // ステージごとのエンカウント設定.
    int currentStage = 0;       // 現在のステージの進行度.
    private int selection=0;
    private bool selected = false;
    private bool teated = false;
    public int trapCount = 3;


    UserData Userdata => SaveSystem.instance.UserData;
    PlayerManager Player => PlayerManager.instance;
    SettingManager SettingManager => SettingManager.instance;
    DialogTextManager Dialog => DialogTextManager.instance;
    StageUIManager StageUI => StageUIManager.instance;


    public int Selection { get => selection; set => selection = value; }
    public bool Selected { get => selected; set => selected = value; }
    public bool Teated { get => teated; set => teated = value; }


    #region Singleton
    public static QuestManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            // Destroy(this.gameObject);
        }
    }
    #endregion

    PlayerUIManager PlayerUI => PlayerUIManager.instance;


    protected virtual void Start()
    {
        Debug.Log(Player.Dodge);

        dialogWindow = GameObject.Find("DialogUI");

        PlayerUI.SetupUI(Player);
        PlayerUI.UpdateSpcUI(Player);
        playerUIPanel.transform.localPosition = new Vector3(0, -480, 0);
        PlayerUI.SwitchActivateButton(false);    // falseがデフォルト.   

        DialogTextManager.instance.SetScenarios(new string[] { "ダンジョンにたどりついた" });
        StageUI.UpdateUI(currentStage);

        SetEncount();   // エンカウント率を設定する.
    }

    public void SetEncount()
    {
        // ステージ数を設定.
        encountTable = new int[MAX_STAGE]; 

        for (int i=0; i<encountTable.Length; i++)
        {
            int n = Random.Range(-2,2);
            // int n = -1;  // デバッグ用;
            encountTable[i] = n;

            if (Player.Level == 1 && encountTable[i] == 0)  // レベル１では強敵に出くわさない.
            {
                encountTable[i] = 1;
            }
            else if (Player.Level >= 5 && encountTable[i] == 1) // レベル５以上の時、弱い敵には出くわさないようにする.
            {
                encountTable[i] = -1;
            }
            else if (Player.Level == 4 && encountTable[i] == 1)    // レベル４の時、弱い敵は一定確率で強敵に変える.
            {
                int r = Random.Range(0, 2);
                if (r == 0)
                {
                    encountTable[i] = 0;
                }
                else
                {
                    encountTable[i] = -1;
                }
                
            }
            else if(Player.Level == 2 && encountTable[i] == 0)      // レベル２の時は強い敵を1/2の確率で弱い敵に差し替える.
            {
                int r = Random.Range(0, 2);
                if (r == 0)
                {
                    encountTable[i] = 1;
                }
            }
        }
    }

    public int ModoruStage(int modoruStage)
    {
        currentStage = currentStage - modoruStage;
        StageUI.UpdateUI(currentStage);

        return currentStage;
    }


    IEnumerator Searching()
    {
        trapCount++;

        DialogTextManager.instance.SetScenarios(new string[] { "探索中..." });
        // 背景画像を拡大する.それを完了後に元の大きさに戻す.
        questBG.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 2.0f)
            .OnComplete(() => questBG.transform.localScale = new Vector3(1, 1, 1));
        // 背景画像をフェードアウトさせる.完了後に元の大きさに戻す.
        SpriteRenderer questBGSpriterenderer = questBG.GetComponent<SpriteRenderer>();
        questBGSpriterenderer.DOFade(0, 2f)
            .OnComplete(() => questBGSpriterenderer.DOFade(1, 0));

        // 時間遅延.
        yield return new WaitForSeconds(2.0f);

        currentStage++;
        StageUI.UpdateUI(currentStage);

        if(currentStage == encountTable.Length)     // ダンジョンの一番奥では必ずボス戦を.
        {
            // ボスとのエンカウント.
            EncountBoss();
        }
        else if(currentStage < encountTable.Length)
        {
            if (encountTable[currentStage] < 0)
            {
                QuestEvent questEvent = this.GetComponent<QuestEvent>();
                questEvent.EventRandom(currentStage, encountTable.Length);
            }
            else
            {
                // 敵とのエンカウント.
                EncountEnemy(encountTable[currentStage]);
            }
        } 
    }

    void EncountBoss()
    {
        GameObject obj = Instantiate(bossPrefab[questNumber - 1]);
        EnemyManager enemy = obj.GetComponent<EnemyManager>();
        battleManager.Setup(enemy);         // 敵のUIの表示と、プレイヤーのタッチに反応するようにする.
        StageUI.ButtonUIAppearance(false);  // ステージのUIを切る.
        
        // バトルの開始のダイアログ.
        battleManager.SwitchBattleOpening(enemy);

        // 以後はプレイヤーが敵のUIをタッチすることでゲームが進行する.
    }

    void EncountEnemy(int encountTableCurrentStage)
    {
        GameObject obj= Instantiate(enemyPrefab[encountTableCurrentStage]);
        EnemyManager enemy = obj.GetComponent<EnemyManager>();
        battleManager.Setup(enemy);         // 敵のUIの表示と、プレイヤーのタッチに反応するようにする.
        StageUI.ButtonUIAppearance(false);  // ステージのUIを切る.

        // バトルの開始のダイアログ.
        battleManager.SwitchBattleOpening(enemy);
    }


    public void OnNextButton()
    {
       
        SoundManager.instance.PlayButtonSE(0);       // ボタンSEを鳴らす.

        // ボタン非表示.
        StageUI.ButtonUIAppearance(false);

        StartCoroutine(Searching());

    }

    public void OnTapToTownButton()
    {
        SoundManager.instance.PlayButtonSE(0);

        StartCoroutine(ToTownSelection());
    }

    private IEnumerator ToTownSelection()
    {
        Selected = false;

        // 進むなどのステージUIを消しておく.
        StageUI.ButtonUIAppearance(false);

        DialogTextManager.instance.SetScenarios(new string[] { "街に戻ると体力は全快しますが\nクエストはやり直しになります" });
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


        DialogTextManager.instance.SetScenarios(new string[] { "本当に街に戻りますか？" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        StageUI.YesNoButtonAppearance(true);

        yield return new WaitUntil(() => Selected);

        if (Selection == 1)
        {
            Selection = 0;

            SoundManager.instance.PlayButtonSE(0);

            BattleManager.instance.InitPlayerBuffer();     // バフを消しておく.

            // 毒を無効化.
            if (Player.Poison != null)
            {
                StartCoroutine(Player.Poison.PoisonRefresh());
            }

            Teated = true;
            SceneTransitionManager.instance.LoadTo("Town");
        }
        else if (Selection == 2)
        {
            SoundManager.instance.PlayButtonSE(0);

            Selection = 0;

            // 選択肢ボタンを消し、ステージを進行させるボタンを表示させる.
            StageUI.YesNoButtonAppearance(false);
            StageUI.ButtonUIAppearance(true);
            DialogTextManager.instance.SetScenarios(new string[] { "街へ帰るのを思いとどまった" });
        }

    }

    public void OnTeateButton()
    {
        StartCoroutine(TeateSelectDirecting());
    }

    private IEnumerator TeateSelectDirecting() 
    {
        Selected = false;

        // 進むなどのステージUIを消しておく.
        StageUI.ButtonUIAppearance(false);

        DialogTextManager.instance.SetScenarios(new string[] { "体力を1/3回復させます" });
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


        DialogTextManager.instance.SetScenarios(new string[] { "クエストごとに一度しか使えません\n実行しますか？" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        StageUI.YesNoButtonAppearance(true);

        yield return new WaitUntil(() => Selected);

        if (Selection==1)
        {
            // 選択肢ボタンを消す.
            StageUI.YesNoButtonAppearance(false);
            Selection = 0;
            Teated = true;

            DialogTextManager.instance.SetScenarios(new string[] { "あなたはケガの手当てをした" });
            yield return new WaitForSeconds(SettingManager.MessageSpeed);

            StartCoroutine(HealDirecting());
            yield return new WaitForSeconds(SettingManager.MessageSpeed);
 
            StageUI.ButtonUIAppearance(true);
        }
        else if (Selection==2)
        {
            Selection = 0;

            // 選択肢ボタンを消し、ステージを進行させるボタンを表示させる.
            StageUI.YesNoButtonAppearance(false);
            StageUI.ButtonUIAppearance(true);
            DialogTextManager.instance.SetScenarios(new string[] { "ケガの手当てをやめた" });
        }
    }

    public void OnClickYesButton()
    {
        Selection = 1;
        Selected = true;
    }

    public void OnClickNoButton()
    {
        Selection = 2;
        Selected = true;
    }

    public IEnumerator HealDirecting()
    {
        // healEffect(SE).
        SoundManager.instance.PlayButtonSE(5);
        // 回復エフェクト/
        GameObject healEffect = Resources.Load<GameObject>("HealEffect");
        healEffect.transform.Translate(0, 1, 1);
        healEffect.transform.localScale = new Vector3(5, 5, 0);
        Instantiate(healEffect, new Vector3(0, 0, 0), Quaternion.identity);


        int healPoint = (int)Player.MaxHP / 3;
        Player.HpAdd(healPoint);
        PlayerUI.UpdateUI(Player);
        DialogTextManager.instance.SetScenarios(new string[] { "体力が回復した" });
        yield return new WaitForSeconds(SettingManager.MessageSpeed);

        // 毒を消しておく.
        if(Player.Poison != null)
        {
            StartCoroutine(Player.Poison.PoisonRefresh());
        }
    }

    public void ReturnToQuest()
    {
        StageUI.ButtonUIAppearance(true);
        SoundManager.instance.PlayBGM("Quest");
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2.0f);

        Player.Dead = true;  // ゲームオーバーになったフラグ.(TownManagerで使う).

        BattleManager.instance.InitPlayerBuffer();     // バフを消しておく.

        DialogTextManager.instance.SetScenarios(new string[]{
            "あなたは負けて、街へと引き返した……"
        });

        yield return new WaitForSeconds(SettingManager.MessageSpeed + 1.0f);
        sceneTransitionManager.LoadTo("Town");
    }

    public void GameClear()
    {
        // ゲームクリアフラグ.
        Userdata.isCleared = true;

        if(Player.BuffStatus != null)
        {
            BattleManager.instance.InitPlayerBuffer();
        }

        if(Player.Poison != null)
        {
            Destroy(Player.Poison.GetComponent<PoisonStatus>());
        }


        StartCoroutine(GameClearDirecting());
    }

    private IEnumerator GameClearDirecting() 
    {
        yield return new WaitForSeconds(SettingManager.MessageSpeed/3*2);

        DialogTextManager.instance.SetScenarios(new string[] { "探索中..." });
        // 背景画像を拡大する.それを完了後に元の大きさに戻す.
        questBG.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 2.0f)
            .OnComplete(() => questBG.transform.localScale = new Vector3(1, 1, 1));
        // 背景画像をフェードアウトさせる.完了後に元の大きさに戻す.
        SpriteRenderer questBGSpriterenderer = questBG.GetComponent<SpriteRenderer>();
        questBGSpriterenderer.DOFade(0, 2f)
            .OnComplete(() => questBGSpriterenderer.DOFade(1, 0));

        // 時間遅延.
        yield return new WaitForSeconds(2f);


        SoundManager.instance.StopBGM();    // BGMを停止させておく.
        SoundManager.instance.PlayButtonSE(13);

        StageUI.ClearUIAppearance();        // 宝箱を表示、他のステージUIは切っておく.

        DialogTextManager.instance.SetScenarios(new string[] { "宝物を見つけた！" });
        yield return new WaitForSeconds(4.0f);

        // フェードアウト.
        FadeIOManager.instance.FadeOut();

        SoundManager.instance.PlayBGM("Title");   // BGM.

        DialogTextManager.instance.SetScenarios(new string[] { "トレジャーハンターのあなたは\n街へ戻ると" });
        yield return new WaitForSeconds(3.0f);

        DialogTextManager.instance.SetScenarios(new string[] { "次のお宝を求めて旅立った……" });
        yield return new WaitForSeconds(3.0f);


        DialogTextManager.instance.SetScenarios(new string[] { "GAME CLEAR !! \n Thank you for Playing" });
        yield return new WaitForSeconds(4.0f);

        Player.UndoParameter();      // レベルに合わせて初期化.

        Player.PlayerInitPerBattleEnd();    // バトル終了ごとの初期化処理.

        // ---- セーブ.

        SaveSystem.instance.Save();

        // -----------

        DialogTextManager.instance.SetScenarios(new string[] { "クリアデータをセーブしました" });
        yield return new WaitForSeconds(3.0f);

        DialogTextManager.instance.SetScenarios(new string[] { "次回のゲームは現状の強さのまま\n再開できます" });
        yield return new WaitForSeconds(3.0f);


        DialogTextManager.instance.SetScenarios(new string[] { "『ニューゲーム』ボタンが\nタイトル画面に出現しました" });
        yield return new WaitForSeconds(3.0f);

        DialogTextManager.instance.SetScenarios(new string[] { "『ニューゲーム』では最初から\n冒険をやり直すことができます" });
        yield return new WaitForSeconds(4.0f);

        DialogTextManager.instance.SetScenarios(new string[] { "またクエストにチャレンジしにきてください！" });
        yield return new WaitForSeconds(4.0f);

        CanvasGroup dialogCanvas = dialogWindow.GetComponent<CanvasGroup>();
        dialogCanvas.ignoreParentGroups = false;

        SoundManager.instance.StopBGM();    // BGMを停止させておく.

        sceneTransitionManager.LoadTo("Title");
    }

}
