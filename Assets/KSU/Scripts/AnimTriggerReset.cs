using UnityEngine;

// Trigger 상태 초기화 시키는 스크립트
public class AnimTriggerReset : StateMachineBehaviour
{
    [SerializeField] string triggerName;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
    }
}
