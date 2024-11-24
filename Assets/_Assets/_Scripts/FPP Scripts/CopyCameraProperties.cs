using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CopyCameraProperties : MonoBehaviour {
    [SerializeField] private Camera targetCam,currentCam;
    private void Update(){
        currentCam.fieldOfView = targetCam.fieldOfView;
        currentCam.enabled = targetCam.enabled;
    }
}