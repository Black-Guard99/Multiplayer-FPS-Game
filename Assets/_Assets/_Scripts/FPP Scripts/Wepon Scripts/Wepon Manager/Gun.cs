using System;
using Cinemachine;
using Photon.Pun;
using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class Gun : MonoBehaviour {
	public enum WeponPositions{
		Primary,Secondary,Mele,LethalThrowable,NonLethalThrowable,
	}
	[SerializeField] private GunSO gun;
	[SerializeField] private CinemachineVirtualCamera nonaimCamera,aimCamera;
	[SerializeField] private GameObject model;
	[SerializeField] private ParticleSystem shootSytem;
	[SerializeField] private WeponAnimationManager animationManager;
	[SerializeField] private WeponPositions weponType;
	[SerializeField] private ThrowableSystem throwableSystem;
	[SerializeField] private LineRenderer gunTrail;
	[SerializeField] private List<BodyPartsHitTarget> ownerBodyPartsList;
	[SerializeField]private CinemachineScreenShakeManager screenShakeaiming,screenShakeNonAim;
	[SerializeField] private WeaponSoundSystem weaponSoundSystem;
	private float lastShootTime;
	private bool aiming;	
	private bool canReloadAnytime;
	private Vector2 recoilOffset;
	private ProfileData playerProfile;
	private bool CanUseWepon;
    private bool isMine;
	// ^Actions.........
	public Action OnAfterReloadComplete;
	public Action OnShoot,OnMagEmpty;
	public Action<bool> onAim;
    private Vector2 recoilVelocityRef;

    // ^ Getters...........

    public Gun.WeponPositions GetWeponType{
		get{
			return weponType;
		}
	}
	
	public ShootConfig GetShootConfig{
		get{
			return gun.shootConfig;
		}
	}
	public ProfileData GetplayerData{
		get{
			return gun.playerProfile;
		}
	}
	public int GetCurrentBulletCount{
		get{
			return gun.ammoConfig.currentClipAmmo;
		}
	}
	public int GetMaxBulletCount{
		get{
			return gun.ammoConfig.currentMaxAmmoo;
		}
	}
	public Sprite GetGunSprite{
		get{
			return gun.weponUiIcon;
		}
	}

	public string GetGunName{
		get{
			return gun.name;
		}
	}
	public bool HasAmmoo{
		get{
			return gun.ammoConfig.currentMaxAmmoo > 0;
		}
	}
	public float GetReloadingTime{
		get{
			return animationManager.GetReloadingTime();
		}
	}
	public bool IsReloading { get; private set; }
	public bool CanUse{
		get{
			return CanUseWepon;
		}
	}
	public void SetCanUseWepon(bool value){
		CanUseWepon = value;
	}
	public void SetPlayerProfile(ProfileData profileData){
		this.playerProfile = profileData;
		gun.playerProfile = playerProfile;
	}
	public void SetUp(bool _isMine,List<BodyPartsHitTarget> _ownerBodyPartsList,CinemachineVirtualCamera _nonAimCamera,LineRenderer _gunLine,OffScreenIndicator _offScreenIndicator){
		transform.localPosition = gun.spawnPointOffset;
		SetCanUseWepon(false);
		animationManager.OnShellEject += (Transform ejectDirection)=>{
			GameObject casingNew = ObjectPoolingManager.Current.SpawnFromPool(gun.shootConfig.casing.name,ejectDirection.position,ejectDirection.rotation);
			if(casingNew.TryGetComponent(out BulletCasing casingRig)){
				casingRig.Ejject(ejectDirection.right * gun.shootConfig.shellEjectionForce);
			}

		};
		animationManager.OnWeponDrawIn += ()=>{
			SetCanUseWepon(true);
		};
		this.isMine = _isMine;
		this.ownerBodyPartsList = _ownerBodyPartsList;
		this.gunTrail = _gunLine;
		this.nonaimCamera = _nonAimCamera;
		if(this.nonaimCamera != null){
			if(this.nonaimCamera.TryGetComponent(out CinemachineScreenShakeManager nonAimShake)){
				screenShakeNonAim = nonAimShake;
			}
		}
		if(this.aimCamera != null){
			if(aimCamera.TryGetComponent(out CinemachineScreenShakeManager aimShake)){
				screenShakeaiming = aimShake;
			}
		}
		SetTrailValue();
		switch(gun.gunType){
			case GunType.Shooting:
			// aimCamera.enabled = false;
			gun.ammoConfig.currentClipAmmo = gun.ammoConfig.clipSize;
			gun.ammoConfig.currentMaxAmmoo = gun.ammoConfig.maxAmmo;

			lastShootTime = 0;
			if(this.isMine){
				canReloadAnytime = true;
				// CrossHairMovmenet.Current.ShowHideCrossHairWindow(true);
			}else{
				// CrossHairMovmenet.Current.ShowHideCrossHairWindow(false);
				aimCamera.enabled = false;
				nonaimCamera.enabled = false;
			}
			animationManager.OnAfterReloadComplete += ()=>{
				gun.ammoConfig.Reload();
				IsReloading = false;
				OnAfterReloadComplete?.Invoke();
				canReloadAnytime = true;
				if(aiming){
					aimCamera.enabled = aiming;
					nonaimCamera.enabled = !aiming;
				}
			};
			break;
			case GunType.LethealThrowable:
			case GunType.NonLethanThrowable:
				if(throwableSystem != null){
					throwableSystem.Init(this.nonaimCamera.transform,_offScreenIndicator);
				}
				gun.ammoConfig.currentClipAmmo = gun.ammoConfig.clipSize;
				gun.ammoConfig.currentMaxAmmoo = gun.ammoConfig.maxAmmo;
				gun.ammoConfig.Reload();
				lastShootTime = 0;
				if(this.isMine){
					canReloadAnytime = true;
					// CrossHairMovmenet.Current.ShowHideCrossHairWindow(true);
				}else{
					// CrossHairMovmenet.Current.ShowHideCrossHairWindow(false);
					nonaimCamera.enabled = false;
					// aimCamera.enabled = false;
				}
			break;
		}
	}
	
	private void ChangeLayerRecursively(Transform p_trans, int p_layer) {
		p_trans.gameObject.layer = p_layer;
		foreach (Transform t in p_trans) ChangeLayerRecursively(t, p_layer);
	}

    private void LateUpdate(){
		if(recoilOffset != Vector2.zero){
			recoilOffset = Vector2.SmoothDamp(recoilOffset,Vector2.zero,ref recoilVelocityRef,10f * Time.deltaTime);
		}
	}

    private void SetTrailValue(){
		if(gunTrail != null){
			if(gun.gunType != GunType.Shooting) return;
			gunTrail.colorGradient = gun.trailConfig.colorGradient;
			gunTrail.material = gun.trailConfig.material;
			gunTrail.widthCurve = gun.trailConfig.widthCurve;
			gunTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			gunTrail.positionCount = 2;
		}
	}
	public void ShowHideTrail(bool showTrail){
		if(gunTrail != null){
			if(!isMine){
				if(gun.gunType == GunType.Shooting){
					gunTrail.gameObject.SetActive(showTrail);
					if(showTrail){
						SetTrailValue();
						Vector3 bulletMoveDir;
						Ray shootingCameraRay;
						if(aiming){
							shootingCameraRay = new Ray(aimCamera.transform.position,aimCamera.transform.forward);
						}else{
							shootingCameraRay = new Ray(nonaimCamera.transform.position,nonaimCamera.transform.forward);
						}
						if(Physics.Raycast(shootingCameraRay,out RaycastHit CheckHit,gun.shootConfig.shootRange.currentStatAmount,gun.shootConfig.hitMask)){
							bulletMoveDir = CheckHit.point;
						}else{
							bulletMoveDir = shootingCameraRay.GetPoint(gun.shootConfig.shootRange.currentStatAmount);
						}
						gunTrail.SetPosition(0,shootSytem.transform.position);
						gunTrail.SetPosition(1,bulletMoveDir);
					}
				}
			}else{
				gunTrail.gameObject.SetActive(false);
			}
		}
	}
    public void Tap(){
		if(!CanUseWepon) return;
		gun.playerProfile.username = PhotonNetwork.NickName;
		gun.playerProfile.gunName = gun.name;
		switch(gun.gunType){
			case GunType.Shooting:
				if(IsReloading){
					return;
				}
				if(gun.ammoConfig.currentClipAmmo <= 0) {
					StartReloading();
					recoilOffset = Vector2.zero;
					OnMagEmpty?.Invoke();
					return;
				}
				if(gun.shootConfig.gunShootingMode == ShootConfig.GunShootingMode.Burst){
					BurstShoot();
				}else{
					SingleShoot();
				}
			break;
			case GunType.LethealThrowable:
			case GunType.NonLethanThrowable:
				if(gun.ammoConfig.currentClipAmmo > 0){
					if(Time.time > lastShootTime + gun.shootConfig.fireRate.currentStatAmount){
						lastShootTime = Time.time;
						throwableSystem.CookThrowable();
					}
				}else{
					SetCanUseWepon(false);
				}
			break;
			case GunType.Knife:
				if(gun.shootConfig.gunShootingMode == ShootConfig.GunShootingMode.Tap){
					UseMele();
				}
			break;
			

		}
	}
	public void HoldDown(){
		if(!CanUseWepon) return;
		gun.playerProfile.username = PhotonNetwork.NickName;
		gun.playerProfile.gunName = gun.name;
		switch(gun.gunType){
			case GunType.Shooting:
				if(IsReloading){
					return;
				}
				if(gun.ammoConfig.currentClipAmmo <= 0) {
					StartReloading();
					OnMagEmpty?.Invoke();
					return;
				}
				if(gun.shootConfig.gunShootingMode == ShootConfig.GunShootingMode.Burst){
					BurstShoot();
				}else{
					SingleShoot();
				}
			break;
		}
	}
	public void TapRelease(){
		recoilOffset = Vector2.zero;
		if(!CanUseWepon) return;
		gun.playerProfile.username = PhotonNetwork.NickName;
		gun.playerProfile.gunName = gun.name;
		switch(gun.gunType){
			case GunType.LethealThrowable:
			case GunType.NonLethanThrowable:
				// Throw.
				
				if(gun.ammoConfig.currentClipAmmo <= 0){
					return;
				}
				throwableSystem.Throw(OnThrowComplete_ThrowingSystem);
			break;
		}
	}
	private void UseMele(){
		if(Time.time > gun.shootConfig.fireRate.currentStatAmount + lastShootTime){
			lastShootTime = Time.time;
			animationManager.Shoot();
			OnShoot?.Invoke();
		}
	}
	public void PlayShootAnims(){
		animationManager.Shoot();
	}
	
	
	public void ToggleAim(bool aim){
		if(isMine){
			if(gun.shootConfig.canAim){
				aiming = aim;
				onAim?.Invoke(aiming);
				if(aimCamera != null){
					aimCamera.enabled = aiming;
				}
				if(nonaimCamera != null){
					nonaimCamera.enabled = !aiming;
				}
				CrossHairMovement.Current.ShowHideCrossHairWindow(!aiming);
				animationManager.Aim(aiming);
			}
		}else{
			animationManager.Aim(aim);
		}
	}
	private void BurstShoot(){
		if(Time.time > lastShootTime + gun.shootConfig.fireRate.currentStatAmount){
			Vector3 bulletMoveDirection;
			lastShootTime = Time.time;
			if(weaponSoundSystem != null){
				weaponSoundSystem.PlayFiringSound();
			}
			Ray shootingCameraRay;
			if(aiming){
				screenShakeaiming.Shake(gun.shootConfig.aimScreenShakeProperties);// CameraShaking for Aiming....
				shootingCameraRay = new Ray(aimCamera.transform.position,aimCamera.transform.forward);
			}else{
				screenShakeNonAim.Shake(gun.shootConfig.nonScreenShakeProperties);// Camera Shaking for NonAim..
				shootingCameraRay = new Ray(nonaimCamera.transform.position,nonaimCamera.transform.forward);
			}
			if(Physics.Raycast(shootingCameraRay,out RaycastHit CheckHit,gun.shootConfig.shootRange.currentStatAmount,gun.shootConfig.hitMask)){
				bulletMoveDirection = CheckHit.point;
			}else{
				bulletMoveDirection = shootingCameraRay.GetPoint(gun.shootConfig.shootRange.currentStatAmount);
			}
			ShowShootingEffect();
			animationManager.Shoot();
			int burstAmount = Random.Range(4,7);
			for (int i = 0; i < burstAmount; i++) {
				Vector3 spread = gun.shootConfig.spread;
				Vector3 shootDirection = (shootSytem.transform.forward) + 
					GetSpreadAmount(spread) +
						(bulletMoveDirection - shootSytem.transform.position);
				shootDirection.Normalize();
				// *Shooting Bullets
				if(Physics.Raycast(shootSytem.transform.position,shootDirection,out RaycastHit hit,gun.shootConfig.shootRange.currentStatAmount,gun.shootConfig.hitMask)){
					GameObject BulletNew = ObjectPoolingManager.Current.SpawnFromPool(gun.shootConfig.bulletPoolName.name,shootSytem.transform.position,shootSytem.transform.rotation);
					if(BulletNew.TryGetComponent(out RaycastProjectileMovement raycastProjectileMovement)){
						raycastProjectileMovement.Initalized(shootSytem.transform.position,shootDirection,gun.trailConfig.simulationSpeed,gun.shootConfig.gravity,ownerBodyPartsList,gun,shootSytem.transform.position);
					}
				}else{
					GameObject BulletNew = ObjectPoolingManager.Current.SpawnFromPool(gun.shootConfig.bulletPoolName.name,shootSytem.transform.position,shootSytem.transform.rotation);
					if(BulletNew.TryGetComponent(out RaycastProjectileMovement raycastProjectileMovement)){
						raycastProjectileMovement.Initalized(shootSytem.transform.position,shootDirection,gun.trailConfig.simulationSpeed,gun.shootConfig.gravity,ownerBodyPartsList,gun,shootSytem.transform.position);
					}
				}
			}
			recoilOffset += gun.shootConfig.recoilConfig.GetRecoilValue();
			gun.ammoConfig.currentClipAmmo--;
			if(gun.ammoConfig.currentClipAmmo <= 0){
				gun.ammoConfig.currentClipAmmo = 0;
				recoilOffset = Vector2.zero;
				StartReloading();
				OnMagEmpty?.Invoke();
			}
			OnShoot?.Invoke();
		}
	}
	private void SingleShoot(){
		if(Time.time > lastShootTime + gun.shootConfig.fireRate.currentStatAmount){
			Vector3 bulletMoveDirection = Vector3.zero;
			lastShootTime = Time.time;
			if(weaponSoundSystem != null){
				weaponSoundSystem.PlayFiringSound();
			}
			Ray shootingCameraRay;
			if(aiming){
				shootingCameraRay = new Ray(aimCamera.transform.position,aimCamera.transform.forward);
				screenShakeaiming.Shake(gun.shootConfig.aimScreenShakeProperties);// CameraShaking for Aiming....
			}else{
				shootingCameraRay = new Ray(nonaimCamera.transform.position,nonaimCamera.transform.forward);
				screenShakeNonAim.Shake(gun.shootConfig.nonScreenShakeProperties);// Camera Shaking for NonAim..
			}
			if(Physics.Raycast(shootingCameraRay,out RaycastHit CheckHit,gun.shootConfig.shootRange.currentStatAmount,gun.shootConfig.hitMask)){
				bulletMoveDirection = CheckHit.point;
			}else{
				bulletMoveDirection = shootingCameraRay.GetPoint(gun.shootConfig.shootRange.currentStatAmount);
			}
			animationManager.Shoot();
			Vector3 spread = aiming ? Vector3.zero : gun.shootConfig.spread;
			Vector3 shootDirection = (shootSytem.transform.forward) + 
				GetSpreadAmount(spread) +
					(bulletMoveDirection - shootSytem.transform.position);
			shootDirection.Normalize();
			ShowShootingEffect();
			if(Physics.Raycast(shootSytem.transform.position,shootDirection,out RaycastHit hit,Mathf.Infinity,gun.shootConfig.hitMask)){
				GameObject BulletNew = ObjectPoolingManager.Current.SpawnFromPool(gun.shootConfig.bulletPoolName.name,shootSytem.transform.position,shootSytem.transform.rotation);
				if(BulletNew.TryGetComponent(out RaycastProjectileMovement raycastProjectileMovement)){
					raycastProjectileMovement.Initalized(shootSytem.transform.position,shootDirection,gun.trailConfig.simulationSpeed,gun.shootConfig.gravity,ownerBodyPartsList,gun,shootSytem.transform.position);
				}
			}else{
				GameObject BulletNew = ObjectPoolingManager.Current.SpawnFromPool(gun.shootConfig.bulletPoolName.name,shootSytem.transform.position,shootSytem.transform.rotation);
				if(BulletNew.TryGetComponent(out RaycastProjectileMovement raycastProjectileMovement)){
					raycastProjectileMovement.Initalized(shootSytem.transform.position,shootDirection,gun.trailConfig.simulationSpeed,gun.shootConfig.gravity,ownerBodyPartsList,gun,shootSytem.transform.position);
				}
			}
			recoilOffset += gun.shootConfig.recoilConfig.GetRecoilValue();
			gun.ammoConfig.currentClipAmmo--;
			if(gun.ammoConfig.currentClipAmmo <= 0){
				gun.ammoConfig.currentClipAmmo = 0;
				recoilOffset = Vector2.zero;
				StartReloading();
				OnMagEmpty?.Invoke();
			}
			OnShoot?.Invoke();
		}
	}
	/* 
	private void LateUpdate(){
		if(recoilOffset != Vector2.zero){
			recoilOffset = Vector2.Lerp(recoilOffset,Vector2.zero,10f * Time.deltaTime);
		} 
	}
	*/
	
	public Vector3 GetRecoilAmount(){
		return recoilOffset;
	}
	public void CancleHold(){
		throwableSystem.CancleHold();
	}
	private void OnThrowComplete_ThrowingSystem(){
		gun.ammoConfig.currentClipAmmo --;
		if(gun.ammoConfig.currentClipAmmo <= 0){
			gun.ammoConfig.currentClipAmmo = 0;
			gun.ammoConfig.Reload();
		}
		OnShoot?.Invoke();
	}
	public float GetHoldTime(){
		return throwableSystem.GetHoldTimerNormalized();
	}

	private void ShowShootingEffect(){
		shootSytem.Play();
	}
	private Vector3 GetSpreadAmount(Vector3 spread){
		return new Vector3(Random.Range(-spread.x,spread.x),
			Random.Range(-spread.y,spread.y),
				Random.Range(-spread.z,spread.z));
	}
	
	public bool IsHitOwner(BodyPartsHitTarget ownerBodyPart){
		return ownerBodyPartsList.Contains(ownerBodyPart);
	}
	/* 
    private IEnumerator PlayTrail(Vector3 startPosition, Vector3 direction, RaycastHit hit,Vector3 targetPoint) {
		TrailRenderer instance = CreatTrail();
		instance.transform.position = startPosition;
		yield return null;
		instance.emitting = true;
		float distance = Vector3.Distance(startPosition,direction);
		float remainingDistance = distance;
		while(remainingDistance > 0){
			instance.transform.position = Vector3.Lerp(startPosition,direction,Mathf.Clamp01(1 - (remainingDistance / distance)));
			remainingDistance -= gun.trailConfig.simulationSpeed * Time.deltaTime;
			yield return null;
		}
		instance.transform.position = direction;
		if(hit.collider != null){
			Debug.Log("Hit To " + hit.transform.name);
			if(hit.transform.TryGetComponent(out ITarget target)){
				if(hit.transform.TryGetComponent(out BodyPartsHitTarget bodyPart)){
					if(!IsHitOwner(bodyPart)){
						// float damageValue = gun.shootConfig.damageConfig.GetDamageValue(Vector3.Distance(startPosition,hit.point));
						float damageValue = gun.shootConfig.damageConfig.bodyDamageAmount;
						target.TakeHit(damageValue,hit.point,PhotonNetwork.LocalPlayer.ActorNumber,shootSytem.transform.position,gun.playerProfile.username,gun.playerProfile.gunName);
					}
				}else{
					float damageValue = gun.shootConfig.damageConfig.bodyDamageAmount;
					target.TakeHit(damageValue,hit.point,PhotonNetwork.LocalPlayer.ActorNumber,shootSytem.transform.position,gun.playerProfile.username,gun.playerProfile.gunName);
				}
			}else{
				ObjectPoolingManager.Current.SpawnEffectPool("HitEffect",hit.point,Quaternion.identity);
				// *Play Impact Effect.
				CheckNonLivingTarget(hit.point,direction);
			}
		}else{
			// Debug.Log("Hit None.");
			CheckNonLivingTarget(targetPoint,direction);
		}
		yield return new WaitForSeconds(gun.trailConfig.duration);
		yield return null;
		instance.emitting = false;
		RemovePooledObject(instance.gameObject);
    }
	private void RemovePooledObject(GameObject instance){
		if(instance.TryGetComponent(out IPooledObject pooledObject)){
			pooledObject.DestroyNow();
		}
	}

	 */
	
	private void CheckNonLivingTarget(Vector3 hitPoint,Vector3 direction){
		Collider[] colliders = Physics.OverlapSphere(hitPoint,1f,gun.shootConfig.hitMask,QueryTriggerInteraction.UseGlobal);
		foreach (Collider colis in colliders) {
			if(colis.transform.TryGetComponent(out ITarget target)){
				if(colis.transform.TryGetComponent(out BodyPartsHitTarget hitTarget)){
					if(!IsHitOwner(hitTarget)){
						// float damageValue = gun.shootConfig.damageValue.GetDamageValue(hitPoint)
						target.TakeHit(3f,hitPoint,PhotonNetwork.LocalPlayer.ActorNumber,shootSytem.transform.position,gun.playerProfile.username,gun.playerProfile.gunName);
					}
				}
			}
		}
	}
    private TrailRenderer CreatTrail() {
		GameObject instance = ObjectPoolingManager.Current.SpawnFromPool(gun.shootConfig.bulletPoolName.name,Vector3.zero,Quaternion.identity);
		TrailRenderer trail = instance.GetComponent<TrailRenderer>();
		trail.colorGradient = gun.trailConfig.colorGradient;
		trail.material = gun.trailConfig.material;
		trail.widthCurve = gun.trailConfig.widthCurve;
		trail.time = gun.trailConfig.duration;
		trail.minVertexDistance = gun.trailConfig.minVertexDistance;
		trail.emitting = false;
		trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		return trail;
    }

    public void SetDead() {
		aimCamera.enabled = false;
		animationManager.Die();
    }

    public void StartReloading() {
		if(!IsReloading){
			if(gun.ammoConfig.CanReload()){
				if(aiming){
					if(aimCamera != null){
						aimCamera.enabled = !aiming;
					}
					if(nonaimCamera != null){
						nonaimCamera.enabled = aiming;
					}
				}
				onAim?.Invoke(aiming);
				int reloadCount = 1;
				switch(gun.shootConfig.gunShootingMode){
					case ShootConfig.GunShootingMode.Burst:
						if(gun.ammoConfig.currentClipAmmo > 0){
							int differnce = gun.ammoConfig.clipSize - gun.ammoConfig.currentClipAmmo;
							reloadCount = differnce;
						}else{
							reloadCount = gun.ammoConfig.clipSize;
						}
					break;
				}
				IsReloading = true;
				canReloadAnytime = false;
				Debug.Log("ReloadCount "+ reloadCount);
				animationManager.Reload(reloadCount);
			}
		}
    }

    public void CancleReload() {
		if(IsReloading){
        	animationManager.CancleReload();
			IsReloading = false;
			canReloadAnytime = true;
			if(aiming){
				if(aimCamera != null){
					aimCamera.enabled = aiming;
				}
				if(nonaimCamera != null){
					nonaimCamera.enabled = !aiming;
				}
			}
		}
    }

    public void DrawIn() {
        animationManager.DrawIn();
    }

    public void CheckCanPlayReloading() {
		if(!IsReloading){
			int reloadCount = 1;
			switch(gun.shootConfig.gunShootingMode){
				case ShootConfig.GunShootingMode.Burst:
					if(gun.ammoConfig.currentClipAmmo > 0){
						int differnce = gun.ammoConfig.clipSize - gun.ammoConfig.currentClipAmmo;
						reloadCount = differnce;
					}else{
						reloadCount = gun.ammoConfig.clipSize;
					}
				break;
			}
			animationManager.Reload(reloadCount);
		}
    }

    public void SetSpeed(float currentSpeed) {
        animationManager.SetSpeed(currentSpeed);
    }
	
}