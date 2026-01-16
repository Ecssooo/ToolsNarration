using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NpcDialogueSelector.ConditionalStart))] //création du Drawer
public class ConditionalStartDrawer : PropertyDrawer
{
    static readonly Dictionary<int, Dictionary<string, string>> cache = new(); //cache pour éviter de reparser le json

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)  //Empèche unity de couper mon texte
    {
        float line = EditorGUIUtility.singleLineHeight; //hauteur d'une ligne
        float space = EditorGUIUtility.standardVerticalSpacing; //espace d'une ligne

        float h = 0f;

        h += line;
        h += space;
        h += line;

        h += space;
        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Condition"), true); // ajoute la hauteur requis pour afficher la condition, evite de passer le texte

        h += space;
        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("SetFlagOnPlay"), true); //ajoute la hauteur pour le flag

        return h;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) //dessine l'élément
    {
        float line = EditorGUIUtility.singleLineHeight; //hauteur
        float space = EditorGUIUtility.standardVerticalSpacing; // espace

        EditorGUI.BeginProperty(position, label, property); //début du draw

        var startIdProp = property.FindPropertyRelative("StartNodeId");
        var conditionProp = property.FindPropertyRelative("Condition");
        var setFlagProp = property.FindPropertyRelative("SetFlagOnPlay");

        Rect r = position; //rectangle qu'on déplace
        r.height = line;

        EditorGUI.PropertyField(r, startIdProp); //dessine le champ

        r.y += line + space; //dessend d'une ligne...

        DrawValidationLine(r, property, startIdProp.stringValue); //dessine le message coloré

        r.y += line + space;

        float condH = EditorGUI.GetPropertyHeight(conditionProp, true); //calcule la hauteur pour condition
        r.height = condH;
        EditorGUI.PropertyField(r, conditionProp, true);// dessine condition

        r.y += condH + space;

        float flagH = EditorGUI.GetPropertyHeight(setFlagProp, true); //pareil que condition
        r.height = flagH;
        EditorGUI.PropertyField(r, setFlagProp, true); //dessine flag

        EditorGUI.EndProperty(); //fin du draw
    }

    void DrawValidationLine(Rect r, SerializedProperty elementProperty, string nodeId)
    {
        var selector = elementProperty.serializedObject.targetObject as NpcDialogueSelector; //récup le component dans l'inspector
        if (selector == null)
        {
            DrawColoredLabel(r, "Impossible de valider.", Color.red); //erreur si pas trouvé
            return;
        }

        var dbJsonField = typeof(NpcDialogueSelector).GetField("dialogueDatabaseJson", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance); //récup le field
        var dbJson = dbJsonField != null ? dbJsonField.GetValue(selector) as TextAsset : null; //récup le textAsset json

        if (dbJson == null)
        {
            DrawColoredLabel(r, "Aucun JSON global assigné.", new Color(1f, 0.55f, 0f));//json absent
            return;
        }

        if (string.IsNullOrEmpty(nodeId))
        {
            DrawColoredLabel(r, "StartNodeId vide.", Color.red); //id vide
            return;
        }

        var map = GetOrBuildMap(dbJson); //construit ou récup le cache nodeId -> preview
        if (map == null)
        {
            DrawColoredLabel(r, "JSON invalide.", Color.red); //json cassé
            return;
        }

        if (!map.TryGetValue(nodeId, out var preview))
        {
            DrawColoredLabel(r, $"ID introuvable dans le graph : {nodeId}", Color.red);//id pas trouvé
            return;
        }

        DrawColoredLabel(r, preview, Color.green);//id ok =vert
    }

    static void DrawColoredLabel(Rect r, string text, Color color)
    {
        var style = new GUIStyle(EditorStyles.label); //copie style unity
        style.normal.textColor = color; //change la couleur
        EditorGUI.LabelField(r, text, style); //affiche
    }

    static Dictionary<string, string> GetOrBuildMap(TextAsset json)
    {
        int key = json.GetInstanceID(); //clé unique pour ce textAsset
        if (cache.TryGetValue(key, out var map))
            return map;

        try
        {
            var graph = JsonUtility.FromJson<Graph>(json.text); //parse le json en Graph
            if (graph == null || graph.Nodes == null)
                return null;

            map = new Dictionary<string, string>(StringComparer.Ordinal);

            for (int i = 0; i < graph.Nodes.Count; i++)//parcour tous les nodes
            {
                var n = graph.Nodes[i];
                if (n == null || string.IsNullOrEmpty(n.NodeId)) continue;//skip si invalide

                string t = "";
                if (n.Text != null && n.Text.Count > 0 && n.Text[0] != null)
                    t = n.Text[0]; //prend le premier texte

                t = FirstWords(t, 5);
                map[n.NodeId] = t; //stocke le preview
            }

            cache[key] = map; //enregistre en cache
            return map;
        }
        catch
        {
            return null;
        }
    }

    static string FirstWords(string s, int count)
    {
        if (string.IsNullOrEmpty(s)) return "";

        s = s.Replace("\n", " ").Trim(); //nettoie le texte
        var parts = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //split en mots

        int n = Mathf.Min(count, parts.Length); //limite au nombre de mots dispo
        if (n <= 0) return "";

        string res = string.Join(" ", parts, 0, n); //remet les mots
        if (parts.Length > n) res += "..."; //ajoute si plus long
        return res;
    }
}
