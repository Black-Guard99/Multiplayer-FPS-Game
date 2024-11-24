using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;

public abstract class PopUp : MonoBehaviour, IPooledObject {
    private Transform _startingParent;

    protected abstract void OnActive();
    protected abstract void OnDeactive();
    public void DestroyMySelfWithDelay(float delay = 0) {
        Invoke(nameof(DestroyNow),delay);
    }

    public void DestroyNow() {
        transform.SetParent(_startingParent);
        CancelInvoke(nameof(DestroyNow));
        OnDeactive();
        gameObject.SetActive(false);
    }

    public void OnObjectReuse() {
        gameObject.SetActive(true);
        OnActive();
    }

    public void SetStartinParent(Transform _parent) {
        this._startingParent = _parent;
    }
}
