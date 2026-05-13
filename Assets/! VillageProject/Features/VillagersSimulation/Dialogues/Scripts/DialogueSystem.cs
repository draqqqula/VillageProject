using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class DialogueSystem : MonoBehaviour
{
    private const float BaseReplicaDuration = 2f;
    
    [SerializeField] private DialoguesData _dialoguesData;
    private DialoguesData _dialoguesDataInstance;
    
    private DialogueChooser _dialogueChooser;
    
    private List<DialogueSession> _dialogueSessions = new List<DialogueSession>();
    public List<DialogueSession> DialogueSessions => _dialogueSessions;
    
    public bool IsCanPlayDialogues {get; set;}

    private void Awake()
    {
        _dialoguesDataInstance = ScriptableObject.Instantiate(_dialoguesData);
        _dialogueChooser = new DialogueChooser();
        IsCanPlayDialogues = true;
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
    
    public void PlayShortDialogue(Villager villagerA, IDialogueTarget target, Action callback = null)
    {
        var dialogueID = _dialogueChooser.ChooseDialogueID(villagerA, target, true);
        
        if (target is Villager villagerB) PlayDialogue(villagerA, villagerB, dialogueID, callback);
        else PlayDialogue(villagerA, dialogueID, callback);
    }
    
    public void PlayDialogue(Villager villagerA, Villager villagerB, string id, Action callback = null)
    {
        if (!IsCanPlayDialogues)
        {
            callback?.Invoke();
            return;
        }
        
        var (first, second) = _dialogueChooser.GetSpeakerOrder(villagerA, villagerB, id);
        var dialogue = _dialoguesDataInstance.Dialogues.FirstOrDefault(d => d.DialogueID == id);

        if (dialogue == null)
        {
            Debug.LogError($"Dialogue with index {id} not found!");
            return;
        }
        
        Action<Replica> speakAction = (replica) =>
        {
            if (replica.SpeakerID == Replica.ActorID.ActorA) first.Speak(replica.Text);
            else second.Speak(replica.Text);
        };

        Action silentAction = () =>
        {
            first.KeepSilent();
            second.KeepSilent();
        };

        StopDialogue(first);
        StopDialogue(second);

        var session = new DialogueSession() { Id = id, VillagerA = first, VillagerB = second};
        _dialogueSessions.Add(session);
        
        var coroutine = StartCoroutine(DialogueRoutine(session, dialogue, speakAction, silentAction, callback));
        session.DialogueCoroutine = coroutine;
    }

    public void PlayDialogue(Villager villager, string id, Action callback = null)
    {
        if (!IsCanPlayDialogues)
        {
            callback?.Invoke();
            return;
        }
        
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
        
        var session = new DialogueSession() { Id = id, VillagerA = villager};
        _dialogueSessions.Add(session);
        
        var coroutine = StartCoroutine(DialogueRoutine(session, dialogue, speakAction, silentAction, callback));
        session.DialogueCoroutine = coroutine;
    }
    
    public void StopDialogue(Villager villager)
    {
        var session = _dialogueSessions.FirstOrDefault(s => s.VillagerA == villager || s.VillagerB == villager);
        
        if (session != null)
        {
            session.IsFinished = true;
            
            StopCoroutine(session.DialogueCoroutine);
            session.VillagerA.KeepSilent();
            session.VillagerB.KeepSilent();
            
            _dialogueSessions.Remove(session);
        }
    }

    public void StopAllDialogues()
    {
        foreach (var session in _dialogueSessions)
        {
            if (session != null)
            {
                session.IsFinished = true;
            
                StopCoroutine(session.DialogueCoroutine);
                session.VillagerA.KeepSilent();
                session.VillagerB.KeepSilent();
            }
        }
        
        _dialogueSessions.Clear();
    }

    private IEnumerator DialogueRoutine(DialogueSession session, DialogueConfig dialogue, Action<Replica> speakAction,
        Action silentAction, Action callback)
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
        
        session.IsFinished = true;
        silentAction?.Invoke();
        _dialogueSessions.Remove(session);  
        
        callback?.Invoke();
    }
}

public class DialogueSession
{
    public string Id {get; set;}
    
    public Villager VillagerA { get; set; }
    public Villager VillagerB { get; set; }
    
    public Coroutine DialogueCoroutine { get; set; }
    public bool IsFinished { get; set; }

    public bool IsVillagerInDialog(string villagerKey)
    {
        return (VillagerA != null && villagerKey == VillagerA.VillagerData.Key) ||
               (VillagerB != null && villagerKey == VillagerB.VillagerData.Key);
    }
}