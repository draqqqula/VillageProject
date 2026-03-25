using UnityEngine;

public class Building : MonoBehaviour
{
    [field: SerializeReference, SubclassSelector] public BuildingData Data { get; private set; }
    [field: SerializeField] private GameObject _view;

    public bool IsViewActivated => _view.activeSelf;

    public void ActivateView()
    {
        if (IsViewActivated) return;
        _view.SetActive(true);
    }
    
    public void DeactivateView()
    {
        if (!IsViewActivated) return;
        _view.SetActive(false);
    }
}