using System;
using UnityEngine;

namespace SaveSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SaveFieldAttributes : PropertyAttribute
    {
        public SaveFieldAttributes() : base(true)
        {
        }
    }
}
