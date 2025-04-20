using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DetachFromParent : MonoBehaviour
{
    [SerializeField] private float CorpseDespawnTime = 5.0f;
    public void Detach()
    {
        var parent = transform.parent;
        transform.parent = null;
        Destroy(parent.gameObject);
        StartCoroutine(DespawnCorpse());
    }

    private IEnumerator DespawnCorpse()
    {
        yield return new WaitForSeconds(CorpseDespawnTime);
        Destroy(gameObject);
    }
}
