using UnityEngine;

public class MapIcon : MonoBehaviour
{
    private const string Path = "Common/IconsCanvas";

    [SerializeField] private GameObject _prefab;
    private Transform _root;
    private GameObject _instance;

    private void Awake()
    {
        _root = GetComponentInParent<ParentAnchorSystem>().System.transform.Find(Path);
    }

    private void OnEnable()
    {
        _instance = Instantiate(_prefab, _root);
        _instance.GetComponent<WorldToCanvasPosition>().WorldPosition = transform.position;
    }

    private void OnDisable()
    {
        if (_instance == null)
        {
            return;
        }
        Destroy(_instance);
    }
}
