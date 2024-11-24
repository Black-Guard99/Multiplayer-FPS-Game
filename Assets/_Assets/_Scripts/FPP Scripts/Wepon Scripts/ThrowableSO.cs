using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/ThrowableSO", fileName = "ThrowableSO")]
public class ThrowableSO : GunSO {
    public enum ThrowableType{
        Grenade,FlashBang,SmokeGrenade,PoisonGrenade,Molotove
    }
    public float mass,drag;
    public float maxCookingTime = 5f,force = 20f;
    public ThrowableObject throwable;
    public ThrowableType currentThrowableType;
}