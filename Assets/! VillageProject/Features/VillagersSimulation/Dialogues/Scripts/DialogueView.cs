using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textLabel;
    private List<Villager> _nearbyVillagers = new List<Villager>();
    
    [Inject] DialogueSystem _dialogueSystem;
    private DialogueSession _focusedSession;
    
    public void AddNearbyVillager(Villager villager)
    {
        if (_nearbyVillagers.Contains(villager)) return;
        _nearbyVillagers.Add(villager);
    }

    public void RemoveNearbyVillager(Villager villager)
    {
        _nearbyVillagers.Remove(villager);

        if (_focusedSession != null && _focusedSession.IsVillagerInDialog(villager.VillagerData.Key))
        {
            _focusedSession = null;
        }
    }

    public bool TrySetText(string villagerKey, string villagerName, string profession, string text)
    {
        if (_focusedSession != null && !_focusedSession.IsVillagerInDialog(villagerKey)) return false;
        
        if (_nearbyVillagers.Any(v => v.VillagerData.Key == villagerKey))
        {
            if (_focusedSession == null)
            {
                _focusedSession = _dialogueSystem.DialogueSessions.FirstOrDefault(s => s.IsVillagerInDialog(villagerKey));
            }

            SetText(villagerName, profession, text);
            return true;
        }
        return false;
    }
    
    public void SetText(string villagerName, string profession, string text)
    {
        if (_focusedSession == null) return;
        
        gameObject.SetActive(true);
        if (string.IsNullOrEmpty(profession)) _textLabel.text = $"{villagerName}: {text}";
        else _textLabel.text = $"{villagerName} ({profession}): {text}";
    }

    public void HideText()
    {
        if (_focusedSession != null && _focusedSession.IsFinished)
        {
            _focusedSession = null;
        }
        
        gameObject.SetActive(false);
    }
}