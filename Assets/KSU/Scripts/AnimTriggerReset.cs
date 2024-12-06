using UnityEngine;

// Trigger ���� �ʱ�ȭ ��Ű�� ��ũ��Ʈ
public class AnimTriggerReset : StateMachineBehaviour
{
    [SerializeField] string triggerName;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
    }
}
