using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnchorMode : MonoBehaviour
{
    private ReactiveProperty<Anchor> _active = new ReactiveProperty<Anchor>(null);
    [SerializeField] private Anchor _defaultAnchor;
    public ReadOnlyReactiveProperty<Anchor> ActiveAnchor => _active;

    private void OnEnable()
    {
        Time.timeScale = 0.0f;

        MoveTo(_defaultAnchor);
    }

    private void OnDisable()
    {
        Time.timeScale = 1.0f;

        if (_active.Value != null)
        {
            SetLocalActive(false);
        }
        _active.Value = null;
    }

    public void MoveTo(Anchor anchor)
    {
        if (_active.Value != null)
        {
            SetLocalActive(false);
        }
        _active.Value = anchor;
        SetLocalActive(true);
        var position = anchor.transform.position;
    }

    private void SetLocalActive(bool value)
    {
        _active.Value.Active.Value = value;
    }
}
