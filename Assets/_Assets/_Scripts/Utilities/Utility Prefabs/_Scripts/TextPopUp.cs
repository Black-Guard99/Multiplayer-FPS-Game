using TMPro;
using UnityEngine;
using DG.Tweening;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;
public class TextPopUp : PopUp {
    [SerializeField] private float maxLifeTime;
    [SerializeField] private Ease scaleEase = Ease.OutSine;
    [SerializeField] private TextMeshPro popUpText;
    public void SetText(string text,Color textColor){
        popUpText.SetText(text);
        popUpText.color = textColor;
    }

    protected override void OnActive() {
        transform.LookAt(Camera.main.transform.position);
        transform.DOKill(false);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
        transform.DOScale(0f,0.7f).SetEase(scaleEase).onComplete += () =>{
            DestroyNow();
        };
    }

    protected override void OnDeactive() { }
}
