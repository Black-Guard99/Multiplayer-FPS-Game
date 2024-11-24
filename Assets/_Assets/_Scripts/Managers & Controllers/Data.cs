using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Data : MonoBehaviour {
    public static void SaveData (object dataToSave,string filesuffix) {
        try {
            string path = Application.persistentDataPath + "/" + filesuffix + ".dt";

            if (File.Exists(path)) File.Delete(path);

            FileStream file = File.Create(path);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, dataToSave);
            file.Close();

            Debug.Log("SAVED SUCCESSFULY");
        } catch {
            Debug.Log("SOMETHING WENT TERRIBLY WRONG");
        }
    }

    public static object LoadData (string fileName) {
        object ret = new object();

        try {
            string path = Application.persistentDataPath  + "/" + fileName + ".dt";

            if (File.Exists(path)) {
                FileStream file = File.Open(path, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                ret = bf.Deserialize(file);

                Debug.Log("LOADED SUCCESSFULY");
            }
        } catch {
            Debug.Log("FILE WASN'T FOUND");
        }

        return ret;
    }
}