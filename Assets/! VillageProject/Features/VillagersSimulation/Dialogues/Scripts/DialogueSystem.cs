using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    private const float BaseReplicaDuration = 2f;
    
    [SerializeField] private DialoguesData _dialoguesData;
    private DialoguesData _dialoguesDataInstance;
    
    private DialogueChooser _dialogueChooser;
    private Coroutine _dialogueCoroutine;

    private void Awake()
    {
        _dialoguesDataInstance = ScriptableObject.Instantiate(_dialoguesData);
        _dialogueChooser = new DialogueChooser();
    }

    public void PlayDialogue(Villager villagerA, Villager villagerB, Action callback = null)
    {
        var dialogueID = _dialogueChooser.ChooseDialogueID(villagerA, villagerB);
        PlayDialogue(villagerA, villagerB, dialogueID, callback);
    }
    
    public void PlayDialogue(Villager villagerA, Villager villagerB, string id, Action callback = null)
    {
        var dialogue = _dialoguesDataInstance.Dialogues.FirstOrDefault(d => d.DialogueID == id);

        if (dialogue == null)
        {
            Debug.LogError($"Dialogue with index {id} not found!");
            return;
        }

        if (_dialogueCoroutine != null) StopCoroutine(_dialogueCoroutine);
        _dialogueCoroutine = StartCoroutine(DialogueRoutine(villagerA, villagerB, dialogue, callback));
    }
    
    public void StopDialogue()
    {
        if (_dialogueCoroutine != null)
        {
            StopCoroutine(_dialogueCoroutine);
            _dialogueCoroutine = null;
        }
    }

    private IEnumerator DialogueRoutine(Villager villagerA, Villager villagerB, DialogueConfig dialogue, Action callback)
    {
        var curReplicaIndex = 0;
        while (curReplicaIndex < dialogue.Replicas.Count)
        {
            var replica = dialogue.Replicas[curReplicaIndex];
            
            if (replica.SpeakerID == Replica.ActorID.ActorA) villagerA.Speak(replica.Text);
            else villagerB.Speak(replica.Text);
            
            if (replica.ReplicaDuration > 0) yield return new WaitForSeconds(replica.ReplicaDuration);
            else yield return new WaitForSeconds(BaseReplicaDuration);
            curReplicaIndex++;
        }
        
        _dialogueCoroutine = null;
        callback?.Invoke();
    }
}

public class DialogueChooser
{
    public string ChooseDialogueID(Villager villagerA, Villager villagerB)
    {
        return "TestDialogue";
    }
}