using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Pool",menuName = "Utils/Object Pooling/Pool")]
public class PoolSO : ScriptableObject {
    public GameObject prefabs;
    public int size;
}
