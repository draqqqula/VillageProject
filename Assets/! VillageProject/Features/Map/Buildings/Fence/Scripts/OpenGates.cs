using UnityEngine;
using R3;

public class OpenGates : MonoBehaviour
{
    [SerializeField] private GateState _state;
    [SerializeField] private PlayerTrigger _trigger;

    private void Awake()
    {
        _trigger.PlayerInside.Subscribe(HandlePlayerInside);
    }

    private void HandlePlayerInside(bool value)
    {
        if (value)
        {
            _state.Open();
            return;
        }
        _state.Close();
    }
}
