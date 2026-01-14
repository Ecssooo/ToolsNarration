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

        foreach (var b in config.bindings)
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

    public void TriggerEvent(string eventId)
    {
        if (!eventMap.TryGetValue(eventId, out var method))
        {
            Debug.LogWarning($"Aucun binding pour l'event '{eventId}'");
            return;
        }

        object instance = null;
        if (!method.IsStatic)
        {
            // Ici tu peux améliorer : FindObjectOfType, référence explicite, etc.
            instance = FindAnyObjectByType(method.DeclaringType);
        }

        method.Invoke(instance, null);
    }
}
