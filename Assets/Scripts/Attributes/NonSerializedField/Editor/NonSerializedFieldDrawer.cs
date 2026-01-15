using UnityEditor;
using UnityEngine;

namespace Attributes.NonSerializedField.Editor
{
    [CustomPropertyDrawer(typeof(NonSerializedFieldAttribute))]
    public class NonSerializedFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            NonSerializedFieldAttribute nonSerialized = attribute as NonSerializedFieldAttribute;
            
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}
