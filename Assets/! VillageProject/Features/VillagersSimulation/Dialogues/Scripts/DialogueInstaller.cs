using UnityEngine;
using Zenject;

public class DialogueInstaller : MonoInstaller<DialogueInstaller>
{
    [SerializeField] private DialogueSystem _dialogueSystem;
    
    public override void InstallBindings()
    {
        Container.Bind<DialogueSystem>().FromInstance(_dialogueSystem).AsSingle();
    }
}