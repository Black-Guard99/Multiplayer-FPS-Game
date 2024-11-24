using UnityEditor;
using UnityEngine;

// [CreateAssetMenu(menuName = "Configs/StatSO", fileName = "StatSO")]
public class StatSO : ScriptableObject {
    public enum StatType{
        Damage,Accuracy,FireRate,Range,Ammo,Recoil,
    }
    public StatType statType;
    public float maxStat;
    public float minStat;

    public float currentStatAmount;
    public float normalized;
    public void SetStat(float statamount){
        currentStatAmount = Mathf.Clamp(statamount,minStat,maxStat);
    }
    private void OnValidate(){
        if(currentStatAmount >= maxStat){
            currentStatAmount = maxStat;
        }
        if(currentStatAmount <= minStat){
            currentStatAmount = minStat;
        }
        normalized = GetStateNormalizedValue();
    }

    public float GetStateNormalizedValue(){
        float value = (currentStatAmount - minStat) / (maxStat - minStat) * 100;
        return value / 100f;
    }
}