using Photon.Pun;
using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using System;
using Baracuda.Monitoring;
using System.Collections.ObjectModel;
[DefaultExecutionOrder(-10)]
public class Loadout : MonoBehaviourPunCallbacks,IPunObservable {
    [SerializeField] private Transform headCheckPoint;
    [SerializeField] private LayerMask headCheckLayer;
	[SerializeField] private float headCheckLength =2f;
    [SerializeField] private OffScreenIndicator offScreenIndicator;
    [SerializeField] private CinemachineVirtualCamera nonAimingCamera;
    [SerializeField] private List<BodyPartsHitTarget> ownerBodyPartsList;
    [SerializeField] private LineRenderer gunLine;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private NewPlayerInputController inputController;
    [SerializeField] private GameObject leaningSlider;
    [SerializeField] private float spinreRotationSpeed;
    [SerializeField] private List<LoadoutSO> allLoadoutList;
    [SerializeField] private LoadoutSO currentLoadout;
    [SerializeField] private Transform spineRotation;
    [SerializeField] private Transform chestWeponHolder;
    [SerializeField] private WeponUIManager weponUIManager;
    [SerializeField] private NightVisionSystem nightVisionSystem;
    [SerializeField] private Gun CurrentGun;
    [SerializeField] private List<Gun> loadWeponsList;
    private bool isPrimary = true;
    private int CurrentGunIndex;
    private float currentRotaion,spineRotationRef;
    private bool tap = false;
    private bool aiming;
    private bool crouch;
    private bool jump;
    private bool isUsingMele;
    private bool slide;
    private ProfileData playerProfile;
    private bool IsReloading;
    private float currentSpeed;
    private Vector2 recoilAmount;
    public Gun GetCurrentGun{
        get{
            return CurrentGun;
        }
    }
    public bool IsAiming{
        get{
            return aiming;
        }
        private set{
            aiming = value;
        }
    }
    private void Awake(){
        Monitor.StartMonitoring(this);
        this.StartMonitoring();
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
    }
    private void OnDestroy(){
        Monitor.StopMonitoring(this);
        this.StopMonitoring();
    }
    public void SetUpLoadout(){
        for (int i = 0; i < allLoadoutList.Count; i++) {
            if(allLoadoutList[i].isActiveLoadout){
                currentLoadout = allLoadoutList[i];
            }
        }
        loadWeponsList = new List<Gun>();
        SpawnAndAddWeponsToList();
        isUsingMele = false;
        leaningSlider.SetActive(false);
        
        CurrentGunIndex = 0;
        for(int i = 0; i < loadWeponsList.Count; i++){
            loadWeponsList[i].SetUp(photonView.IsMine,ownerBodyPartsList,nonAimingCamera,gunLine,offScreenIndicator);
            SetUpTotalBulletsForAllWepons();
            playerData.SetWeponIcons(loadWeponsList[i].GetGunSprite,loadWeponsList[i].GetWeponType);
        }
        if(photonView.IsMine){
            CrossHairMovement.Current?.ShowHideCrossHairWindow(true);
        }else{
            CrossHairMovement.Current?.ShowHideCrossHairWindow(false);
        }
        weponUIManager.RefreshThrowableUi();
        CheckActiveGun();
    }

