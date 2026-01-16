using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NpcDialogueSelector : MonoBehaviour
{
    [Serializable]
    public struct BoolCondition
    {
        public string Key;
        public bool Expected;

        public bool Evaluate(IBoolStateProvider provider)
        {
            if (provider == null) return false;
            return provider.GetBool(Key) == Expected;
        }
    }

    [Serializable]
    public struct FlagSet
    {
        public bool Enabled;
        public string Key;
        public bool Value;

        public void Apply(IBoolStateProvider provider)
        {
            if (!Enabled || provider == null) return;
            provider.SetBool(Key, Value);
        }
    }
    
    [Serializable]
    public struct ConditionalStart
    {
        public string StartNodeId;
        public BoolCondition Condition;
        public FlagSet SetFlagOnPlay;
    }
    
    [SerializeField] private TextAsset dialogueDatabaseJson;

    [Header("Rules (first valid wins)")] 
    [SerializeField] private List<ConditionalStart> rules = new();

    [Header("Fallback")] 
    [SerializeField] private string defaultStartNodeId;

    [Header("State Provider")] 
    [SerializeField] private MonoBehaviour stateProviderBehaviour;

    [Header("Dialogue UI")] 
    [SerializeField] private VisualisedDialogue visualisedDialogue;
    
    IBoolStateProvider Provider => stateProviderBehaviour as IBoolStateProvider;

    public void Interact()
    {
        if (visualisedDialogue == null) return;

        var provider = Provider;
        var picked = Pick(provider);

        var startId = !string.IsNullOrEmpty(picked.StartNodeId) ? picked.StartNodeId : defaultStartNodeId;
        if (string.IsNullOrEmpty(startId)) return;
        
        visualisedDialogue.StartDialogue(startId);
        picked.SetFlagOnPlay.Apply(provider);
    }

    ConditionalStart Pick(IBoolStateProvider provider)
    {
        for (int i = 0; i < rules.Count; i++)
        {
            var r = rules[i];
            if (string.IsNullOrEmpty(r.StartNodeId)) continue;
            if (r.Condition.Evaluate(provider))
                return r;
        }

        return default;
    }
}
