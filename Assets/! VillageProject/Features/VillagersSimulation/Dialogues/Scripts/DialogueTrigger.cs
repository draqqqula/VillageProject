using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public class DialogueTrigger : MonoBehaviour
{
    [Inject] private DialogueSystem _dialogueSystem;
    [SerializeField] private Villager _villager;
    private Villager _triggerVillager;

    [SerializeField, Range(0, 1)] private float _dialogueChance = 0.6f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (_triggerVillager != null) return;
        
        if (other.transform.parent != null && other.transform.parent.TryGetComponent(out _triggerVillager))
        {
            if (_triggerVillager == _villager || _triggerVillager.VillagerData.IsTalking)
            {
                _triggerVillager = null;
                return;
            }
            
            var randomValue = Random.Range(0f, 1f);
            
            if (randomValue <= _dialogueChance)
            {
                _triggerVillager.VillagerData.IsTalking = true;
                _villager.VillagerData.IsTalking = true;
            
                _dialogueSystem.PlayShortDialogue(_villager, _triggerVillager, OnDialogueFinished);       
            }
        }
    }

    private void OnDialogueFinished()
    {
        _villager.VillagerData.IsTalking = false;
        _triggerVillager.VillagerData.IsTalking = false;

        _triggerVillager = null;
    }
}