using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public Dictionary<string, object> DatasToSave = new Dictionary<string, object>();
    }


    [Serializable]
    public class SavableField
    {
        public string className;
        public string fieldName;
        public bool isSavable;

        public SavableField(string className, string fieldName, bool isSavable)
        {
            this.className = className;
            this.fieldName = fieldName;
            this.isSavable = isSavable;
        }
    }
}
