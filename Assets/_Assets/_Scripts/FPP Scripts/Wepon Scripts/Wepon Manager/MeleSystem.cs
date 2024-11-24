using UnityEngine;
using Photon.Pun;

public class MeleSystem : MonoBehaviour {
    [SerializeField] private WeponAnimationManager gunAnimationManager;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Gun gunSystem;
    [SerializeField] private Transform hitCheckPoint;
    [SerializeField] private float hitCheckRadius;
    [SerializeField] private LayerMask hitMask;
    private void Start(){
        tr.gameObject.SetActive(false);
        // gunSystem.OnShoot += GunSystem_OnShoot;
        gunAnimationManager.OnMeleHiting += GunAnimationManager_OnMeleHitting;
    }

    /* private void GunSystem_OnShoot() {
        tr.gameObject.SetActive(true);
        CancelInvoke(nameof(ResetTril));
        Invoke(nameof(ResetTril),1f);
    }
    private void ResetTril(){
        tr.gameObject.SetActive(false);
    } */

    private void GunAnimationManager_OnMeleHitting() {
        if(Physics.Raycast(hitCheckPoint.position,transform.forward,out RaycastHit raycastHit,hitCheckRadius,hitMask,QueryTriggerInteraction.UseGlobal)){
            ObjectPoolingManager.Current.SpawnEffectPool("HitEffect",raycastHit.point,Quaternion.identity);
        }
        Collider[] hitingColliders = Physics.OverlapSphere(hitCheckPoint.position,hitCheckRadius,hitMask,QueryTriggerInteraction.Collide);
        if(hitingColliders.Length > 0){
            foreach(Collider hit in hitingColliders){
                if(hit.TryGetComponent(out ITarget target)){
                    if(hit.TryGetComponent(out BodyPartsHitTarget bodyPart)){
                        if(!gunSystem.IsHitOwner(bodyPart)){
                            target.TakeHit(gunSystem.GetShootConfig.damageConfig.bodyDamageAmount,hitCheckPoint.position,PhotonNetwork.LocalPlayer.ActorNumber,hitCheckPoint.position,gunSystem.GetplayerData.username,"Knife");
                        }
                    }
                }
            }
        }
    }
    
    private void OnDrawGizmos(){
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(hitCheckPoint.position,transform.forward * hitCheckRadius);
        Gizmos.DrawWireSphere(hitCheckPoint.position,hitCheckRadius);
    }
}