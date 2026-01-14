using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueEventConfig", menuName = "Dialogue/Dialogue Event Config")]
public class DialogueEventConfig : ScriptableObject
{
    [Serializable]
    public class Binding
    {
        public string eventId;      // string du JSON (ex: "ChangeBGColor")
        public string typeName;     // type complet (ex: "MyGame.GameEvents")
        public string methodName;   // nom de la méthode (ex: "ChangeBackgroundColor")
    }

    public List<Binding> bindings = new List<Binding>();
}