    private void SpawnAndAddWeponsToList(){
        Gun primaryWeponSpawn = Instantiate(currentLoadout.primaryGunAttachments.gunCurrent.gunModel,chestWeponHolder);
        if(!loadWeponsList.Contains(primaryWeponSpawn)){
            loadWeponsList.Add(primaryWeponSpawn);
        }
        primaryWeponSpawn.gameObject.SetActive(false);
        Gun secondaryWeponSpawn = Instantiate(currentLoadout.secondaryGunAttachments.gunCurrent.gunModel,chestWeponHolder);
        if(!loadWeponsList.Contains(secondaryWeponSpawn)){
            loadWeponsList.Add(secondaryWeponSpawn);
        }
        secondaryWeponSpawn.gameObject.SetActive(false);
        Gun meleWeponSpawn = Instantiate(currentLoadout.meleWeaponAttachments.gunCurrent.gunModel,chestWeponHolder);
        if(!loadWeponsList.Contains(meleWeponSpawn)){
            loadWeponsList.Add(meleWeponSpawn);
        }
        meleWeponSpawn.gameObject.SetActive(false);
        Gun lethalWeponSpawn = Instantiate(currentLoadout.lethalThrowabl.gunCurrent.gunModel,chestWeponHolder);
        if(!loadWeponsList.Contains(lethalWeponSpawn)){
            loadWeponsList.Add(lethalWeponSpawn);
        }
        lethalWeponSpawn.gameObject.SetActive(false);
        Gun nonLethalWeponSpawn = Instantiate(currentLoadout.nonLethalThrowabl.gunCurrent.gunModel,chestWeponHolder);
        if(!loadWeponsList.Contains(nonLethalWeponSpawn)){
            loadWeponsList.Add(nonLethalWeponSpawn);
        }
        nonLethalWeponSpawn.gameObject.SetActive(false);
    }
    private void Update(){
        if(photonView.IsMine){
            if(Input.mouseScrollDelta.y > 0){
                if(aiming){
                    aiming = false;
                    Aim(false);
                }
                CurrentGun.CancleReload();
                SwitchPrimaryAndSecondaryWepons();
                CheckActiveGun();
            }else if(Input.mouseScrollDelta.y < 0){
                if(aiming){
                    aiming = false;
                    Aim(false);
                }
                CurrentGun.CancleReload();
                SwitchPrimaryAndSecondaryWepons();
                CheckActiveGun();
            }
            if(Input.GetKeyDown(KeyCode.G)){
                if(aiming){
                    aiming = false;
                    CurrentGun.ToggleAim(aiming);
                }
                SwitchMele();
            }
            if(Input.GetKeyDown(KeyCode.N)){
                nightVisionSystem.ToggleNightVision();
            }
            
            if(inputController.GetPcTap){
                TapOnFireButton();
                photonView.RPC(nameof(ShowWeponTrail),RpcTarget.AllBuffered);
            }
            if(inputController.GetPcTapHold){
                HoldingDownFireButton();
                photonView.RPC(nameof(ShowWeponTrail),RpcTarget.AllBuffered);
            }
            if(inputController.GetPcTapEnd){
                ReleaseFireButton();
                photonView.RPC(nameof(ShowWeponTrail),RpcTarget.AllBuffered);
            }
            if(inputController.RelaodingInput){
                StartReload();
            }
            if(inputController.GetPcAimToggle){
                HandleingAiming();
            }
            if(CurrentGun != null){
                IsReloading = CurrentGun.IsReloading;
            }
        }
    }

    public void HandleingAiming(){
        if(photonView.IsMine){
            if(!CurrentGun.GetShootConfig.canAim) return;
            if(TooCloseToObstacles()){
                return;
            }
            aiming = !aiming;
            Aim(aiming);
            if(aiming){
                leaningSlider.SetActive(true);
            }else{
                leaningSlider.SetActive(false);
                SpineRotation(0f);
            }
        }
    }
    
    public Vector3 RecoilAmount{
        get{
            recoilAmount = CurrentGun.GetRecoilAmount();
            return CurrentGun.GetRecoilAmount();
        }
    }

    [PunRPC]
	private void ShowWeponTrail(){
        if(CurrentGun != null){
            CurrentGun.ShowHideTrail(true);
        }
        CancelInvoke(nameof(HideTrail));
        Invoke(nameof(HideTrail),.2f);
	}
    private void HideTrail(){
        if(CurrentGun != null){
            CurrentGun.ShowHideTrail(false);
        }
    }
    public void TapOnFireButton(){
        if(CurrentGun != null){
            if(!TooCloseToObstacles()){
                CurrentGun.Tap();
            }
        }
    }
    public void HoldingDownFireButton(){
        if(CurrentGun != null){
            if(!TooCloseToObstacles()){
                CurrentGun.HoldDown();
            }
        }
    }
    public void ReleaseFireButton(){
        if(CurrentGun != null){
            if(!TooCloseToObstacles()){
                CurrentGun.TapRelease();
            }
        }
    }
    public void CancleHold(){
        CurrentGun.CancleHold();
    }
    
