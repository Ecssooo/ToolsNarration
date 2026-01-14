using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class InspectorButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawButtonsForTargets(targets);
    }

    static void DrawButtonsForTargets(UnityEngine.Object[] unityTargets)
    {
        var first = unityTargets != null && unityTargets.Length > 0 ? unityTargets[0] : null;
        if (first == null) return;

        var type = first.GetType();
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<ButtonAttribute>(true) != null)
            .Where(m => m.GetParameters().Length == 0)
            .ToArray();

        if (methods.Length == 0) return;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<ButtonAttribute>(true);
            var label = string.IsNullOrWhiteSpace(attr.Label) ? method.Name : attr.Label;

            using (new EditorGUI.DisabledScope(attr.PlayModeOnly && !EditorApplication.isPlaying))
            {
                if (GUILayout.Button(label))
                {
                    foreach (var obj in unityTargets)
                    {
                        var mb = obj as MonoBehaviour;
                        if (mb == null) continue;

                        Undo.RecordObject(mb, $"Invoke {method.Name}");
                        method.Invoke(mb, null);
                        EditorUtility.SetDirty(mb);
                    }
                }
            }
        }
    }
}
