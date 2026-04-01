using System.Linq;
using UnityEngine;
using Zenject;

public class SkinChanger
{
    private VillagersSkinsInfo _skinsInfo;
    private IInstantiator _instantiator;

    public SkinChanger(VillagersSkinsInfo skinsInfoInstance, IInstantiator instantiator)
    {
        _skinsInfo = skinsInfoInstance;
        _instantiator = instantiator;
    }
    
    public GameObject CreateSkin(Transform parent, Gender gender, ProfessionType profession)
    {
        var skin = _skinsInfo.SkinConfigs.FirstOrDefault(s => s.Gender == gender && s.Profession == profession);
        if (skin == null)
        {
            skin = _skinsInfo.SkinConfigs[0];
            Debug.LogError($"Skin with {gender} {profession} not found!");
        }

        var skinInstance = _instantiator.InstantiatePrefab(skin.Prefab, parent);
        return skinInstance;
    }
}