    public void SetPlayerSpeed(float speed){
        currentSpeed = speed;
        if(CurrentGun != null){
            CurrentGun.SetSpeed(currentSpeed);
        }
    }
    public void Aim(bool aim){
        if(CurrentGun != null){
            CurrentGun.ToggleAim(aim);
        }
    }
    public void Lean(float value){
        SpineRotation(value);
    }
    public void StartReload(){
        CurrentGun.StartReloading();
    }

    
    private void SpineRotation(float rotationValue){
        currentRotaion = Mathf.SmoothDamp(currentRotaion,rotationValue,ref spineRotationRef,spinreRotationSpeed * Time.deltaTime);
        SetRotation();
    }
    private void CheckActiveGun(){
        for (int i = 0; i < loadWeponsList.Count; i++) {
            if(i == CurrentGunIndex){
                if(CurrentGun != null){
                    CurrentGun.OnShoot -= OnShoot_CurrentGun;
                    CurrentGun.OnAfterReloadComplete -= OnAfterReloadComplete_CurrentGun;
                    CurrentGun.SetCanUseWepon(false);
                    CancleReload();
                }
                CurrentGun = loadWeponsList[i];
                CurrentGun.gameObject.SetActive(true);
                CurrentGun.DrawIn();
                CurrentGun.OnShoot += OnShoot_CurrentGun;
                CurrentGun.OnAfterReloadComplete += OnAfterReloadComplete_CurrentGun;
                SetUpTotalBulletsForAllWepons();
            }else{
                loadWeponsList[i].gameObject.SetActive(false);
            }
        }
    }
    private void SetUpTotalBulletsForAllWepons(){
        for (int i = 0; i < loadWeponsList.Count; i++) {
            playerData.SetBulletCount(loadWeponsList[i].GetCurrentBulletCount,loadWeponsList[i].GetMaxBulletCount,loadWeponsList[i].GetWeponType);
        }
    }
    private void OnShoot_CurrentGun(){
        SetUpTotalBulletsForAllWepons();
    }
    private void OnAfterReloadComplete_CurrentGun(){
        SetUpTotalBulletsForAllWepons();
        Debug.Log("Reloading Start");
    }

