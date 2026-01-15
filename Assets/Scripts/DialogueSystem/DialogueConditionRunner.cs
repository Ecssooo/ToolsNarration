using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DialogueConditionRunner : MonoBehaviour
{
    [SerializeField] private DialogueEventConfig config;

    public static DialogueConditionRunner Instance { get; private set; }

    private Dictionary<string, FieldInfo> conditionMap = new Dictionary<string, FieldInfo>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        BuildConditionMap();
    }

    void BuildConditionMap()
    {
        conditionMap.Clear();

        foreach (var b in config.conditionBindings)
        {
            var type = Type.GetType(b.typeName);
            if (type == null)
            {
                Debug.LogWarning($"Type introuvable pour condition {b.eventId}: {b.typeName}");
                continue;
            }

            var field = type.GetField(
                b.fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
            );

            if (field == null)
            {
                Debug.LogWarning($"Champ introuvable pour condition {b.eventId}: {b.fieldName}");
                continue;
            }

            conditionMap[b.eventId] = field;
        }
    }

    public bool EvaluateCondition(string conditionId)
    {
        if (!conditionMap.TryGetValue(conditionId, out var field))
        {
            Debug.LogWarning($"Unknown condition: {conditionId}");
            return false;
        }

        object instance = null;
        if (!field.IsStatic)
        {
            instance = FindAnyObjectByType(field.DeclaringType);
            if (instance == null)
            {
                Debug.LogWarning($"No instance found for condition field: {conditionId}");
                return false;
            }
        }

        var value = field.GetValue(instance);
        if (value is bool boolValue)
        {
            return boolValue;
        }
        else
        {
            Debug.LogWarning($"Condition field is not a boolean: {conditionId}");
            return false;
        }
    }
}
