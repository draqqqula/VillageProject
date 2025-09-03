using R3;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AnchorLinks))]
public class Anchor : MonoBehaviour
{
    [field: SerializeField] public AnchorLinks Links { get; set; }
    public ReactiveProperty<bool> Active { get; private set; } = new ReactiveProperty<bool>(false);

    private void Reset()
    {
        Links = GetComponent<AnchorLinks>();
        if (Links == null)
        {
            Links = gameObject.AddComponent<AnchorLinks>();
        }
    }
}