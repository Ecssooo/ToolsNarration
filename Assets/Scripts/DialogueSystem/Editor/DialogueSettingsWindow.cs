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
    private List<MethodInfo> dialogueMethods;
    private Vector2 scrollPosition;
    private string[] jsonEventIds = Array.Empty<string>();

    [MenuItem("Window/Dialogue Settings")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueSettingsWindow));
    }

    private void OnGUI()
    {
        // mettre le lien de config
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
            EditorGUILayout.HelpBox("Assigne un DialogueManager pour charger les données de dialogue.", MessageType.Info);
            return;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Refresh"))
        {
            dialogueManager.LoadDialogueDataFromJson();
            RefreshData();
        }

        if (dialogueMethods == null || dialogueMethods.Count == 0)
        {
            EditorGUILayout.HelpBox("Aucune méthode marquée [DialogueEvent] trouvée.", MessageType.Warning);
            return;
        }

        if (jsonEventIds == null || jsonEventIds.Length == 0)
        {
            EditorGUILayout.HelpBox("Aucun event trouvé dans le JSON (Choices.Events).", MessageType.Warning);
        }

        EditorGUILayout.Space(10);
        using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scroll.scrollPosition;

            if (dialogueMethods != null)
            {
                foreach (var method in dialogueMethods)
                {
                    DrawMethodBinding(method);
                }
            }
        }
    }

    private void RefreshData()
    {
        Debug.Log("Refreshing Dialogue Data from JSON...");
        dialogueMethods = FindAllDialogueEventMethods();
        jsonEventIds = ExtractEventIdsFromJson();
    }

    List<MethodInfo> FindAllDialogueEventMethods()
    {
        var result = new List<MethodInfo>();
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
                        result.Add(m);
                    }
                }
            }
        }

        return result;
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

    void DrawMethodBinding(MethodInfo method)
    {
        EditorGUILayout.BeginHorizontal();

        // Label de la méthode
        EditorGUILayout.LabelField($"{method.Name}", GUILayout.Width(260));

        if (config == null)
        {
            EditorGUILayout.LabelField("No Config Assigned");
            EditorGUILayout.EndHorizontal();
            return;
        }

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
        var binding = config.bindings.FirstOrDefault(b => b.typeName == fullTypeName && b.methodName == method.Name);

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
                    config.bindings.Remove(binding);
                }
            }
            else
            {
                // newIndex > 0 → correspond à jsonEventIds[newIndex - 1]
                string selectedEventId = jsonEventIds[newIndex - 1];

                if (binding == null)
                {
                    binding = new DialogueEventConfig.Binding
                    {
                        typeName = fullTypeName,
                        methodName = method.Name
                    };
                    config.bindings.Add(binding);
                }

                binding.eventId = selectedEventId;
            }

            EditorUtility.SetDirty(config);
        }

        EditorGUILayout.EndHorizontal();
    }

}
