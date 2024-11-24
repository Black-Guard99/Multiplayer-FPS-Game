using UnityEngine;

public class OptionsMenuUI : MonoBehaviour {
    [SerializeField] private GameObject optionsMenuScroll;
    private bool isOptionVisible;
    private void Start(){
        optionsMenuScroll.SetActive(false);
    }
    public void ToggleOptionMenu(){
        isOptionVisible = !isOptionVisible;
        optionsMenuScroll.SetActive(isOptionVisible);
    }

}