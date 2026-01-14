using System;
using SaveSystem.Attributes;
using UnityEngine;

namespace SaveSystem
{
    [SaveClassAttributes("SaveTest")]
    public class SaveTest : MonoBehaviorSavable
    {
        [SerializeField, SaveFieldAttributes("Int")] private int _int;
        [SerializeField, SaveFieldAttributes("Float")] private float _float;
        [SerializeField, SaveFieldAttributes("String")] private string _string;
        [SerializeField, SaveFieldAttributes("Struct")] private StructSaveTest _saveTest;
    }

    [Serializable]
    public struct StructSaveTest
    {
        public int _int;
        public float _float;
        public string _string;
    }
}
