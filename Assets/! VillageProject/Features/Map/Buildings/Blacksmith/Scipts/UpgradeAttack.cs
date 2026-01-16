using UnityEngine;
using Zenject;

public class UpgradeAttack : MonoBehaviour
{
    [SerializeField] private float _multiplier;
    [Inject] AttackBonus _bonus;

    public void Start()
    {
        _bonus.DamageMultiplier = _multiplier;
    }
}
