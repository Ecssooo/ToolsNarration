using System;
using System.Collections.Generic;
using Attributes.NonSerializedField;

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
        [NonSerializedField] public string className;
        [NonSerializedField] public string fieldName;
        public bool isSavable;

        public SavableField(string className, string fieldName, bool isSavable)
        {
            this.className = className;
            this.fieldName = fieldName;
            this.isSavable = isSavable;
        }
    }
}
