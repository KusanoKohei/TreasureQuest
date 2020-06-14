using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// クエストの進行を管理する.
public class QuestManager : MonoBehaviour
{
    public StageUIManager stageUI;
    public GameObject[] enemyPrefab;
    public GameObject[] bossPrefab;
    public BattleManager battleManager;
    public SceneTransitionManager sceneTransitionManager;
    public GameObject questBG;
    
    private GameObject dialogWindow;
    private PlayerUIManager playerUI;
    private GameObject playerUIPanel;
    private FadeIOManager fadeManager;

    public int questNumber = 1; // クエストの種類.
    public int MAX_STAGE;       // ステージ数.
    int[] encountTable;         // ステージごとのエンカウント設定.
    int currentStage = 0;       // 現在のステージの進行度.
    private int selection=0;
    private bool selected = false;
    public bool teated = false;


    UserData Userdata => SaveSystem.instance.UserData;
    PlayerManager Player => PlayerManager.instance;
    SettingManager SettingManager => SettingManager.instance;
    DialogTextManager Dialog => DialogTextManager.instance;


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
            Destroy(this.gameObject);
        }
    }
    #endregion

    private void Start()
    {
        dialogWindow = GameObject.Find("DialogUI");
        playerUI = GameObject.Find("PlayerUICanvas").GetComponent<PlayerUIManager>();
        playerUIPanel = GameObject.Find("PlayerUIPanel");

        playerUI.SetupUI(Player);
        playerUI.UpdateSpcUI(Player);
        playerUIPanel.transform.localPosition = new Vector3(0, -500, 0);
        playerUI.SwitchActivateButton(false);    // falseがデフォルト.   

        DialogTextManager.instance.SetScenarios(new string[] { "ダンジョンにたどりついた" });
        stageUI.updateUI(currentStage);

        SetEncount();   // エンカウント率を設定する.
    }

    public void SetEncount()
    {
        // ステージ数を設定.
        encountTable = new int[MAX_STAGE]; 

        for (int i=0; i<encountTable.Length; i++)
        {
            int n = Random.Range(-2,2);
            // int n = -2;  // デバッグ用;
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


    IEnumerator Searching()
    {
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

        currentStage++;
        stageUI.updateUI(currentStage);


        if(currentStage == encountTable.Length)     // ダンジョンの一番奥では必ずボス戦を.
        {
            EncountBoss();
        }
        else if(currentStage < encountTable.Length)
        {
            if (encountTable[currentStage] >= 0)             // -1,-2であればエンカウントしない.
            {
                EncountEnemy(encountTable[currentStage]);
            }
            else
            {
                stageUI.ButtonUIAppearance(true);

                if (currentStage >= encountTable.Length - 3)
                {
                    DialogTextManager.instance.SetScenarios(new string[] { "強い敵の気配がする……" });
                }
                else
                {
                    DialogTextManager.instance.SetScenarios(new string[] { "お宝も敵も見当たらない" });
                }
            }
        } 
    }

    void EncountBoss()
    {
        GameObject obj = Instantiate(bossPrefab[questNumber - 1]);
        EnemyManager enemy = obj.GetComponent<EnemyManager>();
        battleManager.Setup(enemy);         // 敵のUIの表示と、プレイヤーのタッチに反応するようにする.
        stageUI.ButtonUIAppearance(false);  // ステージのUIを切る.
        
        // バトルの開始のダイアログ.
        battleManager.SwitchBattleOpening(enemy);

        // 以後はプレイヤーが敵のUIをタッチすることでゲームが進行する.
    }

    void EncountEnemy(int encountTableCurrentStage)
    {
        GameObject obj= Instantiate(enemyPrefab[encountTableCurrentStage]);
        EnemyManager enemy = obj.GetComponent<EnemyManager>();
        battleManager.Setup(enemy);         // 敵のUIの表示と、プレイヤーのタッチに反応するようにする.
        stageUI.ButtonUIAppearance(false);  // ステージのUIを切る.

        // バトルの開始のダイアログ.
        battleManager.SwitchBattleOpening(enemy);
    }


    public void OnNextButton()
    {
       
        SoundManager.instance.PlayButtonSE(0);       // ボタンSEを鳴らす.

        // ボタン非表示.
        stageUI.ButtonUIAppearance(false);

        StartCoroutine(Searching());

    }

    public void OnTapToTownButton()
    {
        SoundManager.instance.PlayButtonSE(0);

        StartCoroutine(ToTownSelection());
    }

    private IEnumerator ToTownSelection()
    {
        selected = false;

        // 進むなどのステージUIを消しておく.
        stageUI.ButtonUIAppearance(false);

        DialogTextManager.instance.SetScenarios(new string[] { "街に戻るとクエストは\nやりなおしになります" });
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


        DialogTextManager.instance.SetScenarios(new string[] { "本当に街に戻りますか？" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        stageUI.YesNoButtonAppearance(true);

        yield return new WaitUntil(() => selected);

        if (selection == 1)
        {
            SoundManager.instance.PlayButtonSE(0);

            selection = 0;
            teated = true;
            SceneTransitionManager.instance.LoadTo("Town");
        }
        else if (selection == 2)
        {
            SoundManager.instance.PlayButtonSE(0);

            selection = 0;

            // 選択肢ボタンを消し、ステージを進行させるボタンを表示させる.
            stageUI.YesNoButtonAppearance(false);
            stageUI.ButtonUIAppearance(true);
            DialogTextManager.instance.SetScenarios(new string[] { "" });
        }

    }

    public void OnTeateButton()
    {
        StartCoroutine(TeateSelectDirecting());
    }

    private IEnumerator TeateSelectDirecting() 
    {
        selected = false;

        // 進むなどのステージUIを消しておく.
        stageUI.ButtonUIAppearance(false);

        DialogTextManager.instance.SetScenarios(new string[] { "体力を1/3回復させます" });
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


        DialogTextManager.instance.SetScenarios(new string[] { "今回のクエストで一度しか使えません\n実行しますか？" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        stageUI.YesNoButtonAppearance(true);

        yield return new WaitUntil(() => selected);

        if (selection==1)
        {
            selection = 0;
            teated = true;
            StartCoroutine(HealDirecting());
        }
        else if (selection==2)
        {
            selection = 0;

            // 選択肢ボタンを消し、ステージを進行させるボタンを表示させる.
            stageUI.YesNoButtonAppearance(false);
            stageUI.ButtonUIAppearance(true);
            DialogTextManager.instance.SetScenarios(new string[] { "" });
        }
    }

    public void OnClickYesButton()
    {
        selection = 1;
        selected = true;
    }

    public void OnClickNoButton()
    {
        Debug.Log("no");
        selection = 2;
        selected = true;
    }

    private IEnumerator HealDirecting()
    {
        DialogTextManager.instance.SetScenarios(new string[] { "あなたはケガの手当てをした" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);
        
        // healEffect(SE).
        SoundManager.instance.PlayButtonSE(5);

        int healPoint = (int)Player.MaxHP / 3;
        Player.HpAdd(healPoint);
        playerUI.UpdateUI(Player);
        DialogTextManager.instance.SetScenarios(new string[] { "体力が回復した" });
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed);

        // 選択肢ボタンを消し、ステージを進行させるボタンを表示させる.
        stageUI.YesNoButtonAppearance(false);
        stageUI.ButtonUIAppearance(true);
    }

    public void ReturnToQuest()
    {
        stageUI.ButtonUIAppearance(true);
        SoundManager.instance.PlayBGM("Quest");
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2.0f);

        Player.Dead = true;  // ゲームオーバーになったフラグ.(TownManagerで使う).

        DialogTextManager.instance.SetScenarios(new string[]{
            "あなたは負けて、街へと引き返した……"
        });

        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed + 1.0f);
        sceneTransitionManager.LoadTo("Town");
    }

    public void GameClear()
    {
        StartCoroutine(GameClearDirecting());
    }

    private IEnumerator GameClearDirecting() 
    {
        yield return new WaitForSeconds(SettingManager.instance.MessageSpeed/3*2);

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

        stageUI.ClearUIAppearance();        // 宝箱を表示、他のステージUIは切っておく.

        DialogTextManager.instance.SetScenarios(new string[] { "宝物を見つけた！" });
        yield return new WaitForSeconds(4.0f);

        // フェードアウト.
        FadeIOManager.instance.FadeOut();

        SoundManager.instance.PlayBGM("Title");   // BGM.

        DialogTextManager.instance.SetScenarios(new string[] { "トレジャーハンターのあなたは\n街へ戻ると" });
        yield return new WaitForSeconds(2.0f);

        DialogTextManager.instance.SetScenarios(new string[] { "次のお宝を求めて旅だった……" });
        yield return new WaitForSeconds(4.0f);


        DialogTextManager.instance.SetScenarios(new string[] { "GAME CLEAR !! \n Thank you for Playing" });
        yield return new WaitForSeconds(4.0f);

        Player.UndoParameter();      // レベルに合わせて初期化.

        Player.PlayerInitPerBattleEnd();    // バトル終了ごとの初期化処理.

        /*
        // ---- セーブ.
        Userdata.level = Player.Level;
        Userdata.maxHP = Player.MaxHP;
        Userdata.hp = Player.Hp;
        Userdata.atk = Player.Atk;
        Userdata.spd = Player.Spd;
        Userdata.dodge = Player.Dodge;
        Userdata.critical = Player.Critical;
        Userdata.skill = Player.Skill;
        Userdata.nextEXP = Player.NextEXP;
        Userdata.nowEXP = Player.NowEXP;

        Userdata.messageSpeed = SettingManager.instance.MessageSpeed;
        Userdata.BGMvolume = SoundManager.instance.audioSourceBGM.volume;
        Userdata.SEvolume = SoundManager.instance.audioSourceSE.volume;

        SaveSystem.Instance.Save();

        // -----------

        DialogTextManager.instance.SetScenarios(new string[] { "クリアデータをセーブしました" });
        yield return new WaitForSeconds(2.0f);

        DialogTextManager.instance.SetScenarios(new string[] { "タイトルの『つづきから』で\n現状の強さから再開できます" });
        yield return new WaitForSeconds(4.0f);

        */

        DialogTextManager.instance.SetScenarios(new string[] { "今後もこのゲームは\n様々な機能を実装する予定です" });
        yield return new WaitForSeconds(2.0f);

        DialogTextManager.instance.SetScenarios(new string[] { "またクエストに\n挑戦しにきてください！" });
        yield return new WaitForSeconds(4.0f);

        CanvasGroup dialogCanvas = dialogWindow.GetComponent<CanvasGroup>();
        dialogCanvas.ignoreParentGroups = false;

        SoundManager.instance.StopBGM();    // BGMを停止させておく.

        sceneTransitionManager.LoadTo("Title");
    }

}