    // OnPhotonSerializeView Sending Only
    private int lastSendIndex;
    private bool lastAimPos;
    private bool lastCheckReloading;
    private float lastSpeed;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting){
            stream.SendNext(currentRotaion);
            stream.SendNext(CurrentGunIndex);
            stream.SendNext(aiming);
            stream.SendNext(IsReloading);
            stream.SendNext(currentSpeed);
        }else{
            currentRotaion = (float)stream.ReceiveNext();
            CurrentGunIndex = (int)stream.ReceiveNext();
            aiming = (bool)stream.ReceiveNext();
            IsReloading = (bool)stream.ReceiveNext();
            currentSpeed = (float)stream.ReceiveNext();
            ///....................
            if(lastSpeed != currentSpeed){
                lastSpeed = currentSpeed;
                SetPlayerSpeed(currentSpeed);
            }
            if(lastCheckReloading != IsReloading){
                lastCheckReloading = IsReloading;
                CurrentGun.CheckCanPlayReloading();
            }
            if(CurrentGunIndex != lastSendIndex){
                lastSendIndex = CurrentGunIndex;
                CheckActiveGun();
            }
            if(lastAimPos != aiming){
                lastAimPos = aiming;
                Aim(aiming);
            }
            SetRotation();
        }
    }
    private bool TooCloseToObstacles(){
        if(Physics.Raycast(headCheckPoint.position,transform.forward,out RaycastHit hit,headCheckLength,headCheckLayer,QueryTriggerInteraction.Ignore)){
            Debug.Log("Hit With " + hit.transform.name);
            return true;
        }
        return false;
    }
    private void SetRotation(){
        spineRotation.localEulerAngles = new Vector3(spineRotation.localEulerAngles.x,spineRotation.localEulerAngles.y,currentRotaion);
    }

    public void Die(){
        nonAimingCamera.enabled = false;
        CurrentGun.SetDead();
    }
    public void SelectSingleThrowable(ThrowableSO.ThrowableType throwableType){
        isPrimary = true;
        if(!aiming && !CurrentGun.IsReloading){
            for (int i = 0; i < loadWeponsList.Count; i++) {
                if(loadWeponsList[i].TryGetComponent(out ThrowableSystem throwableSystem)){
                    if(throwableSystem.GetThrowableSO.currentThrowableType == throwableType){
                        Gun currentLethalThrowable = loadWeponsList[i];
                        if(currentLethalThrowable.HasAmmoo){
                            CurrentGunIndex = loadWeponsList.IndexOf(currentLethalThrowable);
                        }else{
                            return;
                        }
                        break;
                    }
                }
            }
        }
        CheckActiveGun();
    }
    public Gun GetCurrentThrowable(ThrowableSO.ThrowableType throwableType){
        for (int i = 0; i < loadWeponsList.Count; i++) {
            if(loadWeponsList[i].TryGetComponent(out ThrowableSystem throwableSystem)){
                if(throwableSystem.GetThrowableSO.currentThrowableType == throwableType){
                    return loadWeponsList[i];
                }
            }
        }
        return null;
    }
    public float GetThrowableTimer(){
        return CurrentGun.GetHoldTime();
    }
    public void SwitchtoPrimaryOnly(){
        if(aiming){
            aiming = false;
            Aim(false);
        }
        if(!aiming && !CurrentGun.IsReloading){
            isPrimary = true;
            for (int i = 0; i < loadWeponsList.Count; i++) {
                if(loadWeponsList[i].GetWeponType == Gun.WeponPositions.Primary){
                    CurrentGunIndex = i;
                }
            }
            CheckActiveGun();
        }
    }
    public void SwitchtoSecondaryOnly(){
        if(aiming){
            aiming = false;
            Aim(false);
        }
        if(!aiming && !CurrentGun.IsReloading){
            isPrimary = false;
            for (int i = 0; i < loadWeponsList.Count; i++) {
                if(loadWeponsList[i].GetWeponType == Gun.WeponPositions.Secondary){
                    CurrentGunIndex = i;
                }
            }
            CheckActiveGun();
        }
    }
    public void SwitchPrimaryAndSecondaryWepons() {
        if(aiming){
            aiming = false;
            Aim(false);
        }
        if(!aiming && !CurrentGun.IsReloading){
            isPrimary = !isPrimary;
            for (int i = 0; i < loadWeponsList.Count; i++) {
                if(isPrimary){
                    if(loadWeponsList[i].GetWeponType == Gun.WeponPositions.Primary){
                        CurrentGunIndex = i;
                    }
                }else{
                    if(loadWeponsList[i].GetWeponType == Gun.WeponPositions.Secondary){
                        CurrentGunIndex = i;
                    }
                }
            }
            CheckActiveGun();
        }
    }
    public void SwitchMele(){
        if(aiming){
            aiming = false;
            Aim(false);
        }
        if(!aiming && !CurrentGun.IsReloading){
            for (int i = 0; i < loadWeponsList.Count; i++) {
                if(loadWeponsList[i].GetWeponType == Gun.WeponPositions.Mele){
                    isUsingMele = true;
                    CurrentGunIndex = i;
                }
                
            }
            CheckActiveGun();
        }
    }

    public void SetPlayerData(ProfileData playerProfile) {
        foreach(Gun gun in loadWeponsList){
            gun.SetPlayerProfile(playerProfile);
        }
    }

    public void CancleReload() {
        GetCurrentGun.CancleReload();
    }
    private void OnDrawGizmos(){
		Gizmos.color = Color.red;
        Gizmos.DrawRay(headCheckPoint.position,transform.forward * headCheckLength);
	}
}