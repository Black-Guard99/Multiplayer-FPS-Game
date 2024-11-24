using UnityEngine;

public class MainMenu : MonoBehaviour {
    public Launcher launcher;

    private void Start() {
        Pause.paused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void JoinMatchRandom() {
        // launcher.JoinRandom();
    }

    public void CreateMatch() {
        launcher.Create();
    }

    public void QuitGame() {
        Application.Quit();
    }
    
}