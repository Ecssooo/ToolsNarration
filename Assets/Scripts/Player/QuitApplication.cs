using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    [DialogueEvent]
    public void QuitApp()
    {
        Application.Quit();
    }
}
