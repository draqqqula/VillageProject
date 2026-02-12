using R3;
using UnityEngine;
using Zenject;

public class ShoppingZone : MonoBehaviour
{
    [Inject] private WaveController _waveController;
    public ReactiveProperty<bool> PlayerInside { get; private set; } = new ReactiveProperty<bool>(true);
}
