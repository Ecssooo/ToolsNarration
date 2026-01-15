using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DialogueEventRunner : MonoBehaviour
{
    [SerializeField] private DialogueEventConfig config;

    public static DialogueEventRunner Instance { get; private set; }

    private Dictionary<string, MethodInfo> eventMap = new Dictionary<string, MethodInfo>();

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
        BuildEventMap();
    }

    void BuildEventMap()
    {
        eventMap.Clear();

        foreach (var b in config.eventsBindings)
        {
            var type = Type.GetType(b.typeName);
            if (type == null)
            {
                Debug.LogWarning($"Type introuvable pour event {b.eventId}: {b.typeName}");
                continue;
            }

            var method = type.GetMethod(
                b.methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
            );

            if (method == null)
            {
                Debug.LogWarning($"Méthode introuvable pour event {b.eventId}: {b.methodName}");
                continue;
            }

            eventMap[b.eventId] = method;
        }
    }

    public void TriggerEvent(DialogueEvent ev)
    {
        if (!eventMap.TryGetValue(ev.Id, out var method))
        {
            Debug.LogWarning($"Unknown event: {ev.Id}");
            return;
        }

        object instance = null;
        if (!method.IsStatic)
            instance = FindAnyObjectByType(method.DeclaringType);

        ParameterInfo[] parameters = method.GetParameters();

        object[] parsedParams = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            parsedParams[i] = Convert.ChangeType(ev.Params[i], parameters[i].ParameterType);
        }

        method.Invoke(instance, parsedParams);
    }
}
