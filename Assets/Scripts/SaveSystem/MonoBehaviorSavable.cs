using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using SaveSystem.Attributes;
using UnityEngine;

namespace SaveSystem
{
    public class MonoBehaviorSavable : MonoBehaviour
    {
        [SerializeField, Tooltip("must be unique")] private int _saveID;
        
        protected SaveManager SaveManager;
        
        protected void Awake()
        {
            SaveManager = FindFirstObjectByType<SaveManager>();
            if (SaveManager == null)
            {
                Debug.LogWarning("SaveManager not find");
                return;
            }

            SaveManager.OnSave += Save;
            SaveManager.OnLoad += Load;
        }

        protected void Save()
        {
            var fieldInfos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.GetCustomAttribute<SaveFieldAttributes>() != null);
            
            string className = "";
            string fieldName = "";
            
            foreach (var info in fieldInfos)
            {
                // var classAttribute = this.GetType().GetCustomAttribute<SaveClassAttributes>();
                // if (classAttribute != null)
                // {
                //     className = classAttribute.ClassName;
                // }
                //
                // var fieldAttribute = info.GetCustomAttribute<SaveFieldAttributes>();
                // if (fieldAttribute != null)
                // {
                //     fieldName = fieldAttribute.FieldName;
                // }

                className = this.GetType().ToString();
                fieldName = info.Name;
                
                if (SaveManager.SavableDatas.FindFieldInfo(className, fieldName, out var field))
                {
                    if (field.isSavable)
                    {
                        string key = $"{className}.{fieldName}.{_saveID}";
                        var value = info.GetValue(this);
                        SaveManager.SaveData.DatasToSave.Add(key, value);
                    }
                }
            }
        }

        protected void Load()
        {
            var fieldInfos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.GetCustomAttribute<SaveFieldAttributes>() != null);
            
            string className = "";
            string fieldName = "";
            
            foreach (var info in fieldInfos)
            {
                // var classAttribute = this.GetType().GetCustomAttribute<SaveClassAttributes>();
                // if (classAttribute != null)
                // {
                //     className = classAttribute.ClassName;
                // }
                //
                // var fieldAttribute = info.GetCustomAttribute<SaveFieldAttributes>();
                // if (fieldAttribute != null)
                // {
                //     fieldName = fieldAttribute.FieldName;
                // }

                className = this.GetType().ToString();
                fieldName = info.Name;
                
                string key = $"{className}.{fieldName}.{_saveID}";

                object value = SaveManager.SaveData.DatasToSave[key];
                if (value is JObject jObject)
                {
                    value = jObject.ToObject(info.FieldType);
                }
                else
                {
                    value = Convert.ChangeType(value, info.FieldType);
                }
                info.SetValue(this, value);
            }
        }
    }
}
