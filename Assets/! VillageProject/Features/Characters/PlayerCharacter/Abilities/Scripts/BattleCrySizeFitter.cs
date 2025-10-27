using UnityEngine;

public class BattleCrySizeFitter : MonoBehaviour
{
    [SerializeField] private float _sizeMultiplier;
    [SerializeField] private bool _isAutoSize;
    
    [SerializeField] private Collider _collider;
    [SerializeField] private Transform _transform;
    
    [Header("BaseParams")]
    [SerializeField] private Vector3 _baseColliderSize;
    [SerializeField] private Vector3 _baseTransformSize;

    #if UNITY_EDITOR
    
    private void OnValidate()
    {
        if (!_isAutoSize) return;

        if (_collider is SphereCollider sphereCollider)
            sphereCollider.radius = _baseColliderSize.x * _sizeMultiplier;
        else if (_collider is CapsuleCollider capsuleCollider)
        {
            capsuleCollider.radius = _baseColliderSize.x * _sizeMultiplier;
            capsuleCollider.height = _baseColliderSize.y * _sizeMultiplier;

            capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y, capsuleCollider.height / 2);
        }
        else if (_collider is BoxCollider boxCollider)
        {
            boxCollider.size = _baseColliderSize * _sizeMultiplier;
        }
        
        _transform.localScale = _baseTransformSize * _sizeMultiplier;
        var battleCryRange = _transform.GetComponent<BattleCryRangeView>();
        battleCryRange.UpdateCurves();
    }

    [ContextMenu("SetBaseSizes")]
    private void SetBaseSizes()
    {
        if (_collider is SphereCollider sphereCollider)
            _baseColliderSize = new Vector3(sphereCollider.radius, 1, 1);
        else if (_collider is CapsuleCollider capsuleCollider)
        {
            _baseColliderSize = new Vector3(capsuleCollider.radius, capsuleCollider.height, 1);
        }
        else if (_collider is BoxCollider boxCollider)
        {
            _baseColliderSize = boxCollider.size;
        }
    }
    
    #endif
}