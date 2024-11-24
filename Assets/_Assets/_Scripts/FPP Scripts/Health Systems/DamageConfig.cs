using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(menuName = "Configs/DamageConfig", fileName = "Gun/DamageConfig")]
public class DamageConfig : StatSO {
    public float bodyDamageAmount,headShotDamage;
    public MinMaxCurve damageCurve;
    private void Reset(){
        damageCurve.mode = ParticleSystemCurveMode.Curve;
    }
    public float GetDamageValue(float distance = 2){
        return damageCurve.Evaluate(distance,Random.value);
    }
}