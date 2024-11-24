using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class AIWeaponAnimationManager : MonoBehaviour {
    [SerializeField] private Animator gunAnimator;
    public void SetSpeed(float speed){
        gunAnimator.SetFloat("Speed",speed);
    }
}