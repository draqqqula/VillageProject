using System.Collections;
using UnityEngine;

public class DangerLabel : MonoBehaviour
{
    [SerializeField] private float _duration;
    public void Ignite()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(HideOnDelay());
    }

    private IEnumerator HideOnDelay()
    {
        yield return new WaitForSeconds(_duration);
        gameObject.SetActive(false);
    }
}