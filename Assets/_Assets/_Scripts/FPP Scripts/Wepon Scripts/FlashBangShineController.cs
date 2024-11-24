using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class FlashBangShineController : MonoBehaviour {
    [SerializeField] private Volume flashBangShine;
    [SerializeField] private float MaxShineEffect = 2f;
    private float currentShineTime;
    private bool startDecreasingShine;
    private void Start(){
        currentShineTime = 0;
    }
    public void StartShine(){
        flashBangShine.weight = 1f;
        currentShineTime = MaxShineEffect;
        startDecreasingShine = false;
        CancelInvoke(nameof(RemoveShine));
        Invoke(nameof(RemoveShine),5f);
    }
    private void RemoveShine(){
        startDecreasingShine = true;
    }
    private void Update(){
        if(currentShineTime > 0){
            currentShineTime -= Time.deltaTime;
            float shineAmountNormalized = currentShineTime / MaxShineEffect;
            flashBangShine.weight = shineAmountNormalized;
        }
    }


}