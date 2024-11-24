using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public interface IPooledObject{
    void SetStartinParent(Transform _parent);
    void OnObjectReuse();
    void DestroyMySelfWithDelay(float delay = 0f);
    void DestroyNow();
}
