using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    public DialogueData_SO currentDialogue;
    private int currentIndex = 0;

    public void StartDialogue(DialogueData_SO dialogueData)
    {
        currentDialogue = dialogueData;
        currentIndex = 0;
        ShowCurrentDialogue();
    }

    //private void ShowCurrentDialogue()
    //{
    //    if (currentIndex < currentDialogue.dialogueList.Count)
    //    {
    //        string dialogue = currentDialogue.dialogueList[currentIndex];
    //        EventHandler.CallShowDialogueEvent(dialogue);
    //        currentIndex++;
    //    }
    //    else
    //    {
    //        // 对话结束
    //        EventHandler.CallShowDialogueEvent(""); // 清空对话框
    //    }
    //}

    //public void NextDialogue()
    //{
    //    ShowCurrentDialogue();
    //}

    public void NextDialogue()
    {
        ShowCurrentDialogue();
    }

    // 在ShowCurrentDialogue中修改
    private void ShowCurrentDialogue()
    {
        if (currentIndex < currentDialogue.dialogueList.Count)
        {
            string dialogue = currentDialogue.dialogueList[currentIndex];
            EventHandler.CallShowDialogueEvent(dialogue);
            currentIndex++;
        }
        else
        {
            // 对话结束
            EventHandler.CallShowDialogueEvent(""); // 清空对话框
            EventHandler.CallDialogueFinishedEvent(); // 新增：通知对话结束
        }
    }

}
