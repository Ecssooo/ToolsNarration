using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BoolStateBlackboard : MonoBehaviour, IBoolStateProvider
{
    [Serializable] public struct Entry
    {
        [FormerlySerializedAs("key")] public string Key;
        public bool Value;
    }

    [SerializeField] private List<Entry> entries = new();

    public bool GetBool(string key)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].Key == key)
                return entries[i].Value;
        }

        return false;
    }

    public void SetBool(string key, bool value)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].Key == key)
            {
                var e = entries[i];
                e.Value = value;
                entries[i] = e;
                return;
            }
        }
        entries.Add(new Entry{Key = key, Value = value});
    }
}
