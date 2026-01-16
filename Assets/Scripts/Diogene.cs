using UnityEngine;

public class Diogene : MonoBehaviour
{
    [SerializeField] private BoolStateBlackboard blackboard;


    [DialogueEvent]
    public void SetBoolState(string key)
    {
        switch (key)
        {
            case "retourD":
                blackboard.SetBool("ImposerMain", true);
                break;
            case "QuestGived":
                blackboard.SetBool("QuestGived", true);
                break;
        }
    }
}
