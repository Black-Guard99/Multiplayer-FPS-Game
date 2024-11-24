using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class HitDirectionIndicator : MonoBehaviour,IPooledObject {
    [SerializeField] private Image arrowImage;
    private Vector3 hitSourceDirection,playerPos,playerForward;
    private Transform _StartingParent;

    public void SetStartinParent(Transform _parent) {
        _StartingParent = _parent;
    }

    public virtual void OnObjectReuse(){
        gameObject.SetActive(true);
    }

    public virtual void DestroyMySelfWithDelay(float delay = 0){
        CancelInvoke(nameof(DestroyNow));
        Invoke(nameof(DestroyNow),delay);
    }

    public virtual void DestroyNow(){
        CancelInvoke(nameof(DestroyNow));
        transform.SetParent(_StartingParent);
        gameObject.SetActive(false);
    }
    private void Update(){
        if(arrowImage.color.a > 0){
            Color newColor = arrowImage.color;
            newColor.a -= Time.deltaTime * .8f;
            if(newColor.a <= 0){
                newColor.a = 0f;
                DestroyNow();
            }
            arrowImage.color = newColor;
        }
        Vector3 _direction = playerPos - hitSourceDirection;
        Quaternion _sourceRot = Quaternion.LookRotation(_direction);
        _sourceRot.z = -_sourceRot.y;
        _sourceRot.x = _sourceRot.y = 0;
        Vector3 northDireciton = new Vector3(0,0,playerForward.y);
        transform.localRotation = _sourceRot * Quaternion.Euler(northDireciton);
    }

    public void SetDamageIndicatorDirection(Vector3 hitSourceDirection,Vector3 playerPos,Vector3 playerForward){
       this.hitSourceDirection = hitSourceDirection;
       this.playerPos = playerPos;
       this.playerForward = playerForward;
    }

    
}