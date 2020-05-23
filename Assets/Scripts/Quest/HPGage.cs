using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPGage : MonoBehaviour
{
    private Slider slider;

    private float currentValue;
    private float maxValue;
    private float coValue;

    PlayerManager Player = PlayerManager.instance;

    public Image sliderImage;


    // Start is called before the first frame update
    void Start()
    {
        // obj = transform.root.gameObject;
        slider = this.gameObject.GetComponent<Slider>();
    }


    public void SetHPBar(GameObject obj)
    {
        switch (obj.tag)
        {
            case "Player":

                currentValue = Player.Hp;
                maxValue = Player.MaxHP;

                slider.value = currentValue;

                slider.maxValue = maxValue;

                
                sliderImage.GetComponent<Image>().fillAmount = currentValue / maxValue;

                GageChangeColor();

                break;

            case "Enemy":
            case "Boss":

                EnemyManager Enemy = obj.GetComponent<EnemyManager>();

                currentValue = Enemy.hp;
                maxValue = Enemy.maxHP;

                slider.value = currentValue;
                slider.maxValue = maxValue;

                sliderImage.GetComponent<Image>().fillAmount = currentValue / maxValue;
                
                GageChangeColor();

                break;

            default:
                Debug.Log("いずれにも属していない");
                break;
        }
    }
        
    public void UpdateHPBar(GameObject obj) 
    {
        switch (obj.tag)
        {
            case "Player":

                currentValue = Player.Hp;
                
                slider.value = currentValue;

                sliderImage.GetComponent<Image>().fillAmount = currentValue / maxValue;

                GageChangeColor();

                break;

            case "Enemy":
            case "Boss":

                EnemyManager Enemy = obj.GetComponent<EnemyManager>();

                currentValue = Enemy.hp;

                sliderImage.GetComponent<Image>().fillAmount = currentValue / maxValue;

                GageChangeColor();

                break;

            default:
                Debug.Log("いずれにも属していない");
                break;
        }
    }


    private void GageChangeColor()
    {
        if (currentValue < maxValue / 4)
        {
            sliderImage.color = Color.red;
        }
        else if (currentValue <= (maxValue / 2))
        {
            sliderImage.color = Color.yellow;
        }
        else if(currentValue>=(maxValue/4*3))
        {
            sliderImage.color = Color.cyan;
        }
        else
        {
            sliderImage.color = Color.green;
        }
    }
}
