using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel;
    //public TMP_Text dialogueText; // 或 public Text dialogueText;
    public Text dialogueText;

    private void OnEnable()
    {
        EventHandler.ShowDialogueEvent += OnShowDialogueEvent;
    }

    private void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
    }

    private void OnShowDialogueEvent(string dialogue)
    {
        if (string.IsNullOrEmpty(dialogue))
        {
            dialoguePanel.SetActive(false);
        }
        else
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = dialogue;
        }
    }

    private void Update()
    {
        // 对话显示中，点击鼠标继续
        if (dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            DialogueManager.Instance.NextDialogue();
        }
    }
}
