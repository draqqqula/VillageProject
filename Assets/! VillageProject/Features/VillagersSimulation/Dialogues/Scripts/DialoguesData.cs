using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialoguesData", menuName = "Dialogues/DialoguesData")]
public class DialoguesData : ScriptableObject
{
    [field: SerializeField] private bool _isTurnOnValidate = true;
    [field: SerializeField] public List<DialogueConfig> Dialogues { get; set; }
 
    
    private void OnValidate()
    {
        if (!_isTurnOnValidate) return;
        
        foreach (var dialogueConfig in Dialogues)
        {
            foreach (var replica in dialogueConfig.Replicas)
            {
                if (replica.IsAutoDuration) replica.ReplicaDuration = replica.GetReadTimeByChars(replica.Text);
            }
        }
    }
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
    
    [SerializeField] private bool _isAutoDuration = true;
    public bool IsAutoDuration => _isAutoDuration;
    
    public enum ActorID {ActorA, ActorB}

    [ContextMenu("Generate Duration")]
    public void GenerateDuration()
    {
        ReplicaDuration = GetReadTimeByChars(Text);
    }
    
    public float GetReadTimeByChars(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0.5f;

        int chars = text.Length;

        float time = chars / 12f;
        return Mathf.Clamp(time, 1.0f, 7f);
    }
}