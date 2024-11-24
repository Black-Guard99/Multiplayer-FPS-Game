using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/RecoilConfigSO", fileName = "RecoilConfigSO")]
public class RecoilConfigSO : StatSO {
    [Range(-1,1)] public float xRecoilMin = 0.01f,xRecoilMax = 0.09f;
    [Range(-1,1)] public float yRecoilMin = -.016f,yRecoilMax = .016f;
    public Vector2 GetRecoilValue(){
        return new Vector2(Random.Range(xRecoilMin,xRecoilMax),Random.Range(yRecoilMin,yRecoilMax));
    }
}