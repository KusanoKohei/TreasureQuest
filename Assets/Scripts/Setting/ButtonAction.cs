using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ButtonAction : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnClick()
    {
        this.gameObject.transform.DOPunchScale(new Vector3 (0.2f, 0.2f), 0.2f);
    }
}
