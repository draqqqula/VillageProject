using System.Collections;
using UnityEngine;

public class DestroyInTime : MonoBehaviour
{
    [SerializeField] public float _delay = 1f;
    void Start()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(_delay);
        Destroy(gameObject);
    }
}
