using System;
using SaveSystem.Attributes;
using UnityEngine;

namespace SaveSystem
{
    [SaveClassAttributes]
    public class SaveTest : MonoBehaviorSavable
    {
        [SerializeField, SaveFieldAttributes] private int _int;
        [SerializeField, SaveFieldAttributes] private float _float;
        [SerializeField, SaveFieldAttributes] private string _string;
        [SerializeField, SaveFieldAttributes] private StructSaveTest _saveTest;
    }

    [Serializable]
    public struct StructSaveTest
    {
        public int _int;
        public float _float;
        public string _string;
    }
}
