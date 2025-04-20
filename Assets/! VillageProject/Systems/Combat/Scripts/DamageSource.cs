using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private IDamageComponentProviderFactory _factory = new DefaultDamageComponentProviderFactory();
    [field: SerializeField] public DamageData Data { get; private set; }
    [field: SerializeField] public DamageInfo Info { get; private set; }
    public IDamageComponentProvider ComponentProvider { get; private set; }

    private void Awake()
    {
        IEnumerable<DamageData> ConcatData()
        {
            yield return Info.Data;
            yield return Data;
        }

        ComponentProvider = _factory.Create(ConcatData());
    }
}
