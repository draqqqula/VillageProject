using UnityEngine;
using Zenject;

public abstract class SelfRegistered : MonoBehaviour
{

    public abstract void RegisterSelf(DiContainer container);
}
