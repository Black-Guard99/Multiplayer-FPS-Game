using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MagzineAtttachmentDisplay : MonoBehaviour {
    [SerializeField] private MagzineSO magzineSo;
    // [SerializeField] private AttachmentDisplayGun attachmentDisplayGun;
    public MagzineSO GetMagzine(){
        return magzineSo;
    }
}