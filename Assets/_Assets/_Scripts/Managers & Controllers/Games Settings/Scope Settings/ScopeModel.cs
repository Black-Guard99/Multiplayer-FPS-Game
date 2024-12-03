using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
public class ScopeModel : MonoBehaviour {
    [SerializeField] private Transform lookAimPoint;
    [SerializeField] private Camera scopeCam;
    [SerializeField] private ScopeSO scope;
    [SerializeField] private GameObject zoomSlider;
    [SerializeField] private Slider zoom;
    [SerializeField] private CinemachineVirtualCamera aimCamera;
    [SerializeField] private Transform aimLookAtPoint;
    private float currentZoomValue;
    public void SetScopeActive(){
        switch(scope.scopeType){
            case ScopeSO.ScopeType.x3Power:
            case ScopeSO.ScopeType.x6Power:
            case ScopeSO.ScopeType.x8Power:
            case ScopeSO.ScopeType.RedDot:
            case ScopeSO.ScopeType.HoloGraphic:
                currentZoomValue = scope.defultScopeZoom;
                zoom.minValue = scope.defultScopeZoom;
                zoom.maxValue = scope.maxZoom;
                zoom.value = scope.defultScopeZoom;
                scopeCam.fieldOfView = currentZoomValue;
            break;

        }
        lookAimPoint.position = aimLookAtPoint.position;
        aimCamera.LookAt = aimLookAtPoint;
        aimCamera.Follow = aimLookAtPoint;
        aimCamera.m_Lens.FieldOfView = scope.defultAimFov;
        zoomSlider.SetActive(false);
    }
    public void Wepon_OnAim(bool aiming) {
        switch(scope.scopeType){
            case ScopeSO.ScopeType.x6Power:
            case ScopeSO.ScopeType.x8Power:
                scopeCam.gameObject.SetActive(aiming);
                zoomSlider.SetActive(aiming);
            break;
            case ScopeSO.ScopeType.x3Power:
            case ScopeSO.ScopeType.RedDot:
            case ScopeSO.ScopeType.HoloGraphic:
                zoomSlider.SetActive(false);
                scopeCam.gameObject.SetActive(aiming);
            break;
            case ScopeSO.ScopeType.IronSight:
                zoomSlider.SetActive(false);
            break;

        }
    }
    public ScopeSO.ScopeType GetScopeType(){
        return scope.scopeType;
    }
    public void OnZoomChange(float zoomValuePercentage){
        // Calling form Slider... On Value Change....
        currentZoomValue = zoomValuePercentage;
        currentZoomValue = Mathf.Clamp(currentZoomValue,scope.defultScopeZoom,scope.maxZoom);
        scopeCam.fieldOfView = currentZoomValue;
    }
    
}