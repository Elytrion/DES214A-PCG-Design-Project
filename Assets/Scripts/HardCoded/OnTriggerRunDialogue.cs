using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerRunDialogue : MonoBehaviour
{
    public int DialogueIndexToRun;
    public HardCodedCutscene _dialogueManager;

    public bool HasntRun = true;

    void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.tag == "Player") && HasntRun)
        {
            _dialogueManager.RunDialogueScene(DialogueIndexToRun);
            HasntRun = false;
        }
    }
}
