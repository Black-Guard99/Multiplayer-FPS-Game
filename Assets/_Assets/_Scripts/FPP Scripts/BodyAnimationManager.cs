using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BodyAnimationManager : MonoBehaviour {
    [SerializeField] private Animator bodyAnimator;
    public void SetCrouch(bool crouch){
        bodyAnimator.SetBool("Crouch",crouch);
    }
    public void SetSpeed(float speedX,float speedY){
        bodyAnimator.SetFloat("Speed",speedY);
        bodyAnimator.SetFloat("SpeedX",speedX);
    }
    public void Jump(bool jump){
        bodyAnimator.SetBool("IsJumping",jump);
    }
    public void SetSlide(bool slide) {
        bodyAnimator.SetBool("Slide",slide);
    }
}