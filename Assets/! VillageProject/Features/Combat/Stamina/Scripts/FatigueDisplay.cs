using R3;
using UnityEngine;

public class FatigueDisplay : MonoBehaviour
{
    [SerializeField] private Stamina _stamina;
    [SerializeField] private HurtEffect _hurt;

    private void Start()
    {
        _stamina.IsOnCooldown.Subscribe(HandleOnCooldown);
    }

    private void HandleOnCooldown(bool value)
    {
        if (value)
        {
            _hurt.Show();
        }
    }
}
