using UnityEngine;
using GamerWolf.Utils;

[CreateAssetMenu(menuName = "Configs/ShootConfig", fileName = "Gun/ShootConfig")]
public class ShootConfig : ScriptableObject {
	public enum GunShootingMode{
        Tap,Holding,Release,Burst
    }
	public bool canAim = true;
	public LayerMask hitMask;
	public Vector3 spread = new Vector3(0.1f,0.2f,0.1f);
	public StatSO shootRange;
	public StatSO fireRate;
	public float gravity = 9.5f;
	public float shellEjectionForce = 5f;
	public PoolSO bulletPoolName,casing;
	public DamageConfig damageConfig;
	public GunShootingMode gunShootingMode;
	public RecoilConfigSO recoilConfig;
	public CinemachineScreenShakePropertiensSO aimScreenShakeProperties,nonScreenShakeProperties;

	


}
