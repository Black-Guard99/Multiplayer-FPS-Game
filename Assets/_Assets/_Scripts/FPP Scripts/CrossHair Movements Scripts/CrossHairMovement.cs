using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class CrossHairMovement : MonoBehaviour {
    public static CrossHairMovement Current{get; private set;}

    [SerializeField] private RectTransform hitCrossHairVisual;
    [SerializeField] private float crosshairExpansionSpeed = 2f;
    [SerializeField] private RectTransform crossHairMain;
    [SerializeField] private Image[] hitCrossHairLine;

    [SerializeField] private float maxSizeWalk,maxSizeRun;
    private float resetingSize,currentSize;
    private void Awake(){
        if(Current == null){
            Current = this;
        }else{
            Destroy(Current.gameObject);
        }
        hitCrossHairVisual.gameObject.SetActive(false);
        resetingSize = crossHairMain.sizeDelta.x;
        currentSize = crossHairMain.sizeDelta.x;
    }
    public void ShowHideCrossHairWindow(bool show){
        crossHairMain.gameObject.SetActive(show);
    }
    public void SetCrossHairLines(float speed){
        if(speed == .5f){
            currentSize = Mathf.Lerp(currentSize,maxSizeWalk,crosshairExpansionSpeed * Time.deltaTime);
        }else if(speed == 1f){
            currentSize = Mathf.Lerp(currentSize,maxSizeRun,crosshairExpansionSpeed * Time.deltaTime);
        }else{
            currentSize = Mathf.Lerp(currentSize,resetingSize,crosshairExpansionSpeed * Time.deltaTime);
        }
        crossHairMain.sizeDelta = new Vector2(currentSize,currentSize);
    }
    public void ShowHitCrossHair(Color hitLineColor){
        hitCrossHairVisual.gameObject.SetActive(true);
        hitCrossHairVisual.transform.DOKill(false);
        hitCrossHairVisual.DOScale(1.4f,0.2f).SetEase(Ease.OutBack).onComplete += ()=>{hitCrossHairVisual.transform.DOScale(1f,.2f);};
        foreach(Image hitLine in hitCrossHairLine){
            hitLine.color = hitLineColor;
        }
        CancelInvoke(nameof(HideHitCrossHair));
        Invoke(nameof(HideHitCrossHair),1f);
    }
    public void HideHitCrossHair(){
        hitCrossHairVisual.gameObject.SetActive(false);
    }
}