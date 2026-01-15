using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPortal : MonoBehaviour
{
    [SerializeField] private string _nextScene;

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(_nextScene);
    }
}
