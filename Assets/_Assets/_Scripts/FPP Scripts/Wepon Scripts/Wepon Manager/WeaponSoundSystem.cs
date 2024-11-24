using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
public class WeaponSoundSystem : MonoBehaviour {
    [SerializeField] private AudioSource gunAudioSource;
    [SerializeField] private AudioClip gunFiringClip;
    [SerializeField] private AudioClip insertShellSound;
    [SerializeField] private AudioClip[] reloadingSoundClip;
    private int currentReloadClipIndex;

    public void PlayFiringSound(){
        gunAudioSource.PlayOneShot(gunFiringClip);
    }
    private void RelaodSound1(){
        currentReloadClipIndex++;
        if(currentReloadClipIndex > reloadingSoundClip.Length - 1){
            currentReloadClipIndex = 0;
        }
        gunAudioSource.PlayOneShot(reloadingSoundClip[currentReloadClipIndex]);
    }
    private void InsertShellSound(){
        gunAudioSource.PlayOneShot(insertShellSound);
    }
}