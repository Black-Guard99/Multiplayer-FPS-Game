using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {
    public static bool paused = false;
    private bool disconnecting = false;

    public void TogglePause()
    {
        if (disconnecting) return;

        paused = !paused;

        transform.GetChild(0).gameObject.SetActive(paused);
        Cursor.lockState = (paused) ? CursorLockMode.None : CursorLockMode.Confined;
        Cursor.visible = paused;
    }

    public void Quit()
    {
        disconnecting = true;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}