using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AnchorLinks))]
public class Anchor : MonoBehaviour
{
    [field: SerializeField] public AnchorLinks Links { get; set; }

    private void Reset()
    {
        Links = GetComponent<AnchorLinks>();
        if (Links == null)
        {
            Links = gameObject.AddComponent<AnchorLinks>();
        }
    }
}