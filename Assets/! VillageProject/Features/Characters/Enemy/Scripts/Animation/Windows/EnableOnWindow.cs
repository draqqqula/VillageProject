using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnableOnWindow : MonoBehaviour
{
    [SerializeField] private AnimationWindowListener _listener;
    [SerializeField] private GameObject _object;

    private void OnEnable()
    {
        _listener.OnEntered += HandleEntered;
        _listener.OnExited += HandleExit;
    }

    private void OnDisable()
    {
        _listener.OnEntered -= HandleEntered;
        _listener.OnExited -= HandleExit;
    }

    private void HandleEntered()
    {
        _object.SetActive(true);
    }

    private void HandleExit()
    {
        _object.SetActive(false);
    }
}
