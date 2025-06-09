using UnityEngine;

public class DestroyInTimeNoCoroutine : MonoBehaviour
{
    [SerializeField] private float _duration;
    private float _timer = 0;
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _duration )
        {
            Destroy(gameObject);
        }
    }
}
