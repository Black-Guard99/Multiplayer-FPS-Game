using System;
using System.IO;
using UnityEngine;
using GamerWolf.Utils;
using System.Runtime.Serialization.Formatters.Binary;

[CreateAssetMenu(menuName = "Configs/Gun", fileName = "Gun")]
public class GunSO : ScriptableObject {
    [Header("Refs")]
    public Gun gunModel;
    
    [Header("UI Referances")]
    public Sprite weponUiIcon;
    [Space(10)]
    [Header("Config Referances")]
    public AmmoConfig ammoConfig;
	public TrailConfig trailConfig;
    public ShootConfig shootConfig;
    public AttachmentSO attachmentConfig;
    [Space(10)]
    [Header("Settings")]
    public GunType gunType;
    public Gun.WeponPositions weponPositions;
	public Vector3 spawnPointOffset;
    public ProfileData playerProfile;

    [Header("Stat")]
	public StatSO accuracyStat;
    
}