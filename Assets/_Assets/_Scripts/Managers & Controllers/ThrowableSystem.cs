using System;
using UnityEngine;
public class ThrowableSystem : MonoBehaviour {
    [SerializeField] private OffScreenIndicator offScreenIndicator;
    [SerializeField] private TrajectoryPredictor trajectoryPredictor;
    [SerializeField] private ThrowableSO throwableSo;
    [SerializeField] private Transform handPos,headLookPoint;
    [SerializeField] private GameObject throwablePlaceHolderGfx,flashBangPlaceHolderGfx,smokePlaceHolderGfx,nuclearPlaceHolderGfx,molotovPlaceHolderGfx;
    [SerializeField] private GameObject grenadePin,smokePin,poisonPin,flashBangPin;
    [SerializeField] private WeponAnimationManager weponAnimationManager;
    private float currentCookTime;
    private ThrowableObject currentThrowable;
    private bool isCooking;
    private Action OnThrowComplete;
    private bool isThrowen;

    
    
    private void Start(){
        currentCookTime = throwableSo.maxCookingTime;
        SelectThrowable();
    }
    public void Init(Transform head,OffScreenIndicator _offScreenIndicator){
        headLookPoint = head;
        offScreenIndicator = _offScreenIndicator;
    }
    public void SelectThrowable(){
        throwablePlaceHolderGfx.SetActive(false);
        flashBangPlaceHolderGfx.SetActive(false);
        smokePlaceHolderGfx.SetActive(false);
        nuclearPlaceHolderGfx.SetActive(false);
        molotovPlaceHolderGfx.SetActive(false);
        switch(throwableSo.currentThrowableType){
            case ThrowableSO.ThrowableType.Grenade:
                throwablePlaceHolderGfx.SetActive(true);
            break;
            case ThrowableSO.ThrowableType.FlashBang:
                flashBangPlaceHolderGfx.SetActive(true);
            break;
            case ThrowableSO.ThrowableType.Molotove:
                molotovPlaceHolderGfx.SetActive(true);
            break;
            case ThrowableSO.ThrowableType.SmokeGrenade:
                smokePlaceHolderGfx.SetActive(true);
            break;
            case ThrowableSO.ThrowableType.PoisonGrenade:
                nuclearPlaceHolderGfx.SetActive(true);
            break;
        }
        SelectPin(false);
    }
    public void SelectPin(bool active){
        grenadePin.SetActive(false);
        smokePin.SetActive(false);
        flashBangPin.SetActive(false);
        poisonPin.SetActive(false);
        switch(throwableSo.currentThrowableType){
            case ThrowableSO.ThrowableType.Grenade:
                grenadePin.SetActive(true);
            break;
            case ThrowableSO.ThrowableType.FlashBang:
                flashBangPin.SetActive(true);
            break;
            case ThrowableSO.ThrowableType.SmokeGrenade:
                smokePin.SetActive(true);
            break;
            case ThrowableSO.ThrowableType.PoisonGrenade:
                poisonPin.SetActive(true);
            break;
        }
    }
    public void CookThrowable(){
        if(throwableSo.ammoConfig.currentMaxAmmoo <= 0){
            trajectoryPredictor.SetTrajectoryVisible(false);
            return;
        }
        if(isThrowen)return;
        SelectPin(true);
        currentCookTime = throwableSo.maxCookingTime;
        weponAnimationManager.PullThePin(true);
        weponAnimationManager.HoldThrowable();
    }
    private void CookingStart(){
        Debug.Log("cooking Start");
        SelectPin(false);
        // calls form Animations Event after Pin is Pulled....
        isThrowen = false;
        isCooking = true;
    }
    public void Throw(Action OnThrowComplete){
        if(!isCooking){
            weponAnimationManager.PullThePin(false);
            trajectoryPredictor.SetTrajectoryVisible(false);
            SelectPin(true);
            return;
        }
        isCooking = false;
        currentCookTime = throwableSo.maxCookingTime;
        currentThrowable = null;
        weponAnimationManager.OnActualThrow += Thrown;
        weponAnimationManager.Throw();
        this.OnThrowComplete = OnThrowComplete;
        
    }
    private void Update(){
        if(isCooking){
            trajectoryPredictor.SetTrajectoryVisible(true);
            trajectoryPredictor.PredictTrajectory(ProjectileData());
            switch(throwableSo.currentThrowableType){
                case ThrowableSO.ThrowableType.Grenade:
                case ThrowableSO.ThrowableType.FlashBang:
                case ThrowableSO.ThrowableType.SmokeGrenade:
                case ThrowableSO.ThrowableType.PoisonGrenade:
                    currentCookTime -= Time.deltaTime;
                    if(currentCookTime <= 0f){
                        if(!isThrowen){
                            currentThrowable = SpawnThrowable();
                            currentThrowable.SetCookedTime(currentCookTime);
                            isCooking = false;
                            currentThrowable = null;
                            weponAnimationManager.UnHoldThrow();
                            trajectoryPredictor.SetTrajectoryVisible(false);
                            OnThrowComplete?.Invoke();
                        }
                    }
                break;
            }
        }
    }
    public void CancleHold(){
        currentCookTime = throwableSo.maxCookingTime;
        isCooking = false;
        weponAnimationManager.UnHoldThrow();
        trajectoryPredictor.SetTrajectoryVisible(false);
    }
    private ProjectileProperties ProjectileData() {
        ProjectileProperties properties = new ProjectileProperties();
        properties.direction = headLookPoint.forward;
        properties.initialPosition = handPos.position;
        properties.initialSpeed = throwableSo.force;
        properties.mass = throwableSo.mass;
        properties.drag = throwableSo.drag;

        return properties;
    }
    // private GameObject curentThrowObject;
    private ThrowableObject SpawnThrowable(){
        GameObject throwable = ObjectPoolingManager.Current.SpawnOverNetwork(throwableSo.shootConfig.bulletPoolName.prefabs.name, handPos.position, Quaternion.identity);
        return throwable.GetComponent<ThrowableObject>();
    }
    
    private void Thrown(){
        if(!isThrowen){
            Debug.Log("Spawning the " + throwableSo.shootConfig.bulletPoolName.name);
            currentThrowable = SpawnThrowable();
            currentThrowable.Throw(throwableSo.force,headLookPoint.forward,currentCookTime,throwableSo.drag,throwableSo.mass,offScreenIndicator);
            trajectoryPredictor.SetTrajectoryVisible(false);
            currentThrowable = null;
            weponAnimationManager.OnActualThrow -= Thrown;
            OnThrowComplete?.Invoke();
            isThrowen = true;
            CancelInvoke(nameof(ResetThrowing));
            Invoke(nameof(ResetThrowing),1.1f);
        }
    }
    private void ResetThrowing(){
        isThrowen = false;
    }
    public ThrowableSO GetThrowableSO{
        get{
            return throwableSo;
        }
    }
    public float GetHoldTimerNormalized(){
        return currentCookTime / throwableSo.maxCookingTime;
    }
}