using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class CrosshairLine : MonoBehaviour {
    [SerializeField] private Vector3 walkMovePos,runMovePos;
    [SerializeField] private RectTransform currentTransform;
    [SerializeField] private float expansionsSpeed = 2f;
    private Vector3 currentPoint;
    private void Awake(){
        currentPoint = currentTransform.anchoredPosition;
    }
    public void MoveAside(float speed){
        if(speed == 0.5f){
            currentTransform.DOKill(true);
            currentTransform.DOLocalMove(currentPoint + walkMovePos,expansionsSpeed).SetEase(Ease.OutBack);
        }
        if(speed == 1f){
            currentTransform.DOKill(true);
            currentTransform.DOLocalMove(currentPoint + runMovePos,expansionsSpeed).SetEase(Ease.OutBack);
        }
        if(speed == 0f){
            currentTransform.DOKill(true);
            currentTransform.DOLocalMove(currentPoint,expansionsSpeed).SetEase(Ease.OutBack);
        }
    }
}