using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace SaveSystem.Editor
{
    public class SavableDataWindowEditor : EditorWindow
    {
        private SavableDatas _savableDatas;
        private Dictionary<string, List<SavableField>> _splitFields;
        
        [MenuItem("SaveSystem/Data savable")]
        static void ShowWindow()
        {
            GetWindow(typeof(SavableDataWindowEditor));
        }

        private void OnGUI()
        {

            _savableDatas = EditorGUILayout.ObjectField("SavableDatas", _savableDatas, typeof(SavableDatas), false) as SavableDatas;

            if (_savableDatas == null)
            {
                EditorGUILayout.LabelField("Assign savable data");
                return;
            }

            if (GUILayout.Button("Update data"))
            {
                _savableDatas.UpdateList();
            }

            if (GUILayout.Button("Load data"))
            {
                _splitFields = _savableDatas.SplitByClassName(_savableDatas.savableFields);
            }

            if (_splitFields == null) return;
            
            foreach (var splitField in _splitFields)
            {
                EditorGUILayout.LabelField(splitField.Key, EditorStyles.boldLabel);
                foreach (var field in splitField.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(field.fieldName);
                    if(_savableDatas.FindFieldInfo(field.className, field.fieldName, out var savableField))
                    {
                        savableField.isSavable = EditorGUILayout.Toggle(savableField.isSavable);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void OnEnable()
        {
            if(_savableDatas != null) _splitFields = _savableDatas.SplitByClassName(_savableDatas.savableFields);

        }
    }
}
