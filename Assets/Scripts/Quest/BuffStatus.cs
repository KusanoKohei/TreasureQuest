using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffStatus : MonoBehaviour
{
    private GameObject buffEffect;
    private ParticleSystem buffParticle;

    private int buffAtk = 5;
    private int buffSpd = 5;

    PlayerManager Player = PlayerManager.instance;

    public int BuffAtk { get => buffAtk; set => buffAtk = value; }
    public int BuffSpd { get => buffSpd; set => buffSpd = value; }
    
    private void Awake()
    {
        // バフエフェクト発生.
        buffEffect = Resources.Load<GameObject>("PwrEffect");
        buffEffect.transform.localPosition = new Vector3(0, -2, 0);
        buffEffect.transform.localScale = new Vector3(5, 5, 0);
        Instantiate(buffEffect, Player.transform, false);

        StartCoroutine(BuffAwake());
    }
    public IEnumerator BuffAwake()
    {
        SoundManager.instance.PlayButtonSE(6);  // チャージエフェクトSE.

        DialogTextManager.instance.SetScenarios(new string[] { "あなたの能力が一時的に強化された" });
        
        // エフェクトの静まり待ち.
        yield return new WaitForSeconds(2.0f);
    }
}
