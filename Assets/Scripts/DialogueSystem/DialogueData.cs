using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class DialogueData : MonoBehaviour
{

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
    public List<DialogueEvent> Events;
    public string NextNodeId;
    public List<string> AvailabilityConditions;
}

[Serializable]
public class DialogueEvent
{
    public string Id;
    public List<string> Parameters;
}