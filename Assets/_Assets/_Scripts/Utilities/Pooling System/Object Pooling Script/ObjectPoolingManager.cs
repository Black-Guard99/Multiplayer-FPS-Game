using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
public class ObjectPoolingManager : MonoBehaviour{
    public static ObjectPoolingManager Current{get; private set;}
    [System.Serializable]
    private class PoolingGroup {
        public string groupName;
        public List<PoolSO> poolingList = new List<PoolSO>();
    }
    [SerializeField] private List<PoolingGroup> poolingGroup = new List<PoolingGroup>();
    [SerializeField] private List<IPooledObject> SpawnedPool;
    private Dictionary<string , Queue<GameObject>> poolDictionary;
    private void Awake(){
        if(Current == null){
            Current = this;
        }else{
            Destroy(gameObject);
        }
    }
    
    public void Init(){
        // photonView.RPC("CreatePool",RpcTarget.All);
        CreatePool();
    }
    // [PunRPC]
    private void CreatePool(){
        spawnedRpcObjectList = new List<GameObject>();
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        SpawnedPool = new List<IPooledObject>();
        foreach (PoolingGroup group in poolingGroup) {
            GameObject pooledGroupParent = new GameObject(group.groupName);
            pooledGroupParent.transform.SetParent(transform);
            if(group.poolingList.Count > 0)    {
                foreach(PoolSO pool in group.poolingList){
                    GameObject pooledObjectsParent = new GameObject("PooledObject = " + pool.name);
                    pooledObjectsParent.transform.SetParent(pooledGroupParent.transform);
                    Queue<GameObject> objectPool = new Queue<GameObject>();
                    for(int i = 0; i < pool.size; i++){
                        if(pool.prefabs != null){
                            GameObject obj = Instantiate(pool.prefabs) as GameObject;
                            obj.SetActive(false);
                            if(pooledGroupParent != null){
                                obj.transform.SetParent(pooledObjectsParent.transform);
                                if(obj.TryGetComponent<IPooledObject>(out IPooledObject pooledObject)){
                                    pooledObject.SetStartinParent(pooledObjectsParent.transform);
                                }
                            }
                            obj.name = string.Concat(pool.name," ",obj.transform.GetSiblingIndex().ToString());
                            objectPool.Enqueue(obj);
                        }
                    }
                    poolDictionary.Add(pool.name,objectPool);
                }
            }
        }
    }
    private string GetRandomTag(string groupName){
        foreach(PoolingGroup poolGroup in poolingGroup){

            if(poolGroup.groupName == groupName){

                int randomNum = Random.Range(0,poolGroup.poolingList.Count);
                return poolGroup.poolingList[randomNum].name;
            }

        }
        return string.Empty;
    }

