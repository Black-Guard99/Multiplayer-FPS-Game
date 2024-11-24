using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ScopeAttachmentDisplay : MonoBehaviour {

    [SerializeField] private ScopeSO scope;
    // [SerializeField] private AttachmentDisplayGun attachmentDisplayGun;
    public ScopeSO GetScopeType(){
        return scope;
    }
}