using System;
using UnityEngine;

namespace SaveSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SaveFieldAttributes : PropertyAttribute
    {
        
        public string FieldName { get; set; }
        public SaveFieldAttributes(string fieldName) : base(true)
        {
            FieldName = fieldName;
        }
    }
}
