using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public class PlayerDialogueTrigger : MonoBehaviour
{
    [Inject] private DialogueView _dialogueView;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.TryGetComponent(out Villager villager))
        {
            _dialogueView.AddNearbyVillager(villager);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.TryGetComponent(out Villager villager))
        {
            _dialogueView.RemoveNearbyVillager(villager);
        }
    }
}