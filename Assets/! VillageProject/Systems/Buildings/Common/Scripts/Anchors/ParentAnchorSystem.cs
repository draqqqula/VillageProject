using System.Collections;
using System.Linq;
using UnityEngine;

public class ParentAnchorSystem : MonoBehaviour
{
    [field: SerializeField] public AnchorMode System { get; private set; }
    private void Reset()
    {
        System = UnityEngine.SceneManagement.SceneManager
            .GetActiveScene()
            .GetRootGameObjects()
            .FirstOrDefault(it => it.GetComponentInChildren<AnchorMode>(true) != null)
            .GetComponentInChildren<AnchorMode>(true);
    }
}