using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualisedDialogue : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private RectTransform choicesContent;
    [SerializeField] private Button choiceButtonPrefab;

    private string currentNodeId;

    public void StartDialogue()
    {
        currentNodeId = DialogueManager.Instance.Graph.StartNodeId;
        UpdateUI(currentNodeId);
    }

    private void UpdateUI(string nodeId)
    {
        Node node = DialogueManager.Instance.GetNodeByID(nodeId);
        currentNodeId = node.NodeId;

        speakerText.text = node.SpeakerId;

        if (node.Text != null && node.Text.Count > 0)
            dialogueText.text = string.Join("\n", node.Text);
        else
            dialogueText.text = "";

        ClearChoices();

        if (node.Choices == null) return;

        for (int i = 0; i < node.Choices.Count; i++)
        {
            var choiceData = node.Choices[i];
            var btn = Instantiate(choiceButtonPrefab, choicesContent);

            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = choiceData.ChoiceText;

            string nextId = choiceData.NextNodeId;
            btn.onClick.AddListener(() =>
            {
                TriggerEvents(choiceData);
                OnChoiceSelected(nextId);
            });
        }
    }

    private void OnChoiceSelected(string nextNodeId)
    {
        UpdateUI(nextNodeId);
    }

    private void TriggerEvents(Choices choice)
    {
        if (choice.Events == null) return;

        foreach (var dialogueEvent in choice.Events)
        {
            DialogueEventRunner.Instance.TriggerEvent(dialogueEvent.Id);
        }
    }

    private void ClearChoices()
    {
        for (int i = choicesContent.childCount - 1; i >= 0; i--)
            Destroy(choicesContent.GetChild(i).gameObject);
    }
}
