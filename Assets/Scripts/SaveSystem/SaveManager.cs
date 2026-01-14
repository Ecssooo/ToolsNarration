using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        private SaveData _saveData = new SaveData();

        public SaveData SaveData { get => _saveData; set => _saveData = value; }

        [field: SerializeField] public SavableDatas SavableDatas { get; set; } 
        
        public Action OnSave;
        public Action OnLoad;
        
        public void SaveGame()
        {
            if (_saveData == null) return;
            ClearSave();
            OnSave?.Invoke();
            string json = JsonConvert.SerializeObject(_saveData.DatasToSave, Formatting.Indented);
            File.WriteAllText(Application.persistentDataPath + "/save.json", json);
            Debug.Log(Application.persistentDataPath);
        }

        public void ClearSave()
        {
            string json = JsonConvert.SerializeObject(new SaveData().DatasToSave, Formatting.Indented);
            File.WriteAllText(Application.persistentDataPath + "/save.json", json);
            _saveData.DatasToSave.Clear();
        }
        
        public void LoadSave()
        {
            string path = Application.persistentDataPath + "/save.json";

            if (!File.Exists(path)) return;
            
            string json = File.ReadAllText(path);

            _saveData.DatasToSave = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            OnLoad?.Invoke();
        }

        public void DebugSaveList()
        {
            foreach (var data in _saveData.DatasToSave)
            {
                Debug.Log($"{data.Key} : {data.Value}");
            }
        }

        //TODO : Create a SaveClassAttributes : DONE
        //       Function GetAllSaveClass : DONE
        //       Get field from SaveClass : DONE
        //       Load from JSON
        //       Reassign all value
    }
}
