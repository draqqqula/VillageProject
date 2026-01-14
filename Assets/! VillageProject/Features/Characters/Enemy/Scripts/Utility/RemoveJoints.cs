using System.Collections.Generic;
using UnityEngine;

public class RemoveJoints : MonoBehaviour
{
    [SerializeField] private List<CharacterJoint> _joints;

    public void DetachAll()
    {
        foreach (var joint in _joints)
        {
            Destroy(joint);
        }
    }

    [ContextMenu("fill")]
    private void Fill()
    {
        GetComponentsInChildren(_joints);
    }
}
