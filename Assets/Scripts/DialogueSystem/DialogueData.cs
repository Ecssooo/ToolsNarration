using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueData : MonoBehaviour
{
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