using System;
using UnityEngine;

namespace SaveSystem
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SaveClassAttributes : PropertyAttribute
    {
        
        
        public SaveClassAttributes() : base(true)
        {
        }
    }
}
