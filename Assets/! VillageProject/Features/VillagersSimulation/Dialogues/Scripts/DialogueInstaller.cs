using UnityEngine;
using Zenject;

public class DialogueInstaller : MonoInstaller<DialogueInstaller>
{
    [SerializeField] private DialogueSystem _dialogueSystem;
    [SerializeField] private Canvas _dialogueCanvas;
    
    public override void InstallBindings()
    {
        Container.BindInstance(_dialogueSystem).AsSingle();
        Container.BindInstance(_dialogueCanvas).WithId("Dialogue").AsCached();
    }
}