using System;
using UnityEngine;

namespace Attributes.ShowIf
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        
        public string BoolProperty { get; set; }
        
        public bool Result { get; set; }

        public ShowIfAttribute(string field) : base(true)
        {
            this.BoolProperty = field;
        }
    }
}
