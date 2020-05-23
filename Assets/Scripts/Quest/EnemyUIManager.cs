using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIManager : MonoBehaviour
{
    public Text nameText;
    public Text hpText;

    public GameObject nameObj;
    public GameObject hpObj;
    public GameObject hpGage;

    private GameObject enemyObj;

    // public GameObject Enemy;

    public static EnemyUIManager instance;

    public void Awake()
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


    public void SetupUI(EnemyManager enemy)
    {
        GameObject obj = enemy.gameObject;

        // HPゲージの更新.
        HPGagePosition(obj);
        hpGage.GetComponent<HPGage>().SetHPBar(obj);

        nameText.text = string.Format(enemy.name);
        hpText.text = string.Format("HP : {0}", enemy.hp);
    }

    private void HPGagePosition(GameObject obj)
    {
        // 名前、HPゲージの位置を調整.
        Vector3 pos = obj.transform.position;

        switch (obj.name)
        {
            case "ばけねこ":
                // nameObj.transform.localPosition = new Vector3(pos.x + 200, pos.y + 450, pos.z);
                hpGage.transform.localPosition = new Vector3(pos.x, pos.y + 400, pos.z);
                break;

            case "クレイジーゾンビ":
                hpGage.transform.localPosition = new Vector3(pos.x, pos.y + 400, pos.z);
                break;

            case "秘宝をまもるマジン":
                hpGage.transform.localPosition = new Vector3(pos.x, pos.y + 400, pos.z);
                break;

            default:
                hpGage.transform.localPosition = new Vector3(pos.x, pos.y + 400, pos.z);
                break;
        }
    }

    public void UpdateUI(EnemyManager enemy)
    {
        GameObject obj = enemy.gameObject;
        // HPゲージの更新.
        hpGage.GetComponent<HPGage>().SetHPBar(obj);

        hpText.text = string.Format("HP : {0}", enemy.hp);
    }
}