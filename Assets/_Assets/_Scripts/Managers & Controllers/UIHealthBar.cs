using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
public class UIHealthBar : MonoBehaviour {
    private const float fadeTimerMax = 1.5f;
    [SerializeField] private Volume healthVolume;
    [SerializeField] private TextMeshProUGUI healthAmountText;
    [SerializeField] private Transform hitIndicatorParent;
    [SerializeField] private Image helalthBarMain,healthBarDamaged,healthBarBlinked;
    private float currentFadeTime;
    private float healtFlashingTimer;
    private float previousHealthAmount;
    private float maxHealth,previousHealth;
    private void Awake(){
        healtFlashingTimer = +.4f;
        healthBarBlinked.gameObject.SetActive(false);
    }
    private void Update(){
        if(healthBarDamaged.color.a > 0){
            currentFadeTime -= Time.deltaTime;
            if(currentFadeTime <= 0){
                Color newColor = healthBarDamaged.color;
                newColor.a -= Time.deltaTime * 2f;
                if(newColor.a <= 0){
                    newColor.a = 0f;
                }
                healthBarDamaged.color = newColor;
            }
        }
        if(healthBarBlinked.gameObject.activeSelf){
            Color lowHealthColor = healthBarBlinked.color;
            lowHealthColor.a += healtFlashingTimer;
            if(lowHealthColor.a > 1f){
                healtFlashingTimer *= -1f;
                lowHealthColor.a = 1f;
            }
            if(lowHealthColor.a < 0f){
                healtFlashingTimer *= -1f;
                lowHealthColor.a = 0f;
            }
            healthBarBlinked.color = lowHealthColor;
        }
    }
    public void SetCurrentHealth(float healthNormalized){
        currentFadeTime = fadeTimerMax;
        if(healthBarDamaged.color.a <= 0f){
            healthBarDamaged.fillAmount = previousHealth / maxHealth;
            Color damagedfullAlpha = healthBarDamaged.color;
            damagedfullAlpha.a = 1f;
            healthBarDamaged.color = damagedfullAlpha;
        }
        if(healthNormalized <= .3f){
            healthBarBlinked.gameObject.SetActive(true);
        }else{
            healthBarBlinked.gameObject.SetActive(false);
        }
        helalthBarMain.fillAmount = healthNormalized;
        healthAmountText.SetText((Mathf.CeilToInt(healthNormalized * 100f)).ToString());
    }
    public void SetHealthPPVolume(float inverseHealthNormalized){
        healthVolume.weight = inverseHealthNormalized;
    }
    
    public void SetPreviouseAndTotalHealth(float previouseHealth,float totalHealth){
        this.previousHealth = previouseHealth;
        this.maxHealth = totalHealth;
    }
    public void SetRegenerationHealth(float healthNormalized,float previouseHealthNormalized){
        currentFadeTime = fadeTimerMax;
        healthBarDamaged.fillAmount = previouseHealthNormalized;
        if(healthBarDamaged.color.a <= 0f){
            Color damagedfullAlpha = healthBarDamaged.color;
            damagedfullAlpha.a = 1f;
            healthBarDamaged.color = damagedfullAlpha;
        }
        if(healthNormalized <= .3f){
            healthBarBlinked.gameObject.SetActive(true);
        }else{
            healthBarBlinked.gameObject.SetActive(false);
        }
        helalthBarMain.fillAmount = healthNormalized;
        healthAmountText.SetText((Mathf.CeilToInt(healthNormalized * 100f)).ToString());
    }
    public void SetDamageIndicatorDirection(Vector3 hitSourceDirection,Vector3 playerPos,Vector3 playerForward){
        GameObject indicatorNew = ObjectPoolingManager.Current.SpawnFromPool("Hit Direction Indicator");
        indicatorNew.transform.SetParent(hitIndicatorParent);
        indicatorNew.transform.localPosition = Vector3.zero;
        if(indicatorNew.TryGetComponent(out HitDirectionIndicator indicator)){
            indicator.SetDamageIndicatorDirection(hitSourceDirection,playerPos,playerForward);
        }
    }
    
}