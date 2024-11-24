using Photon.Pun;
using UnityEngine;

public class RagdollSystem : MonoBehaviour,IPooledObject {
    [SerializeField] private Collider[] collidersArray;
    [SerializeField] private Rigidbody[] bodyRigidBodyArray;
    private Animator animator;
    private bool isDead;

    private void Awake(){
        animator = GetComponent<Animator>();
    }

    public void DeactiveRagDoll(){
        for (int i = 0; i < collidersArray.Length; i++) {
            if(collidersArray[i].TryGetComponent(out HealthSystem healthSystem)){
                healthSystem.ResetHealth();
            }
        }
        animator.enabled = true;
        foreach (Collider bodyCols in collidersArray) {
            bodyCols.isTrigger = true;
        }
        if(bodyRigidBodyArray.Length > 0){
            foreach(Rigidbody bodyRb in bodyRigidBodyArray){
                bodyRb.isKinematic = true;
            }
        }
        isDead = false;
    }
    public void ActivateRagDoll(){
        if(!isDead){
            animator.enabled = false;
            if(bodyRigidBodyArray.Length > 0){
                foreach(Rigidbody bodyRb in bodyRigidBodyArray){
                    bodyRb.isKinematic = false;
                    bodyRb.AddForce(Vector3.up * 8f,ForceMode.Force);
                }
            }
            foreach (Collider bodyCols in collidersArray) {
                if(!bodyCols.enabled)bodyCols.enabled = true;
                bodyCols.isTrigger = false;   
            }
            Debug.Log("Ragdoll activated");
            DestroyMySelfWithDelay(5f);
            isDead = true;
        }
    }

    // Pooling Methods..............
    private Transform _StartingParent;

    public void SetStartinParent(Transform _parent) {
        _StartingParent = _parent;
    }

    public void OnObjectReuse() {
        gameObject.SetActive(true);
        CancelInvoke(nameof(ActivateRagDoll));
        Invoke(nameof(ActivateRagDoll),.5f);
    }

    public void DestroyMySelfWithDelay(float delay = 0) {
        CancelInvoke(nameof(DestroyNow));
        Invoke(nameof(DestroyNow),delay);
    }

    public void DestroyNow() {
        CancelInvoke(nameof(DestroyNow));
        DeactiveRagDoll();
        // gameObject.SetActive(false);
        PhotonNetwork.Destroy(gameObject);
    }
}