using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueConditionAttribute))]
public class DialogueConditionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Boolean)
        {
            EditorGUI.HelpBox(position,
                $"{label.text} doit être un booléen pour utiliser DialogueConditionAttribute.",
                MessageType.Error);
            return;
        }

        // Si OK, afficher normalement la case à cocher
        EditorGUI.PropertyField(position, property, label);
    }
}