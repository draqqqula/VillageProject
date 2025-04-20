using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animation Window", menuName = "Animation Window")]
public class AnimationWindow : ScriptableObject
{
    [SerializeField] private string _name;
}