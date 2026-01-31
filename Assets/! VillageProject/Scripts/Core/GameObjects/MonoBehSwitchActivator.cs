using UnityEngine;

public class MonoBehSwitchActivator : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _opposite;

    private void OnEnable()
    {
        _opposite.enabled = false;
    }

    private void OnDisable()
    {
        _opposite.enabled = true;
    }
}