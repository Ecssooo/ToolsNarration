using System;
using UnityEngine;

namespace Attributes.NonSerializedField
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class NonSerializedFieldAttribute : PropertyAttribute
    {
        public NonSerializedFieldAttribute() : base(true)
        {
        }
    }
}
