using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootPlanter : MonoBehaviour
{
    [SerializeField] private LayerMask _mask;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _rayHeight;


    private void Reset()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        PlaceLeg(AvatarIKGoal.LeftFoot);
        PlaceLeg(AvatarIKGoal.RightFoot);
    }

    private void PlaceLeg(AvatarIKGoal goal)
    {
        var footPosition = _animator.GetIKPosition(goal);
        var ray = new Ray(
            new Vector3(footPosition.x, transform.position.y, footPosition.z), 
            Vector3.down);
        if (Physics.Raycast(ray, out var hit, _rayHeight, _mask)
            && hit.point.y < footPosition.y)
        {
            _animator.SetIKPositionWeight(goal, 1f);
            _animator.SetIKPosition(goal, hit.point);
        }
    }
}
