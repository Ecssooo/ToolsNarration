using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextAsset dialogueJsonFile;


    [ContextMenu("Load Dialogue Data from JSON")]
    public void LoadDialogueDataFromJson()
    {
        DialogueData dialogueData = DialogueData.Instance;
        if (dialogueData != null)
        {
            dialogueData.Graph = JsonManager.JsonToDialogueData(dialogueJsonFile.text);
            Debug.Log("Dialogue Data loaded from JSON.");
        }
        else
        {
            Debug.LogWarning("Dialogue Data instance already exists. Loading from JSON skipped.");
        }
    }
}
