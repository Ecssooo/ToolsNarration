using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SaveSystem.Attributes;
using UnityEngine;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "SavableDatas", menuName = "SaveSystem/SavableDatas")]
    public class SavableDatas : ScriptableObject
    {
        public List<SavableField> savableFields = new List<SavableField>();
        
        public void UpdateList()
        {
            var result = new List<MethodInfo>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            savableFields.Clear();
            
            foreach (var asm in assemblies)
            {
                Type[] types;
                try { types = asm.GetTypes(); }catch { continue; }
                types = types.Where(t => t.IsClass && t.IsDefined(typeof(SaveClassAttributes), inherit: false))
                    .ToArray();

                foreach (var type in types)
                {
                    var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field.GetCustomAttribute<SaveFieldAttributes>() != null);
                    
                    //var className = type.GetCustomAttribute<SaveClassAttributes>()?.ClassName;
                    var className = type.ToString();
                    
                    foreach (var info in fieldInfos)
                    {
                        // var fieldName = info.GetCustomAttribute<SaveFieldAttributes>().FieldName;
                        var fieldName = info.Name;
                        var savableField = new SavableField(className, fieldName, true);
                        if (FindFieldInfo(className, fieldName, out SavableField field))
                        {
                            Debug.Log("field already in list");
                        }
                        else
                        {
                            savableFields.Add(savableField);
                        }
                    }
                }
            }
        }

        public bool FindFieldInfo(string className, string fieldName, out SavableField field)
        {
            
            for (int i = 0; i < savableFields.Count; i++)
            {
                if (savableFields[i].className == className)
                {
                    if (savableFields[i].fieldName == fieldName)
                    {
                        field = savableFields[i];
                        return true;
                    }
                }
                else continue;
            }

            field = null;
            return false;
        }

        public Dictionary<string, List<SavableField>> SplitByClassName(List<SavableField> fields)
        {
            Dictionary<string, List<SavableField>> result = new Dictionary<string, List<SavableField>>();

            foreach (var field in fields)
            {
                if (result.ContainsKey(field.className))
                {
                    result[field.className].Add(field);
                }
                else
                {
                    var list = new List<SavableField>();
                    list.Add(field);
                    result.Add(field.className,list);
                }
            }
            return result;
        }
    }
}
