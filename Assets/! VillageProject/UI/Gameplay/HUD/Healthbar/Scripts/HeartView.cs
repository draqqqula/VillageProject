using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator), typeof(Image))]
public class HeartView : MonoBehaviour
{
    public RectTransform RectTransform => GetComponent<RectTransform>();

    private Animator _animator;
    private Image _image;
    
    [SerializeField] private Sprite _normalHealth;
    [SerializeField] private Sprite _woundedHealth;
    
    public void Awake()
    {
        _animator = GetComponent<Animator>();
        _image = GetComponent<Image>();
    }

    public void SetWoundedHeart()
    {
        _animator.SetInteger("State", 1);
        _image.enabled = true;
    }

    public void SetNormalHeart()
    {
        _animator.SetInteger("State", 0);
        _image.sprite = _normalHealth;
        _image.enabled = true;
    }
    
    public void DisableHeart()
    {
        _animator.SetInteger("State", 2);
    }
}