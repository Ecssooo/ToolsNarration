using UnityEditor;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    [SerializeField] private TextAsset dialogueJsonFile;

    [SerializeField] private Graph graph;
    public Graph Graph
    {
        get { return graph; }
        set { graph = value; }
    }

    void Awake()
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
        }
    }

    [Button("LoadDialogue")]
    public void LoadDialogueDataFromJson()
    {
        graph = JsonManager.JsonToDialogueData(dialogueJsonFile.text);
    }

    public Node GetNodeByID(string id)
    {
        foreach (Node node in graph.Nodes)
        {
            if (node.NodeId == id)
            {
                return node;
            }
        }

        return null;
    }

    public static Graph JsonToDialogueData(string jsonString)
    {
        return JsonUtility.FromJson<Graph>(jsonString);
    }
}
