using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class AnimationWindowListener : MonoBehaviour
{
    [field: SerializeField] public AnimationWindow Window { get; private set; }

    public event Action OnEntered;
    public event Action OnExited;

    private void Reset()
    {
        enabled = false;
    }

    private void OnEnable()
    {
        OnEntered?.Invoke();
    }

    private void OnDisable()
    {
        OnExited?.Invoke();
    }
}
