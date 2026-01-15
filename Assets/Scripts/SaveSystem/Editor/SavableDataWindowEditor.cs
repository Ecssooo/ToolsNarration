using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SaveSystem.Editor
{
    public class SavableDataWindowEditor : EditorWindow
    {
        private SavableDatas _savableDatas;
        private Dictionary<string, List<SavableField>> _splitFields;

        private Vector2 scrollPos;
        
        [MenuItem("SaveSystem/Data savable")]
        static void ShowWindow()
        {
            GetWindow(typeof(SavableDataWindowEditor));
        }

        private void OnGUI()
        {

            _savableDatas = EditorGUILayout.ObjectField("SavableDatas", _savableDatas, typeof(SavableDatas), false) as SavableDatas;
            EditorGUILayout.Space(10);

            if (_savableDatas == null)
            {
                EditorGUILayout.LabelField("Assign savable data");
                return;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Update data", GetButtonGUIOption()))
            {
                _savableDatas.UpdateList();
            }

            if (GUILayout.Button("Load data", GetButtonGUIOption()))
            {
                _splitFields = _savableDatas.SplitByClassName(_savableDatas.savableFields);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            
            if (_splitFields == null) return;
            DisplayField(); 
            
        }

        private void DisplayField()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
            foreach (var splitField in _splitFields)
            {
                EditorGUILayout.LabelField(splitField.Key, EditorStyles.boldLabel);
                foreach (var field in splitField.Value)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.MaxWidth(position.width / 1.05f));
                    EditorGUI.indentLevel++;
                    // EditorGUILayout.LabelField(field.fieldName);
                    EditorGUILayout.Space(-50);
                    if(_savableDatas.FindFieldInfo(field.className, field.fieldName, out var savableField))
                    {
                        savableField.isSavable = EditorGUILayout.Toggle(field.fieldName, savableField.isSavable);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndHorizontal();
                }
                
            }
            EditorGUILayout.EndScrollView();
        }
        
        private void OnEnable()
        {
            if(_savableDatas != null) _splitFields = _savableDatas.SplitByClassName(_savableDatas.savableFields);

        }

        private GUILayoutOption[] GetButtonGUIOption()
        {
            GUILayoutOption[] options = new GUILayoutOption[]
            {
                GUILayout.MaxHeight(30),
                GUILayout.MaxWidth(position.width / 2f)
            };

            return options;
        }
    }
}
