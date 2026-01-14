using UnityEditor;
using UnityEngine;

namespace SaveSystem.Editor
{
    [CustomEditor(typeof(SavableDatas))]
    public class SavableDatasEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
             serializedObject.Update();

             SavableDatas SO = (SavableDatas)target;
             
             DrawDefaultInspector();

             if (GUILayout.Button("Update"))
             {
                 SO.UpdateList();
             }
        }
    }
}
