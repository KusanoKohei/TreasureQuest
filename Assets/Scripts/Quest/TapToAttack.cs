using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TapToAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject icon;
    [SerializeField]
    private GameObject text;
    [SerializeField]
    private CanvasGroup canvasGroup;

    private RectTransform iconRect;
    private RectTransform textRect;

    private Sequence sequence;

    public static TapToAttack instance;

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


    private void Start()
    {
        iconRect = icon.GetComponent<RectTransform>();
        this.canvasGroup = canvasGroup.GetComponent<CanvasGroup>();

        sequence = DOTween.Sequence();
        sequence.Append(iconRect.DOPunchPosition(new Vector3(0, -100, 0), 1.5f,3,3f,false))
            // .AppendInterval(0.5f)
            .SetLoops(-1);
        
        sequence.Play();

        canvasGroup.DOFade(0.0f, 1.5f).SetEase(Ease.Linear).SetLoops(-1);
        
    }
}
