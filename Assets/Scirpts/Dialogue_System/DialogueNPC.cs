using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    public DialogueDataSO myDialogue;                   //npc ���� ���� ��ȭ ������
    private DialogueManager dialogueManager;            //��ȭ �Ŵ��� ���� 
    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();  

        if(dialogueManager == null)
        {
            Debug.LogError("���̾�α� �Ŵ����� �����ϴ�. ");
        }
    }
    void OnMouseDown()                  //���콺�� NPC�� Ŭ�� ������
    {
        if (dialogueManager == null) return;                        //�Ŵ����� ������ ���� ����
        if (dialogueManager.IsDialogueActive()) return;             //�̹� ��ȭ ���̸� ���� ����
        if (myDialogue == null) return;                             //��ȭ �����Ͱ� ������ ���� ����

        dialogueManager.StartDialogue(myDialogue);                  //��� ������ �����Ǹ� �� ��ȭ ����
    }
}
