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

        // Trouver binding existant (si déjà mappé)
        var fullTypeName = method.DeclaringType.AssemblyQualifiedName;

        var binding = config.bindings.FirstOrDefault(b => b.typeName == fullTypeName && b.methodName == method.Name);


        int currentIndex = -1;
        if (binding != null && jsonEventIds.Length > 0)
        {
            currentIndex = Array.IndexOf(jsonEventIds, binding.eventId);
        }

        // Popup
        int newIndex = EditorGUILayout.Popup(currentIndex, jsonEventIds);

        // S’il change, on maj le binding
        if (newIndex != currentIndex)
        {
            if (newIndex >= 0 && newIndex < jsonEventIds.Length)
            {
                if (binding == null)
                {
                    binding = new DialogueEventConfig.Binding
                    {
                        typeName = fullTypeName,
                        methodName = method.Name
                    };
                    config.bindings.Add(binding);
                }

                binding.eventId = jsonEventIds[newIndex];
            }
            else if (binding != null)
            {
                // -1 → retiré
                config.bindings.Remove(binding);
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}
