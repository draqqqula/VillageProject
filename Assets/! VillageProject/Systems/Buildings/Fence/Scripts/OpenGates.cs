using UnityEngine;

public class OpenGates : MonoBehaviour
{
    [SerializeField] private GateState _state;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            _state.Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            _state.Close();
        }
    }
}
