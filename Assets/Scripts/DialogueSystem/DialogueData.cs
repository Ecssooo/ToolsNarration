using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class DialogueData : MonoBehaviour
{
    public static DialogueData Instance;
    [SerializeField] private Graph graph;
    public Graph Graph
    {
        get { return graph; }
        set { graph = value; }
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
        }
    }
}

[Serializable]
public class Graph
{
    public string StartNodeId;
    public List<Node> Nodes;
}

[Serializable]
public class Node
{
    public string NodeId;
    public int NodeType;
    public string SpeakerId;
    public List<string> Text;
    public List<Choices> Choices;
}


[Serializable]
public class Choices
{
    public string ChoiceId;
    public string ChoiceText;
    public List<string> Events;
    public string NextNodeId;
    public List<string> AvailabilityConditions;
}