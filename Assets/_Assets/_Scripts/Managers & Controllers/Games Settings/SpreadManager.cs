using UnityEngine;
using DG.Tweening;

public class SpreadManager : MonoBehaviour {
    [SerializeField] private Ease moveEase;
    [SerializeField] private RectTransform center;
    [SerializeField] private float radius;
    private void Start(){
        Spread();
    }
    public void Spread(){
        int num = center.childCount;
        for(int i = 0; i < num; i++){
            if(center.GetChild(i).TryGetComponent(out RectTransform rectTransform)){
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
        for (int i = 0; i < num; i++) {
            float ang = i * Mathf.PI * 2 / num;
            Vector3 pos = FindPointAround(center.anchoredPosition,radius,ang);
            if(center.GetChild(i).TryGetComponent(out RectTransform rectTransform)){
                rectTransform.DOLocalMove(pos,.3f,false).SetEase(moveEase);
                // rectTransform.anchoredPosition = pos;
            }
        }
    }
    private Vector3 FindPointAround(Vector3 center, float radius, float ang){
        Vector3 pos = Vector3.zero;
        pos.x = center.x + Mathf.Sin(ang) * radius;
        pos.y = center.y + Mathf.Cos(ang) * radius;
        return pos;
    }
}