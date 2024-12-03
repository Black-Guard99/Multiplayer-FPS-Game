using UnityEngine;
public class BulletCasing : MonoBehaviour, IPooledObject {
    [SerializeField] private float casingSpawnForce = 2.4f;
    private Rigidbody rb;
    private Transform startingParent;
    private void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    public void DestroyMySelfWithDelay(float delay = 0) {
        CancelInvoke(nameof(DestroyNow));
        Invoke(nameof(DestroyNow),delay);
    }

    public void DestroyNow(){
        rb.isKinematic = true;
        transform.SetParent(startingParent);
        CancelInvoke(nameof(DestroyNow));
        gameObject.SetActive(false);
    }

    public void OnObjectReuse() {
        gameObject.SetActive(true);
        rb.isKinematic = true;
        DestroyMySelfWithDelay(3f);
    }
    public void Ejject(Vector3 direction){
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(direction + Vector3.up * casingSpawnForce,ForceMode.Impulse);
        rb.AddTorque(Vector3.up * 5f,ForceMode.Impulse);
    }

    public void SetStartinParent(Transform _parent) {
        startingParent = _parent;
    }
}