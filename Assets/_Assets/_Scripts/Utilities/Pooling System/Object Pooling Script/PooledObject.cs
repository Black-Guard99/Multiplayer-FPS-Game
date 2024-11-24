using Photon.Pun;
using UnityEngine;
namespace GamerWolf.Utils {
    public class PooledObject : MonoBehaviour, IPooledObject{

        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private bool dontDestoryAfterDelay;
        [SerializeField] private bool isOnNetwork;
        private ParticleSystem currentParticalSystem;
        private void Awake(){
            currentParticalSystem = GetComponent<ParticleSystem>();
        }
        private void Start(){
            DestroyMySelfWithDelay(lifeTime);
        }
        public void DestroyMySelfWithDelay(float delay = 0f){
            if(!dontDestoryAfterDelay){
                CancelInvoke(nameof(DestroyNow));
                Invoke(nameof(DestroyNow),delay);
            }
            
        }

        public void OnObjectReuse(){
            float disappearTime = lifeTime;

            if(currentParticalSystem != null){
                currentParticalSystem.Play();
                ParticleSystem.MainModule module = currentParticalSystem.main;
                disappearTime = module.startLifetimeMultiplier;
            }
            DestroyMySelfWithDelay(disappearTime);
        }

        public void DestroyNow(){
            CancelInvoke(nameof(DestroyNow));
            if(isOnNetwork){
                ObjectPoolingManager.Current.Destroy(this.gameObject);
            }else{
                transform.SetParent(startingParent);
                gameObject.SetActive(false);
            }

            // ObjectPoolingManager.Current.DeSpawnObjectRpc(gameObject.name);
        }
#region Pooling Attributes.................................
        
        private Transform startingParent;
        public void SetStartinParent(Transform _parent) {
            startingParent = _parent;
        }

#endregion
    }

}