using UnityEngine;

public class SetCharacterHealth : MonoBehaviour
{
    [SerializeField] private float _health;

    void Start()
    {
        var target = GameObject.FindAnyObjectByType<FirstPersonController>().gameObject.GetComponent<Health>();
        target.MaxHealth = _health;
        target.Amount = _health;
    }
}
