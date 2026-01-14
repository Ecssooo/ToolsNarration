using System;
using UnityEngine;

namespace SaveSystem
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SaveClassAttributes : PropertyAttribute
    {
        
        public string ClassName { get; set; }
        
        public SaveClassAttributes(string className) : base(true)
        {
            ClassName = className;
        }
    }
}
