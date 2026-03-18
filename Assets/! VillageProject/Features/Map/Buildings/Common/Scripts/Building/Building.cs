using UnityEngine;

public class Building : MonoBehaviour
{
    [field: SerializeReference, SubclassSelector] public BuildingData Data { get; private set; }
}