using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/Attachments/ScopeSO", fileName = "ScopeSO")]
public class ScopeSO : ScriptableObject {
    public enum ScopeType{
        IronSight,
        RedDot,
        HoloGraphic,
        x3Power,
        x6Power,
        x8Power,
    }
    public ScopeType scopeType;
    public float maxZoom = 60f,defultScopeZoom = 3f;
    public Sprite iconSprite;
    public float defultAimFov = 80;
}