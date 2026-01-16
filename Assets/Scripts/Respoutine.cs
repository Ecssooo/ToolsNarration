using UnityEngine;

public class Respoutine : MonoBehaviour
{
    [DialogueCondition] public bool isRaspoutine = false;

    [DialogueEvent]
    public void UnlockRaspoutine()
    {
        isRaspoutine = true;
    }
}
