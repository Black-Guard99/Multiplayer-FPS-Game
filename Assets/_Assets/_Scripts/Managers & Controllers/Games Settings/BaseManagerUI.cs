using UnityEngine;
public class BaseManagerUI : MonoBehaviour {

    [SerializeField] private GameObject equpmentWindow,matchSettingView,lobbyCanvas,baseWindow;
    [SerializeField] private GameObject dummyPlayerBody;
    private void Awake(){
        ShowBase();
    }
    private void HideWindows(){
        dummyPlayerBody.SetActive(false);
        equpmentWindow.SetActive(false);
        matchSettingView.SetActive(false);
        lobbyCanvas.SetActive(false);
        baseWindow.SetActive(false);
    }
    public void ShowBase(){
        HideWindows();
        dummyPlayerBody.SetActive(true);
        baseWindow.SetActive(true);
    }
    public void OpenEquipment(){
        HideWindows();
        equpmentWindow.SetActive(true);
    }
    public void OpenMatchSettings(){
        HideWindows();
        matchSettingView.SetActive(true);
        dummyPlayerBody.SetActive(false);
    }
}