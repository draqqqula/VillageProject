using UnityEngine;
using Zenject;

public class TutorialRules : MonoBehaviour
{
    [SerializeField] private int _counter = 3;
    [Inject] private MatchState _r;

    public void CompleteObjective()
    {
        _counter -= 1;
        if (_counter == 0)
        {
            _r.DeclareVictory();
        }
    }
}
