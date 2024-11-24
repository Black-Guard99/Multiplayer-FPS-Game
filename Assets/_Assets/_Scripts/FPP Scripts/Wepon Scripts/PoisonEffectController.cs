using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PoisonEffectController : MonoBehaviour {
    [SerializeField] private Volume flashBangShine;
    [SerializeField] private float MaxPoisonEffect = 2f;
    private float currentEffect;
    private bool startDecreasingShine;
    private void Start(){
        currentEffect = 0;
    }
    public void ShowPoison(){
        flashBangShine.weight = 1f;
        currentEffect = MaxPoisonEffect;
        startDecreasingShine = false;
        CancelInvoke(nameof(RemovePoison));
        Invoke(nameof(RemovePoison),5f);
    }
    private void RemovePoison(){
        startDecreasingShine = true;
    }
    private void Update(){
        if(currentEffect > 0){
            currentEffect -= Time.deltaTime;
            float shineAmountNormalized = currentEffect / MaxPoisonEffect;
            flashBangShine.weight = shineAmountNormalized;
        }
    }
}