using UnityEngine;
using System.Collections.Generic;
namespace GamerWolf.Utils{

    public class SavingAndLoadingManager : GenericSingleton<SavingAndLoadingManager>{
        
        [SerializeField] private GameDataSO saveData;
        private void Awake(){
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        public void SaveGame(){
            saveData.Save();
        }
        public void LoadGame(){
            saveData.Load();
        }
        private void OnApplicationPause(bool pause){
            if(pause){
                SaveGame();
            }
        }
        
        private void OnApplicationQuit(){
            SaveGame();
        }

    }

}