using UnityEngine;

public class LookAtTargt : MonoBehaviour {
    private enum LookingType{
        TowardCamera,
        BackingCamera,
    }
    [SerializeField] private LookingType lookingType;
    private void Update(){
        switch(lookingType){
            case LookingType.TowardCamera:
                transform.forward = -Camera.main.transform.forward;
            break;
            case LookingType.BackingCamera:
                transform.forward = Camera.main.transform.forward;
            break;
        }
    }
}