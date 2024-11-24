using UnityEngine;
using UnityEngine.Rendering;
public class NightVisionSystem : MonoBehaviour {
    [SerializeField] private Volume nightVisionVolume;
    [SerializeField] private GameObject nightVisionLight;
    private bool isNightVisionOn = false;
    private void Awake(){
        isNightVisionOn = false;
        nightVisionLight.SetActive(isNightVisionOn);
        nightVisionVolume.gameObject.SetActive(isNightVisionOn);
    }
    public void ToggleNightVision(){
        isNightVisionOn = !isNightVisionOn;
        nightVisionLight.SetActive(isNightVisionOn);
        nightVisionVolume.gameObject.SetActive(isNightVisionOn);
    }
}