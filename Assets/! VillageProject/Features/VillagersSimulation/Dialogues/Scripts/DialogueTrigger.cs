using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public class DialogueTrigger : MonoBehaviour
{
    [Inject] private DialogueSystem _dialogueSystem;
    [SerializeField] private Villager _villager;
    private IDialogueTarget _target;

    [SerializeField, Range(0, 1)] private float _dialogueChance = 0.6f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (_target != null) return;
        
        if (other.transform.parent != null && other.transform.parent.TryGetComponent(out _target))
        {
            if (_target is Villager villagerTarget) OnTriggerVillager(villagerTarget);
            else
            {
                
            }
        }
    }

    private void OnTriggerVillager(Villager villagerTarget)
    {
        if (_target == _villager || villagerTarget.VillagerData.IsTalking)
        {
            _target = null;
            return;
        }
            
        var randomValue = Random.Range(0f, 1f);
            
        if (randomValue <= _dialogueChance)
        {
            villagerTarget.VillagerData.IsTalking = true;
            _villager.VillagerData.IsTalking = true;
            
            _dialogueSystem.PlayShortDialogue(_villager, villagerTarget, OnDialogueFinished);       
        }
    }

    private void OnTriggerTarget(IDialogueTarget target)
    {
        var randomValue = Random.Range(0f, 1f);

        if (randomValue <= _dialogueChance)
        {
            _villager.VillagerData.IsTalking = true;
            _dialogueSystem.PlayShortDialogue(_villager, target, OnDialogueFinished);   
        }
    }

    private void OnDialogueFinished()
    {
        _villager.VillagerData.IsTalking = false;
        if (_target is Villager villagerTarget) villagerTarget.VillagerData.IsTalking = false;

        _target = null;
    }
}

public interface IDialogueTarget
{
    
}