using UnityEngine;
using Photon.Pun;
public class ThrowableObject : MonoBehaviour {
    [SerializeField] private GameObject effect;
    [SerializeField] private Target target;
    [SerializeField] private GameObject visual;
    private OffScreenIndicator offScreenIndicator;
    protected Rigidbody rb;
    protected float currentTime;

    protected virtual void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    public virtual void SetCookedTime(float time){
        currentTime = time;
        DestroyMySelfWithDelay(currentTime);
    }
    public virtual void Throw(float throwForce,Vector3 forceDirection,float time,float drag,float mass,OffScreenIndicator offScreenIndicator){
        rb.mass = mass;
        rb.linearDamping = drag;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(forceDirection * throwForce,ForceMode.Impulse);
        rb.AddTorque(transform.right * throwForce,ForceMode.Impulse);
        SetCookedTime(time);
        this.offScreenIndicator = offScreenIndicator;
        target.SetOffscreenIndicator(this.offScreenIndicator);
    }

    public virtual void OnObjectReuse(){
        gameObject.SetActive(true);
    }

    public virtual void DestroyMySelfWithDelay(float delay = 0){
        CancelInvoke(nameof(DestroyNow));
        Invoke(nameof(DestroyNow),delay);
    }
    public void ShowHideVisual(bool show){
        if(visual != null){
            visual.SetActive(show);
        }
        if(!show){
            target.OnObjectDissappear();
        }
    }
    public void ShowHideEffect(bool show){
        if(effect != null){
            effect.SetActive(show);
        }
    }

    public virtual void DestroyNow(){
        CancelInvoke(nameof(DestroyNow));
        target.OnObjectDissappear();
        PhotonNetwork.Destroy(gameObject);
    }
}