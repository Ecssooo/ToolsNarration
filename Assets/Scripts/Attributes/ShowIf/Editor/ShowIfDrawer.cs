using UnityEditor;
using UnityEngine;

namespace Attributes.ShowIf.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showif = attribute as ShowIfAttribute;
            if (showif == null) return;
            
            SerializedProperty condition = property.serializedObject.FindProperty(showif.BoolProperty);
            if (condition == null) return;

            if (condition.boolValue)
            {
                EditorGUI.PropertyField(position, property);
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.targetObject == null) return 0f;
            
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;
            SerializedProperty conditionProp = property.serializedObject.FindProperty(showIf.BoolProperty);

            if (conditionProp == null) return -EditorGUIUtility.standardVerticalSpacing;;

            bool show = conditionProp.boolValue;

            return show ? EditorGUI.GetPropertyHeight(property, label) : 0f;
        }  
    }
}
