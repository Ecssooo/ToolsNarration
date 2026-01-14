using UnityEngine;

public class PNJ : MonoBehaviour
{
    [SerializeField] GameObject pressECanvas;
    [SerializeField] GameObject dialogueCanvas;

    public void ShowInterract(bool show)
    {
        if (pressECanvas != null) pressECanvas.SetActive(show);
    }

    public void DoInteract()
    {
        if (pressECanvas != null) pressECanvas.SetActive(false);
        if (dialogueCanvas != null) dialogueCanvas.SetActive(true);
    }

    public void HideDialogue(bool show)
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(show);
    }
}