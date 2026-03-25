using UnityEngine;

public class MapIcon : MonoBehaviour
{
    private const string Path = "Common/IconsCanvas";

    [SerializeField] private GameObject _prefab;
    private Transform _root;
    protected GameObject _instance;

    protected virtual void Awake()
    {
        _root = GetComponentInParent<ParentAnchorSystem>().System.transform.Find(Path);
    }
    
    protected void InstantiateIcon()
    {
        _instance = Instantiate(_prefab, _root);
        _instance.GetComponent<WorldToCanvasPosition>().WorldPosition = transform.position;
    }

    protected void DestroyIcon()
    {
        if (_instance == null)
        {
            return;
        }
        Destroy(_instance);
    }
    
    protected virtual void OnEnable()
    {
        InstantiateIcon();
    }

    protected virtual void OnDisable()
    {
        DestroyIcon();
    }
}