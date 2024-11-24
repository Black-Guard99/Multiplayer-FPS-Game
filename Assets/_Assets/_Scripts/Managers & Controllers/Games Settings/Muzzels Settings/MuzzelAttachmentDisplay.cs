using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MuzzelAttachmentDisplay : MonoBehaviour {
    [SerializeField] private MuzzelSO muzzel;
    // [SerializeField] private AttachmentDisplayGun attachmentDisplayGun;
    public MuzzelSO GetMuzzel(){
        return muzzel;
    }
}