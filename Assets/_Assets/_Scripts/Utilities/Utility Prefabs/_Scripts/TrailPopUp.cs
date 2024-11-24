using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrailPopUp : PopUp {
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private float distanceFromCamera = 3f;
    public void SetTrailPosiion(Vector3 touchPosition){
        // touchPosition.z = distanceFromCamera;
        transform.position = touchPosition;
    }
    protected override void OnActive(){
        tr.emitting = true;
    }

    protected override void OnDeactive() {
        tr.emitting = false;
    }
}
