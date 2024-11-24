using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Game Data",menuName = "ScriptableObject/Game Data")]
public class GameDataSO : ScriptableObject {
    
    [SerializeField] private PlayerSaveData saveData;
    public void SetNewLongestDistance(float currentDistance){
        if(currentDistance > saveData.LongestDistance){
            saveData.LongestDistance = currentDistance;
        }
    }
    public void IncreaseLevel(){
        saveData.levelNum ++;

    }
    public void IncreaseCash(int amount){
        saveData.TotalCash += amount;
    }
    public void SpendCash(int amount){
        saveData.TotalCash -= amount;
        if(saveData.TotalCash <= 0){
            saveData.TotalCash = 0;
        }
    }
    public void SetReviewd(){
        saveData.settingsSaveData.isReviewd = true;
    }
    public int GetTotalCash(){
        return saveData.TotalCash;
    }
    public int GetLevelNum(){
        return saveData.levelNum;
    }
    public void SetHasAdsInGame(bool value){
        saveData.settingsSaveData.hasAdsInGame = value;
    }
    public bool GetHasAds(){
        return saveData.settingsSaveData.hasAdsInGame;
    }
    public bool IsRevived(){
        return saveData.settingsSaveData.isReviewd;
    }
    public float GetLongestDistance(){
        return saveData.LongestDistance;
    }
    public void ToggleVFX(){
        saveData.settingsSaveData.isVFXOn = !saveData.settingsSaveData.isVFXOn;
    }
    public void ToggelMusic(){
        saveData.settingsSaveData.isMusicOn = !saveData.settingsSaveData.isMusicOn;
    }
    public void ToggelSound(){
        saveData.settingsSaveData.isSoundOn = !saveData.settingsSaveData.isSoundOn;
    }
    public bool GetMusicState(){
        return saveData.settingsSaveData.isMusicOn;
    }
    public bool GetSoundState(){
        return saveData.settingsSaveData.isSoundOn;
    }
    public bool GetVFXState(){
        return saveData.settingsSaveData.isVFXOn;
    }
    



    #region Saving and Loading................

    [ContextMenu("Save")]
    public void Save(){
        /* string data = JsonUtility.ToJson(saveData,true);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath,"/",name,"GameData",".dat"));
        formatter.Serialize(file,data);
        file.Close(); */
        Data.SaveData((PlayerSaveData) saveData,"gameData");
    }

    [ContextMenu("Load")]
    public void Load(){
        /* if(File.Exists((string.Concat(Application.persistentDataPath,"/",name,"GameData",".dat")))){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream Stream = File.Open(string.Concat(Application.persistentDataPath,"/",name,"GameData",".dat"),FileMode.Open);
            JsonUtility.FromJsonOverwrite(formatter.Deserialize(Stream).ToString(),saveData);
            Stream.Close();
        } */
        saveData = (PlayerSaveData)Data.LoadData("gameData");
    }

    #endregion
    [System.Serializable]
    public struct PlayerSaveData{
        public int levelNum;
        public int TotalCash;
        public float LongestDistance;

        [Header("Settings")]
        public SettingsSaveData settingsSaveData;
    }
    [System.Serializable]
    public struct SettingsSaveData{
        public bool hasAdsInGame;
        public bool isReviewd;
        public bool isVFXOn;
        public bool isMusicOn;
        public bool isSoundOn;
    }
}
