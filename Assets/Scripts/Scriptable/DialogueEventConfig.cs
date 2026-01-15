using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueEventConfig", menuName = "Dialogue/Dialogue Event Config")]
public class DialogueEventConfig : ScriptableObject
{
    [Serializable]
    public class BindingEvent
    {
        public string eventId;      // string du JSON (ex: "ChangeBGColor")
        public string typeName;     // type complet (ex: "MyGame.GameEvents")
        public string methodName;   // nom de la méthode (ex: "ChangeBackgroundColor")
    }

    [Serializable]
    public class BindingCondition
    {
        public string eventId;      // string du JSON (ex: "IsPlayerHealthy")
        public string typeName;     // type complet (ex: "MyGame.GameConditions")
        public string fieldName;   // nom de la field (ex: "CheckPlayerHealth")
    }

    public List<BindingEvent> eventsBindings = new List<BindingEvent>();
    public List<BindingCondition> conditionBindings = new List<BindingCondition>();
}
