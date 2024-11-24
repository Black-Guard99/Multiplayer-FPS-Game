using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Button))]
public class UIButtonToggles : MonoBehaviour {
    
    [SerializeField] private Color enableColor;
    [SerializeField] private Color desableColor;


    [SerializeField] private Button graphics;

    private void Awake(){
        if(graphics == null){
            graphics = GetComponent<Button>();
        }
    }
    public void Toggle(bool on){

        if(on){
            graphics.targetGraphic.color = enableColor;
        }else{
            graphics.targetGraphic.color = desableColor;
        }
    }
    
}
