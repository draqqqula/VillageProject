using UnityEngine;
using UnityEngine.UI;

public class BuildPlanDisplay : MonoBehaviour
{
    [SerializeField] private Image _progressImage;
    [SerializeField] private GameObject _lockShadow;
    
    public void UpdateBuildProgress(float progress)
    {
        _progressImage.fillAmount = progress;
    }

    public void UnlockPlan()
    {
        _lockShadow.SetActive(false);
    }

    public void LockPlan()
    {
        _lockShadow.SetActive(true);
    }
}