using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class MovementControllerUIBtn : MonoBehaviour{
    [SerializeField] private NewPlayerMovement newPlayerMovement;
    
    public void Jump(){
        newPlayerMovement.JumpPressUI();
    }
    public void Crouch(){
        newPlayerMovement.ToggleCrouchUI();
    }
    
}