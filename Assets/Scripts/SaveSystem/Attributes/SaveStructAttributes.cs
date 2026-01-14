using System;
using UnityEngine;

namespace SaveSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public class SaveStructAttributes : PropertyAttribute
    {
        public string StructName { get; set; }
        public SaveStructAttributes(string structName) : base(true)
        {
            StructName = structName;
        }
    }
}
