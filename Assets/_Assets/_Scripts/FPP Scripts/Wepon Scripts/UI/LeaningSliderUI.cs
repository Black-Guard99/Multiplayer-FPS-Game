using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LeaningSliderUI : MonoBehaviour,IPointerDownHandler ,IPointerUpHandler{
    [SerializeField] private Loadout loadout;
    [SerializeField] private Slider leaningSlider;
    private bool IsPointerDown = false;
    public void Leaning(float value){
        float currentLeanValue = value * 30f;
        Debug.Log("Leaning Value " + currentLeanValue);
        loadout.Lean(currentLeanValue);
    }
    private void Update(){
        if(!IsPointerDown){
            if(leaningSlider.value != 0){
                leaningSlider.value = Mathf.Lerp(leaningSlider.value, 0f, 10f * Time.deltaTime);
                loadout.Lean(leaningSlider.value);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        // float currentLeanValue = value * 30f;
        Debug.Log("ON Pointer Up");
        IsPointerDown = false;
        
    }

    public void OnPointerDown(PointerEventData eventData) {
        IsPointerDown = true;
    }
}