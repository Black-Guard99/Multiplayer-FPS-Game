using System;
using UnityEngine;
public class WeponAnimationManager : MonoBehaviour {
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private GameObject bulletShell,shellEjectionPoint;
    [SerializeField,Range(0,2.5f)] private float reloadTimeMultiplier = 1f;
    private int reloadCount;
    private float reloadTime;

    // ^ Actions............
    public Action OnAfterReloadComplete,OnActualThrow,OnMeleHiting,OnWeponDrawIn;
    public Action<Transform> OnShellEject;

    private void Awake(){
        UpdateAnimClipTimes();
    }
    private void UpdateAnimClipTimes() {
        AnimationClip[] clips = gunAnimator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips) {
            if(clip.name == "Reload"){
                reloadTime = clip.length;
            }
        }
        reloadTime /= reloadTimeMultiplier;
    }
    public void SetSpeed(float speed){
        gunAnimator.SetFloat("Speed",speed);
    }
    public void DrawIn(){
        gunAnimator.SetBool("DrawIn",true);
        CancelInvoke(nameof(ResetDraw));
        Invoke(nameof(ResetDraw),.1f);
    }
    private void ResetDraw(){
        gunAnimator.SetBool("DrawIn",false);
    }
    
    public void Aim(bool isAiming){
        gunAnimator.SetBool("Aim",isAiming);
    }

    public void Shoot() {
        gunAnimator.SetBool("Shoot",false);
        CancelInvoke(nameof(ResetShooting));
        Invoke(nameof(ResetShooting),.1f);
        gunAnimator.SetBool("Shoot",true);
    }
    
    private void ResetShooting(){
        gunAnimator.SetBool("Shoot",false);
    }
    public void Die() {
        //? On Death Spawn Something..........................
        GameObject ragodlls = ObjectPoolingManager.Current.SpawnOverNetwork("Ragdoll",transform.position,transform.rotation);
        if(ragodlls.TryGetComponent(out RagdollSystem ragdollSystem)){
            ragdollSystem.ActivateRagDoll();
            ragdollSystem.DestroyMySelfWithDelay(5f);
        }
    }

    public void Reload(int reloadCount) {

        this.reloadCount = reloadCount;
        Debug.Log("Reloading Start");
        DecreaseReloadCount();
    }
    
    private void ResetReload(){
        gunAnimator.SetBool("Reload",false);
    }
    
    
    
    public void PullThePin(bool pull){
        if(pull){
            gunAnimator.SetBool("PinPull",true);
            CancelInvoke(nameof(ResetPinPull));
            Invoke(nameof(ResetPinPull),.1f);
        }else{
            gunAnimator.SetBool("Canceld",true);
            CancelInvoke(nameof(ResetCanceld));
            Invoke(nameof(ResetCanceld),.1f);
        }
    }
    private void ResetPinPull(){
        gunAnimator.SetBool("PinPull",false);
    }
    private void ResetCanceld(){
        gunAnimator.SetBool("Canceld",false);
    }
    public void HoldThrowable() {
        gunAnimator.SetBool("Hold",true);
    }

    public void Throw() {
        gunAnimator.SetBool("Hold",false);
        Shoot();
    }
    public void UnHoldThrow(){
        gunAnimator.SetBool("Hold",false);
    }
    private void ActualThrow(){
        OnActualThrow?.Invoke();
    }

    public void CancleReload(){
        gunAnimator.SetBool("ReloadCancle",true);
        CancelInvoke(nameof(ResetCanceldReload));
        Invoke(nameof(ResetCanceldReload),.1f);
    }
    private void ResetCanceldReload(){
        gunAnimator.SetBool("ReloadCancle",false);
    }




    //^ Calling from Animations Event..........

    private void ReloadComplete(){
        // Calling from animation Event AFter Reload Complete...........
        Debug.Log("Reloading End");
        OnAfterReloadComplete?.Invoke();
    }
    private void DecreaseReloadCount(){
        //! Calling from all the Reloding Animations......
        // Calling from Shotgun Pumpin in Shell.
        if(reloadCount <= 0){
            reloadCount = 0;
            ResetReload();
            return;
        }
        gunAnimator.SetBool("Reload",true);
        CancelInvoke(nameof(ResetReload));
        Invoke(nameof(ResetReload),.1f);
        reloadCount--;

    }
    public float GetReloadingTime(){
        return reloadTime;
    }
    private void AfterWeponDrawn(){
        // Calling from Drawing In Animation End Event..........
        OnWeponDrawIn?.Invoke();
    }
    

    private void ShellEject(){
        OnShellEject?.Invoke(shellEjectionPoint.transform);
    }
    private void ShowBulletShell(){
        if(bulletShell != null){
            bulletShell.SetActive(true);
        }
    }
    private void HideBulletShell(){
        if(bulletShell != null){
            bulletShell.SetActive(false);
        }
    }
    private void OnMeleHit(){
        OnMeleHiting?.Invoke();
    }
}