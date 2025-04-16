using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    [SerializeField] private CombatController _combatController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!EnsureCombatController(animator))
        {
            return;
        }
        _combatController.HandleAttackAnimationStarted();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!EnsureCombatController(animator))
        {
            return;
        }
        _combatController.HandleAttackAnimationEnded();
    }

    private bool EnsureCombatController(Animator animator)
    {
        if (_combatController == null)
        {
            _combatController = animator.GetComponentInParent<CombatController>();
            if (_combatController == null)
            {
                return false;
            }
        }
        return true;
    }
}