    public GameObject SpawnRandomFromPool(string groupName,Vector3 _spawnPoint,Quaternion _rotations){
        return SpawnFromPool(GetRandomTag(groupName),_spawnPoint,_rotations);
    }
    public  GameObject SpawnRandomFromPool(string groupName,Vector3 _spawnPoint,Quaternion _rotations,Transform _parent){
        GameObject newPooledObject = SpawnFromPool(GetRandomTag(groupName),_spawnPoint,_rotations);
        newPooledObject.transform.SetParent(_parent);
        return newPooledObject;
    }
    public GameObject SpawnFromPool(string tag){
        return SpawnFromPool(tag,Vector3.zero,Quaternion.identity);
    }
    public GameObject SpawnFromPool(string tag,Transform parent){
        GameObject poolObject = SpawnFromPool(tag,Vector3.zero,Quaternion.identity);
        poolObject.transform.SetParent(parent);
        return poolObject;
    }
    public  GameObject SpawnFromPool(string tag,Vector3 _spawnPosition,Quaternion _rotation,Transform _parent){
        GameObject spawnedObject = SpawnFromPool(tag,_spawnPosition,_rotation);
        spawnedObject.transform.SetParent(_parent);
        return spawnedObject;
    }
    public GameObject SpawnFromPool(string tag,Vector3 _spawnPosition,Quaternion _rotation){
        if(!poolDictionary.ContainsKey(tag)){
            Debug.Log("Pool With the " + tag + " is not Found");
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.transform.position = _spawnPosition;
        objectToSpawn.transform.rotation = _rotation;
        objectToSpawn.SetActive(true);
        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
        if(pooledObject != null){
            pooledObject.OnObjectReuse();
            SpawnedPool.Add(pooledObject);
        }
        
        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
    private List<GameObject> spawnedRpcObjectList;
    public void SpawnObjectRpc(string tag,Vector3 _spawnPosition,Quaternion _rotation){
        // photonView.RPC("GetObject",RpcTarget.All,tag,_spawnPosition,_rotation);
    }
    [PunRPC]
    private void GetObject(string tag,Vector3 _spawnPosition,Quaternion _rotation){
        GameObject currentReturningObject = SpawnFromPool(tag,_spawnPosition,_rotation);
        currentReturningObject.name = tag;
        if(!spawnedRpcObjectList.Contains(currentReturningObject)){
            currentReturningObject.SetActive(true);
            spawnedRpcObjectList.Add(currentReturningObject);
        }
    }
    public void DeSpawnObjectRpc(string tag){
        // photonView.RPC("ReleaseObject",RpcTarget.All,tag);
    }
    [PunRPC]
    private void ReleaseObject(string tag){
        foreach(GameObject currentSpawned in spawnedRpcObjectList){
            if(currentSpawned.name == tag){
                if(spawnedRpcObjectList.Contains(currentSpawned)){
                    currentSpawned.SetActive(false);
                    spawnedRpcObjectList.Remove(currentSpawned);
                }
            }
        }
    }
    public GameObject GetRecentlySpawnedObject(string tag){
        foreach(GameObject currentSpawned in spawnedRpcObjectList){
            if(currentSpawned.name == tag){
                return currentSpawned;
            }
        }
        return null;
    }
    /* public GameObject SpawnPhotonNetworkObject(string tag,Vector3 _pos,Quaternion _rotations){
        return PhotonNetwork.Instantiate(tag,_pos,_rotations);
    } */
    public int GetPoolLength(string groupName){
        foreach(PoolingGroup poolGroup in poolingGroup){
            if(poolGroup.groupName == groupName){
                return poolGroup.poolingList.Count;
            }
        }
        return 0;
    }
    public void ResetPool(System.Action OnPoolReset){
        for (int i = 0; i < SpawnedPool.Count; i++){
            SpawnedPool[i].DestroyNow();
        }
        OnPoolReset?.Invoke();
    }
    public void SpawnEffectOverNetwork(string effectName,Vector3 spawnPoint,Quaternion SpawnRotation){
        GameObject EffectObject = InstantiateOverNet(effectName,spawnPoint,SpawnRotation);
        if(EffectObject != null){
            if(EffectObject.TryGetComponent<ParticleSystem>(out ParticleSystem effect)){
                effect.Play();
            }
        }else{
            Debug.Log("Effect is not Found or Not Spawned in The Pool");
        }
    }
    public GameObject SpawnOverNetwork(string spawnId,Vector3 spawnPoint,Quaternion SpawnRotation){
        return InstantiateOverNet(spawnId,spawnPoint,SpawnRotation);
    }
    public void SpawnEffectPool(string effectName,Vector3 spawnPoint,Quaternion SpawnRotation){
        GameObject EffectObject = SpawnFromPool(effectName,spawnPoint,SpawnRotation);
        if(EffectObject != null){
            if(EffectObject.TryGetComponent<ParticleSystem>(out ParticleSystem effect)){
                effect.Play();
            }
        }else{
            Debug.Log("Effect is not Found or Not Spawned in The Pool");
        }
    }

    private GameObject InstantiateOverNet(string prefabId, Vector3 position, Quaternion rotation){
        return PhotonNetwork.Instantiate(prefabId,position,rotation);
    }

    public void Destroy(GameObject desableObject) {
        PhotonNetwork.Destroy(desableObject);
    }
}

