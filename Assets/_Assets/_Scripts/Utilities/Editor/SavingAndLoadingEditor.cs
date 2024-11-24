using UnityEngine;
using UnityEditor;
namespace GamerWolf.Utils{
    [CustomEditor(typeof(SavingAndLoadingManager))]
    public class SavingAndLoadingEditor : Editor{

        public override void OnInspectorGUI() {
            serializedObject.Update();
            SavingAndLoadingManager saveAndLoad = (SavingAndLoadingManager)target;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("saveData"));
            if(GUILayout.Button("Save Data")){
                saveAndLoad.SaveGame();
            }
            if(GUILayout.Button("Load Data")){
                saveAndLoad.LoadGame();
            }
            serializedObject.ApplyModifiedProperties();

        }
    }
}
