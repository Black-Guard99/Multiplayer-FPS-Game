using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityRandom = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/TrailConfig", fileName = "Gun/TrailConfig")]
public class TrailConfig : ScriptableObject {
    public Material material;
    public AnimationCurve widthCurve;
    public float duration = 0.2f;
    public float minVertexDistance =0.3f;
    public Gradient colorGradient;

    public float missDistance = 100f;
    public float simulationSpeed = 100f;
}