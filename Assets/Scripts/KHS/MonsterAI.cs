using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class MonsterAI : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float meleeAttackRange = 2f;

    [Header("Movement")]
    [SerializeField] private float movSpeed = 4f;
    
    private Vector3 originPos = Vector3.zero;
    private BehaviorTreeRunner runnerBT = null;
    Transform detectedPlayerTr = null;
    private Animator anim = null;

    private const string _ATTACK_ANIM_STATE_NAME = "Attack";
    private const string _ATTACK_ANIM_BOOL_NAME = "attack";
    private const string _DETECT_ANIM_BOOL_NAME = "detect";

    private void Awake()
    {
        anim = GetComponent<Animator>();
        runnerBT = new BehaviorTreeRunner(SettingBT());
        originPos = transform.position;
    }

    private void Update()
    {
        runnerBT.Operate();
    }

    INode SettingBT()
    {
        return new SelectorNode
            (
                new List<INode>()
                {
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(CheckMeleeAttacking),
                            new ActionNode(CheckEnemyWithinMeleeAttackRange),
                            new ActionNode(DoMeleeAttack),
                        }
                    ),
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(CheckDetectEnemy),
                            new ActionNode(MoveToDetectEnemy),
                        }
                    ),
                    new ActionNode(MoveToOriginPosition)
                }
            );
    }

    bool IsAnimationRunning(string _stateName)
    {
        if(anim != null)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(_stateName))
            {
                var normalizedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

                return normalizedTime != 0 && normalizedTime < 1f;
            }
        }

        return false;
    }

    #region Attack Node
    INode.ENodeState CheckMeleeAttacking()
    {
        if(IsAnimationRunning(_ATTACK_ANIM_STATE_NAME))
        {
            return INode.ENodeState.ENS_Running;
        }
        anim.SetBool(_ATTACK_ANIM_BOOL_NAME, false);
        return INode.ENodeState.ENS_Success;
    }

    INode.ENodeState CheckEnemyWithinMeleeAttackRange()
    {
        if(detectedPlayerTr != null)
        {
            if(Vector3.SqrMagnitude(detectedPlayerTr.position - transform.position) < (meleeAttackRange * meleeAttackRange))
            {
                return INode.ENodeState.ENS_Success;
            }
        }

        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState DoMeleeAttack()
    {
        if(detectedPlayerTr != null)
        {
            anim.SetTrigger(_ATTACK_ANIM_BOOL_NAME);
            return INode.ENodeState.ENS_Success;
        }

        return INode.ENodeState.ENS_Failure;
    }
    #endregion

    #region Detect & Move Node
    INode.ENodeState CheckDetectEnemy()
    {
        var overlapColliders = Physics.OverlapSphere(transform.position, detectRange, LayerMask.GetMask("Player"));

        if(overlapColliders != null && overlapColliders.Length > 0)
        {
            detectedPlayerTr = overlapColliders[0].transform;

            return INode.ENodeState.ENS_Success;
        }

        detectedPlayerTr = null;

        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState MoveToDetectEnemy()
    {
        if(detectedPlayerTr != null)
        {
            if (Vector3.SqrMagnitude(detectedPlayerTr.position - transform.position) < (meleeAttackRange * meleeAttackRange))
            {
                return INode.ENodeState.ENS_Success;
            }

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(detectedPlayerTr.position.x, 0f, detectedPlayerTr.position.z), Time.deltaTime * movSpeed);

            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Failure;
    }
    #endregion

    #region Move Origin Pos Node
    INode.ENodeState MoveToOriginPosition()
    {
        if(Vector3.SqrMagnitude(originPos - transform.position) < float.Epsilon * float.Epsilon)
        {
            return INode.ENodeState.ENS_Success;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, Time.deltaTime * movSpeed);
            return INode.ENodeState.ENS_Running;
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, detectRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, meleeAttackRange);
    }
}
