using System.Collections;
using UnityEngine;

public class SwitchActivator : MonoBehaviour
{
    [SerializeField] private GameObject _opposite;

    private void OnEnable()
    {
        _opposite.SetActive(false);
    }

    private void OnDisable()
    {
        _opposite.SetActive(true);
    }
}