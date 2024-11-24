using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class RaycastProjectileMovement : MonoBehaviour,IPooledObject {
    private bool isInitialized = false;
    private float speed,gravity;
    private Vector3 startPosition,startForward;
    private TrailRenderer tr;
    private float startTime = -1;

    // ^ After Hit Check.....................
    private List<BodyPartsHitTarget> ownerBodyPartsList;
    private GunSO gunSO;
    private Vector3 shooterPos;
    private void Awake(){
        tr = GetComponent<TrailRenderer>();
    }

    public void Initalized(Vector3 startPoint,Vector3 startForward,float speed ,float gravity,List<BodyPartsHitTarget> ownerBodyPartsList,GunSO gunSO,Vector3 shooterPos){
        this.ownerBodyPartsList = new List<BodyPartsHitTarget>();
        this.gunSO = gunSO;
        this.ownerBodyPartsList = ownerBodyPartsList;
        this.startPosition = startPoint;
        this.startForward = startForward;
        this.speed = speed;
        this.gravity = gravity;
        tr.emitting = true;
        isInitialized = true;
        this.shooterPos = shooterPos;
        DestroyMySelfWithDelay(gunSO.trailConfig.missDistance);
    }
    private Vector3 FindPointOnParabola(float time){
        Vector3 Point = startPosition  + (startForward * speed * time);
        Vector3 gravityVec = Vector3.down * gravity * time * time;
        return Point + gravityVec;
    }
    private bool CastRayBetweenPoints(Vector3 startPoint,Vector3 endPoint, out RaycastHit hit){
        return Physics.Raycast(startPoint,(endPoint - startPoint),out hit,(endPoint - startPoint).magnitude,gunSO.shootConfig.hitMask,QueryTriggerInteraction.UseGlobal);
    }
    private void FixedUpdate(){
        if(!isInitialized) return;
        if(startTime < 0) startTime = Time.time;
        float currentTime = Time.time  - startTime;
        float nextTime = currentTime + Time.fixedDeltaTime;

        Vector3 currentPoint = FindPointOnParabola(currentTime);
        Vector3 nextPoint = FindPointOnParabola(nextTime);
        if(CastRayBetweenPoints(currentPoint,nextPoint,out RaycastHit hit)){
            tr.emitting = false;
            Debug.Log("Hit To " + hit.transform.name);
            if(hit.transform.TryGetComponent(out ITarget target)){
				if(hit.transform.TryGetComponent(out BodyPartsHitTarget bodyPart)){
					if(!IsHitOwner(bodyPart)){
                        ObjectPoolingManager.Current.SpawnEffectPool("BloodEffect",hit.point,Quaternion.identity);
                        // GameObject holeObject = ObjectPoolingManager.Current.SpawnFromPool("BulletHole",hit.point,Quaternion.identity);
                        // holeObject.transform.rotation = Quaternion.FromToRotation(-holeObject.transform.forward,hit.normal);
						// float damageValue = gunSO.shootConfig.damageConfig.GetDamageValue(Vector3.Distance(startPosition,hit.point));
                        float damageValue = gunSO.shootConfig.damageConfig.bodyDamageAmount;
						target.TakeHit(damageValue,hit.point,PhotonNetwork.LocalPlayer.ActorNumber,shooterPos,gunSO.playerProfile.username,gunSO.playerProfile.gunName);
					}
				}else{
                    Debug.Log("Hit To " + hit.transform.name);
                    // GameObject holeObject = ObjectPoolingManager.Current.SpawnFromPool("BulletHole",hit.point,Quaternion.identity);
                    // holeObject.transform.rotation = Quaternion.FromToRotation(-holeObject.transform.forward,hit.normal);
					// float damageValue = gunSO.shootConfig.damageConfig.GetDamageValue(Vector3.Distance(startPosition,hit.point));
                    float damageValue = gunSO.shootConfig.damageConfig.bodyDamageAmount;
                    Debug.Log("Damage Value " + damageValue);
					target.TakeHit(damageValue,hit.point,PhotonNetwork.LocalPlayer.ActorNumber,shooterPos,gunSO.playerProfile.username,gunSO.playerProfile.gunName);
				}
			}else{
				GameObject holeObject = ObjectPoolingManager.Current.SpawnFromPool("BulletHole",hit.point,Quaternion.identity);
                holeObject.transform.rotation = Quaternion.FromToRotation(-holeObject.transform.forward,hit.normal);
				// *Play Impact Effect.
				CheckNonLivingTarget(hit.point);
			}
            DestroyNow();
        }
    }
    private void Update(){
        if(!isInitialized || startTime < 0) return;
        float currentTime = Time.time - startTime;
        Vector3 currentPoint = FindPointOnParabola(currentTime);
        transform.position = currentPoint;
    }

    public void DestroyMySelfWithDelay(float delay = 0f){
        CancelInvoke(nameof(DestroyNow));
        Invoke(nameof(DestroyNow),delay);
    }
    public bool IsHitOwner(BodyPartsHitTarget ownerBodyPart){
		return ownerBodyPartsList.Contains(ownerBodyPart);
	}
    private void CheckNonLivingTarget(Vector3 hitPoint){
		Collider[] colliders = Physics.OverlapSphere(hitPoint,.1f,gunSO.shootConfig.hitMask,QueryTriggerInteraction.UseGlobal);
		foreach (Collider colis in colliders) {
			if(colis.transform.TryGetComponent(out ITarget target)){
				if(colis.transform.TryGetComponent(out BodyPartsHitTarget hitTarget)){
					if(!IsHitOwner(hitTarget)){
						// float damageValue = gun.shootConfig.damageValue.GetDamageValue(hitPoint)
						float damageValue = gunSO.shootConfig.damageConfig.bodyDamageAmount;
						target.TakeHit(damageValue,hitPoint,PhotonNetwork.LocalPlayer.ActorNumber,shooterPos,gunSO.playerProfile.username,gunSO.playerProfile.gunName);
					}
				}
			}
		}
	}
    

    public void OnObjectReuse(){
        gameObject.SetActive(true);
    }

    public void DestroyNow(){
        tr.emitting = false;
        isInitialized = false;
        startTime = -1;
        startPosition = Vector3.zero;
        startForward = Vector3.zero;
        transform.SetParent(startingParent);
        CancelInvoke(nameof(DestroyNow));
        gameObject.SetActive(false);
    }
#region Pooling Attributes.................................
    
    private Transform startingParent;
    public void SetStartinParent(Transform _parent) {
        startingParent = _parent;
    }

    

    #endregion

}