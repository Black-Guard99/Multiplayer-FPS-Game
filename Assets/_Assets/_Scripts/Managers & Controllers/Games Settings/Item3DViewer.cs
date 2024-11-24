using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class Item3DViewer : MonoBehaviour{
    [SerializeField] private bool onPc = true;
    [SerializeField] private Transform itemToDrag;
    [SerializeField] private float rotationXmin = -7f;
    [SerializeField] private float rotationXmax = +10f;
    [SerializeField] private float rotateSpeed = .2f;
    private Vector3 lastTouchPoint;

    private void Update(){
        if(onPc){
            PerformPc();
        }else{
            PerformMobile();
        }
        
    }

    private void PerformMobile() {
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            if(!EventSystem.current.IsPointerOverGameObject(touch.fingerId)){
                switch(touch.phase){
                    case TouchPhase.Began:
                        lastTouchPoint = touch.position;
                    break;
                    case TouchPhase.Moved:
                        Vector3 mouseDelta = (Vector2)lastTouchPoint - touch.position;
                        mouseDelta.y = Math.Clamp(mouseDelta.y,-200,200);
                        mouseDelta.x = Math.Clamp(mouseDelta.x,-200,200);
                        rotateSpeed = .2f;
                        itemToDrag.localEulerAngles += new Vector3(mouseDelta.y,-mouseDelta.x,0f) * rotateSpeed;
                        float eulerAnglesX = itemToDrag.localEulerAngles.x;
                        if(eulerAnglesX > 180){
                            eulerAnglesX -= 360f;
                        }
                        float rotationX = Mathf.Clamp(eulerAnglesX,rotationXmin,rotationXmax);

                        itemToDrag.localEulerAngles = new Vector3(rotationX,itemToDrag.localEulerAngles.y,itemToDrag.localEulerAngles.z);
                        lastTouchPoint = touch.position;
                    break;
                    case TouchPhase.Ended:
                        lastTouchPoint = itemToDrag.eulerAngles;
                    break;
                }
            }
        }
    }

    private void PerformPc(){
        if(Input.GetMouseButtonDown(0)){
            lastTouchPoint = Input.mousePosition;
        }
        if(Input.GetMouseButton(0)){
            Vector3 mouseDelta = lastTouchPoint - Input.mousePosition;
            mouseDelta.y = Math.Clamp(mouseDelta.y,-200,200);
            mouseDelta.x = Math.Clamp(mouseDelta.x,-200,200);
            rotateSpeed = .2f;
            itemToDrag.localEulerAngles += new Vector3(mouseDelta.y,-mouseDelta.x,0f) * rotateSpeed;
            float eulerAnglesX = itemToDrag.localEulerAngles.x;
            if(eulerAnglesX > 180){
                eulerAnglesX -= 360f;
            }
            float rotationX = Mathf.Clamp(eulerAnglesX,rotationXmin,rotationXmax);

            itemToDrag.localEulerAngles = new Vector3(rotationX,itemToDrag.localEulerAngles.y,itemToDrag.localEulerAngles.z);
            lastTouchPoint = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(0)){
            lastTouchPoint = itemToDrag.eulerAngles;
        }
    }
}