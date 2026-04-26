using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    private const float BaseReplicaDuration = 2f;
    
    [SerializeField] private DialoguesData _dialoguesData;
    private DialoguesData _dialoguesDataInstance;
    
    private DialogueChooser _dialogueChooser;
    private Coroutine _dialogueCoroutine;
    private List<DialogueSession> _dialogueSessions = new List<DialogueSession>();

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

    public void PlayShortDialogue(Villager villagerA, Villager villagerB, Action callback = null)
    {
        var dialogueID = _dialogueChooser.ChooseDialogueID(villagerA, villagerB, true);
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
        
        Action<Replica> speakAction = (replica) =>
        {
            if (replica.SpeakerID == Replica.ActorID.ActorA) villagerA.Speak(replica.Text);
            else villagerB.Speak(replica.Text);
        };

        Action silentAction = () =>
        {
            villagerA.KeepSilent();
            villagerB.KeepSilent();
        };

        StopDialogue(villagerA);
        StopDialogue(villagerB);
        var coroutine = StartCoroutine(DialogueRoutine(dialogue, speakAction, silentAction, callback));
        _dialogueSessions.Add(new DialogueSession() {VillagerA = villagerA, VillagerB = villagerB, DialogueCoroutine = coroutine});
    }

    public void PlayDialogue(Villager villager, string id, Action callback = null)
    {
        var dialogue = _dialoguesDataInstance.Dialogues.FirstOrDefault(d => d.DialogueID == id);

        if (dialogue == null)
        {
            Debug.LogError($"Dialogue with index {id} not found!");
            return;
        }
        
        Action<Replica> speakAction = (replica) =>
        {
            villager.Speak(replica.Text);
        };

        Action silentAction = () =>
        {
            villager.KeepSilent();
        };
        
        StopDialogue(villager);
        var coroutine = StartCoroutine(DialogueRoutine(dialogue, speakAction, silentAction, callback));
        _dialogueSessions.Add(new DialogueSession() {VillagerA = villager, DialogueCoroutine = coroutine});
    }
    
    public void StopDialogue(Villager villager)
    {
        var session = _dialogueSessions.FirstOrDefault(s => s.VillagerA == villager || s.VillagerB == villager);
        
        if (session != null)
        {
            StopCoroutine(session.DialogueCoroutine);
            _dialogueSessions.Remove(session);
        }
    }

    private IEnumerator DialogueRoutine(DialogueConfig dialogue, Action<Replica> speakAction, Action silentAction, Action callback)
    {
        var curReplicaIndex = 0;
        while (curReplicaIndex < dialogue.Replicas.Count)
        {
            var replica = dialogue.Replicas[curReplicaIndex];
            speakAction?.Invoke(replica);
            
            if (replica.ReplicaDuration > 0) yield return new WaitForSeconds(replica.ReplicaDuration);
            else yield return new WaitForSeconds(BaseReplicaDuration);
            curReplicaIndex++;
            
            silentAction?.Invoke();
        }
        
        _dialogueCoroutine = null;
        callback?.Invoke();
    }
}

public class DialogueSession
{
    public Villager VillagerA { get; set; }
    public Villager VillagerB { get; set; }
    public Coroutine DialogueCoroutine { get; set; }
}

public class DialogueChooser
{
    public string ChooseDialogueID(Villager villagerA, Villager villagerB, bool isShort = false)
    { 
        if (isShort) return "ShortTestDialogue";
        return "TestDialogue";
    }
}