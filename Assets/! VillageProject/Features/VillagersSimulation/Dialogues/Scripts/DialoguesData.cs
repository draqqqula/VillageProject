using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialoguesData", menuName = "Dialogues/DialoguesData")]
public class DialoguesData : ScriptableObject
{
    [field: SerializeField] public List<DialogueConfig> Dialogues { get; set; }
}

[Serializable]
public class DialogueConfig
{
    [field: SerializeField] public string DialogueID {get; set;}
    [field: SerializeField] public List<Replica> Replicas {get; set;}
}

[Serializable]
public class Replica
{
    [field: SerializeField] public string Text {get; set;}
    [field: SerializeField] public ActorID SpeakerID {get; set;}
    [field: SerializeField] public float ReplicaDuration {get; set;}
    
    public enum ActorID {ActorA, ActorB}
}