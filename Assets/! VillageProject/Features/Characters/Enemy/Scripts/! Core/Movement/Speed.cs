using R3;
using UnityEngine;

public class Speed : MonoBehaviour
{
    [SerializeField] private float _defaultSpeed;
    public ReactiveProperty<float> Value { get; private set; } = new ReactiveProperty<float>();

    private void Start()
    {
        Value.Value = _defaultSpeed;
    }
}
