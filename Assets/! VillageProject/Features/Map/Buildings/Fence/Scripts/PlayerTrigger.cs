using R3;
using System.Collections;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public ReadOnlyReactiveProperty<bool> PlayerInside => _playerInside;
    private ReactiveProperty<bool> _playerInside = new ReactiveProperty<bool>(false);

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            _playerInside.Value = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            _playerInside.Value = false;
        }
    }
}