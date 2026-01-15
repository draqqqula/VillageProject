using UnityEngine;

public class WeakSpotOpenAtStart : MonoBehaviour
{
    [SerializeField] private WeakSpotController _weakSpotController;

    void Start()
    {
        _weakSpotController.Open();
    }
}
