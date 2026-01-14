using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class ButtonAttribute : PropertyAttribute
{
    public readonly string Label;
    public readonly bool PlayModeOnly;

    public ButtonAttribute(string label = null, bool playModeOnly = false)
    {
        Label = label;
        PlayModeOnly = playModeOnly;
    }
}
