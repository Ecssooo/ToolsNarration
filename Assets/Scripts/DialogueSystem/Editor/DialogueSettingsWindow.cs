using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

public class DialogueSettingsWindow : EditorWindow
{
    private DialogueEventConfig config;
    private DialogueManager dialogueManager;
    private List<MethodInfo> dialogueEventMethods = new List<MethodInfo>();
    private List<FieldInfo> dialogueConditionFields = new List<FieldInfo>();
    private Vector2 scrollPositionEvents;
    private Vector2 scrollPositionConditions;
    private string[] jsonEventIds = Array.Empty<string>();
    private string[] jsonConditionIds = Array.Empty<string>();

    [MenuItem("Window/Dialogue Settings")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueSettingsWindow));
    }

    private void OnEnable()
    {
        if (dialogueManager == null || config == null)
        {
            return;
        }
        RefreshData();
    }

    private void OnGUI()
    {
        GUILayout.Label("Dialogue Event Config", EditorStyles.boldLabel);
        config = EditorGUILayout.ObjectField("Config Asset", config, typeof(DialogueEventConfig), false) as DialogueEventConfig;

        if (config == null)
        {
            EditorGUILayout.HelpBox("Assigne un DialogueEventConfig pour sauvegarder les bindings.", MessageType.Info);
            return;
        }

        GUILayout.Space(10);

        GUILayout.Label("Dialogue Data", EditorStyles.boldLabel);
        dialogueManager = EditorGUILayout.ObjectField("Dialogue Manager", dialogueManager, typeof(DialogueManager), true) as DialogueManager;

        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
            if (dialogueManager == null)
            {
                EditorGUILayout.HelpBox("Assigne un DialogueManager pour charger les données de dialogue.", MessageType.Info);
                return;
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Refresh"))
        {
            dialogueManager.LoadDialogueDataFromJson();
            RefreshData();
        }

        if (dialogueEventMethods == null || dialogueEventMethods.Count == 0)
        {
            EditorGUILayout.HelpBox("Aucune méthode marquée [DialogueEvent] trouvée.", MessageType.Warning);
        }

        if (dialogueConditionFields == null || dialogueConditionFields.Count == 0)
        {
            EditorGUILayout.HelpBox("Aucune méthode marquée [DialogueCondition] trouvée.", MessageType.Warning);
        }

        if (jsonEventIds == null || jsonEventIds.Length == 0)
        {
            EditorGUILayout.HelpBox("Aucun event trouvé dans le JSON (Choices.Events).", MessageType.Warning);
        }

        EditorGUILayout.Space(10);
        GUILayout.Label("Bind Dialogue Events to Methods", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginScrollView(scrollPositionEvents);
        using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPositionEvents))
        {
            scrollPositionEvents = scroll.scrollPosition;

            if (dialogueEventMethods != null)
            {
                foreach (var method in dialogueEventMethods)
                {
                    DrawMethodBinding(method);
                }
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(10);
        GUILayout.Label("Bind Dialogue Conditions to Methods", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginScrollView(scrollPositionConditions);
        using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPositionConditions))
        {
            scrollPositionConditions = scroll.scrollPosition;

            if (dialogueConditionFields != null)
            {
                foreach (var field in dialogueConditionFields)
                {
                    DrawnConditionBinding(field);
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void RefreshData()
    {
        Debug.Log("Refreshing Dialogue Data from JSON...");
        FindAllDialogueEventMethods();
        jsonEventIds = ExtractEventIdsFromJson();
        jsonConditionIds = ExtractConditionIdsFromJson();
    }

    void FindAllDialogueEventMethods()
    {
        dialogueEventMethods.Clear();
        dialogueConditionFields.Clear();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var asm in assemblies)
        {
            Type[] types;
            try { types = asm.GetTypes(); } catch { continue; }

            foreach (var type in types)
            {
                var methods = type.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Static
                );

                foreach (var m in methods)
                {
                    if (m.GetCustomAttribute(typeof(DialogueEventAttribute)) != null)
                    {
                        dialogueEventMethods.Add(m);
                    }
                }
                var fields = type.GetFields(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Static
                );
                foreach (var f in fields)
                {
                    if (f.GetCustomAttribute(typeof(DialogueConditionAttribute)) != null)
                    {
                        dialogueConditionFields.Add(f);
                    }
                }

            }
        }

    }

    string[] ExtractEventIdsFromJson()
    {
        Graph data = dialogueManager.Graph;
        if (data == null)
            return Array.Empty<string>();

        HashSet<string> events = new HashSet<string>();

        try
        {
            if (data?.Nodes != null)
            {
                foreach (var node in data.Nodes)
                {
                    if (node.Choices == null) continue;
                    foreach (var choice in node.Choices)
                    {
                        if (choice.Events == null) continue;
                        foreach (var ev in choice.Events)
                        {

                            // if (!string.IsNullOrEmpty(ev.Id))
                            events.Add(ev.Id);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Erreur parse JSON : " + e.Message);
        }

        return events.ToArray();
    }

    string[] ExtractConditionIdsFromJson()
    {
        Graph data = dialogueManager.Graph;
        if (data == null)
            return Array.Empty<string>();

        HashSet<string> conditions = new HashSet<string>();

        try
        {
            if (data?.Nodes != null)
            {
                foreach (var node in data.Nodes)
                {
                    if (node.Choices == null) continue;
                    foreach (var choice in node.Choices)
                    {
                        if (choice.AvailabilityConditions == null) continue;
                        foreach (var cond in choice.AvailabilityConditions)
                        {
                            conditions.Add(cond);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Erreur parse JSON : " + e.Message);
        }

        return conditions.ToArray();
    }

    void DrawMethodBinding(MethodInfo method)
    {
        // Une ligne "tableau" avec une bordure
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        if (config == null)
        {
            EditorGUILayout.LabelField("No Config Assigned");
            EditorGUILayout.EndHorizontal();
            return;
        }

        // Colonne 1 : le nom de la méthode
        EditorGUILayout.Space(2);
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField(method.Name, GUILayout.Width(260));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);


        // Options du popup : 0 = <None>, puis tous les eventIds
        string[] popupOptions;
        if (jsonEventIds != null && jsonEventIds.Length > 0)
        {
            popupOptions = new string[jsonEventIds.Length + 1];
            popupOptions[0] = "<None>";
            Array.Copy(jsonEventIds, 0, popupOptions, 1, jsonEventIds.Length);
        }
        else
        {
            popupOptions = new[] { "<None>" };
        }

        // Trouver binding existant (si déjà mappé)
        var fullTypeName = method.DeclaringType.AssemblyQualifiedName;
        var binding = config.eventsBindings.FirstOrDefault(b =>
            b.typeName == fullTypeName &&
            b.methodName == method.Name
        );

        // Index courant dans le popup
        int currentIndex = 0; // 0 = <None> par défaut

        if (binding != null && jsonEventIds.Length > 0)
        {
            int idx = Array.IndexOf(jsonEventIds, binding.eventId);
            if (idx >= 0)
            {
                currentIndex = idx + 1; // +1 car 0 = <None>
            }
        }

        // Colonne 2 : dropdown
        int newIndex = EditorGUILayout.Popup(currentIndex, popupOptions);

        // S’il change, on maj le binding
        if (newIndex != currentIndex)
        {
            if (newIndex == 0)
            {
                // <None> → on supprime le binding s'il existe
                if (binding != null)
                {
                    config.eventsBindings.Remove(binding);
                }
            }
            else
            {
                // newIndex > 0 → correspond à jsonEventIds[newIndex - 1]
                string selectedEventId = jsonEventIds[newIndex - 1];

                if (binding == null)
                {
                    binding = new DialogueEventConfig.BindingEvent
                    {
                        typeName = fullTypeName,
                        methodName = method.Name
                    };
                    config.eventsBindings.Add(binding);
                }

                binding.eventId = selectedEventId;
            }

            EditorUtility.SetDirty(config);
        }

        EditorGUILayout.EndHorizontal();
    }


    void DrawnConditionBinding(FieldInfo field)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        if (config == null)
        {
            EditorGUILayout.LabelField("No Config Assigned");
            EditorGUILayout.EndHorizontal();
            return;
        }

        // Label de la méthode
        EditorGUILayout.Space(2);
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        EditorGUILayout.LabelField($"{field.Name}", GUILayout.Width(260));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);

        // Options du popup : 0 = <None>, puis tous les eventIds
        string[] popupOptions;
        if (jsonConditionIds != null && jsonConditionIds.Length > 0)
        {
            popupOptions = new string[jsonConditionIds.Length + 1];
            popupOptions[0] = "<None>";
            Array.Copy(jsonConditionIds, 0, popupOptions, 1, jsonConditionIds.Length);
        }
        else
        {
            popupOptions = new[] { "<None>" };
        }

        // Trouver binding existant (si déjà mappé)
        var fullTypeName = field.DeclaringType.AssemblyQualifiedName;
        var binding = config.conditionBindings.FirstOrDefault(b => b.typeName == fullTypeName && b.fieldName == field.Name);

        // Index courant dans le popup
        int currentIndex = 0; // 0 = <None> par défaut

        if (binding != null && jsonConditionIds.Length > 0)
        {
            int idx = Array.IndexOf(jsonConditionIds, binding.eventId);
            if (idx >= 0)
            {
                currentIndex = idx + 1; // +1 car 0 = <None>
            }
        }

        // Popup
        int newIndex = EditorGUILayout.Popup(currentIndex, popupOptions);

        // S’il change, on maj le binding
        if (newIndex != currentIndex)
        {
            if (newIndex == 0)
            {
                // <None> → on supprime le binding s'il existe
                if (binding != null)
                {
                    config.conditionBindings.Remove(binding);
                }
            }
            else
            {
                // newIndex > 0 → correspond à jsonConditionIds[newIndex - 1]
                string selectedEventId = jsonConditionIds[newIndex - 1];

                if (binding == null)
                {
                    binding = new DialogueEventConfig.BindingCondition
                    {
                        typeName = fullTypeName,
                        fieldName = field.Name
                    };
                    config.conditionBindings.Add(binding);
                }

                binding.eventId = selectedEventId;
            }

            EditorUtility.SetDirty(config);
        }

        EditorGUILayout.EndHorizontal();
    }
}
