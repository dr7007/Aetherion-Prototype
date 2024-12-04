using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossMonsterAI : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] private float level_One_Range = 10f;
    [SerializeField] private float level_Two_Range = 25f;
    [SerializeField] private float level_Three_Range = 50f;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 12f;

    [Header("BossStats")]
    [SerializeField] private float hp = 30000f;
    [SerializeField] private float atk = 30f;
    private enum EPartDemege
    {
        CRIT = 4,
        NORMAL = 2,
        RESIST = 1
    }

    
    private Vector3 originPos = Vector3.zero;
    private BehaviorTreeRunner runnerBT = null;
    private Transform detectedPlayerTr = null;
    private Animator anim = null;
    private float tmpTime = 0f;
    private float offset = 1.5f;

    private const string _ATTACK_ANIM_STATE_NAME = "Attack";
    private const string _ATTACK_ANIM_BOOL_NAME = "attack";
    private const string _FIRSTDETECT_ANIM_BOOL_NAME = "first_detect";
    private const string _DETECT_ANIM_BOOL_NAME = "detect";

    private const string _COUNTER_ANIM_TRIGGER_NAME = "counter";
    private const string _JUDGE_ANIM_TRIGGER_NAME = "judge";
    private const string _RANGE_ANIM_INT_NAME = "range_level";

    private void Awake()
    {
        anim = GetComponent<Animator>();
        runnerBT = new BehaviorTreeRunner(SettingBT());
        originPos = transform.position;
    }
    private void Start()
    {
        anim.SetBool(_FIRSTDETECT_ANIM_BOOL_NAME, true);
        anim.SetBool(_DETECT_ANIM_BOOL_NAME, false);
        anim.SetBool(_ATTACK_ANIM_BOOL_NAME, false);
        anim.SetInteger(_RANGE_ANIM_INT_NAME, 0);
    }
    private void Update()
    {
        runnerBT.Operate();
    }

    private void FixedUpdate()
    {
        DetectPlayer();
        SetJudgeTime();
    }


    private void SetJudgeTime()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("FirstDetect") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("FirstDetectAttack"))
        {
            return; // 전투 애니메이션 중에는 judge를 실행하지 않음
        }
        if (tmpTime >= 3f)
        {
            tmpTime = 0f;

            // Judge 타이밍마다 BT 동작 실행
            runnerBT.Operate();

            // 애니메이션 트리거
            anim.SetTrigger(_JUDGE_ANIM_TRIGGER_NAME);
        }
        else
        {
            tmpTime += Time.deltaTime;
        }
    }
    private void DetectPlayer()
    {
        Collider[] overlapColliders = Physics.OverlapSphere(transform.position, level_Three_Range, LayerMask.GetMask("Player"));

        if (overlapColliders.Length > 0)
        {
            detectedPlayerTr = overlapColliders[0].transform;
            anim.SetBool(_DETECT_ANIM_BOOL_NAME, true);

            // 플레이어와의 거리 확인 후 범위 분류
            float distance = Vector3.Distance(transform.position, detectedPlayerTr.position);
            if (distance <= level_One_Range)
            {
                anim.SetInteger(_RANGE_ANIM_INT_NAME, 1);
            }
            else if (distance <= level_Two_Range)
            {
                anim.SetInteger(_RANGE_ANIM_INT_NAME, 2);
            }
            else if (distance <= level_Three_Range)
            {
                anim.SetInteger(_RANGE_ANIM_INT_NAME, 3);
            }
            else
                anim.SetInteger(_RANGE_ANIM_INT_NAME, 0);
        }
        else
        {
            detectedPlayerTr = null;
            anim.SetBool(_DETECT_ANIM_BOOL_NAME, false);
        }
    }
    public void OnFirstDetectAnimationTrigger()
    {
        anim.SetBool(_FIRSTDETECT_ANIM_BOOL_NAME, false);
    }
    public void FitstAttackTeleport()
    {
        StartCoroutine(FirstAttackTeleportCoroutine());
    }

    private IEnumerator FirstAttackTeleportCoroutine()
    {
        // 애니메이션 일시 정지
        anim.speed = 0f;
        anim.applyRootMotion = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);

        // 플레이어 위치로 이동
        if (detectedPlayerTr != null)
        {
            transform.position = new Vector3(detectedPlayerTr.position.x - offset, 4f, detectedPlayerTr.position.z -offset); // 플레이어 바로 위로 이동
        }
        else
        {
            Debug.LogWarning("Player not detected during teleport!");
        }
        anim.applyRootMotion = true;
        yield return new WaitForSeconds(0.5f); // 특정 지연 시간 (애니메이션 효과를 위한)
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        // 애니메이션 이어서 실행
        anim.speed = 1f;
    }

    INode CreateAttackBehavior()
    {
        return new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckDetectEnemy),
                        new SelectorNode
                        (
                            new List<INode>()
                            {
                                new SequenceNode
                                (
                                    new List<INode>()
                                    {
                                        new ActionNode(CheckRangeLevelOne),
                                        new SelectorNode
                                        (
                                            new List<INode>()
                                            {
                                                new SequenceNode
                                                (
                                                    new List<INode>()
                                                    {
                                                        new ActionNode(PressAttackCheck),
                                                        new ActionNode(OnGuard),
                                                    }
                                                ),
                                                new ActionNode(DefaultMeleeAttackEnemy),
                                            }
                                        ),
                                    }
                                ),
                                new SequenceNode
                                (
                                    new List<INode>()
                                    {
                                        new ActionNode(CheckRangeLevelTwo),
                                        new SelectorNode
                                        (
                                            new List<INode>()
                                            {
                                                new SequenceNode
                                                (
                                                    new List<INode>()
                                                    {
                                                        new ActionNode(IsApproachCheck),
                                                        new ActionNode(UpperAttackEnemy),
                                                    }
                                                ),
                                                new ActionNode(JumpAttackEnemy),
                                            }
                                        ),
                                    }
                                ),
                                new SequenceNode
                                (
                                    new List<INode>()
                                    {
                                        new ActionNode(CheckRangeLevelThree),
                                        new SelectorNode
                                        (
                                            new List<INode>()
                                            {
                                                new SequenceNode
                                                (
                                                    new List<INode>()
                                                    {
                                                        new ActionNode(RandomDecisionCheck),
                                                        new ActionNode(LongRangeAttackEnemy),
                                                    }
                                                ),
                                                new ActionNode(RushEnemy),
                                            }
                                        ),
                                    }
                                ),
                            }
                        ),
                    }
                );
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
                        new ActionNode(FirstDetectCheck),
                        new ActionNode(CheckDetectEnemy),
                    }
                ),
                CreateAttackBehavior(),
                new ActionNode(NonDetect)
            }
        );
    }

    #region Detect Node
    INode.ENodeState FirstDetectCheck()
    {
        if (anim.GetBool(_FIRSTDETECT_ANIM_BOOL_NAME) && anim.GetBool(_DETECT_ANIM_BOOL_NAME))
        {
            return INode.ENodeState.ENS_Success;
        }

        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState CheckDetectEnemy()
    {
        if (anim.GetBool(_DETECT_ANIM_BOOL_NAME) && !anim.GetBool(_FIRSTDETECT_ANIM_BOOL_NAME))
        {
            return INode.ENodeState.ENS_Success;
        }

        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState NonDetect()
    {
        if(!anim.GetBool(_DETECT_ANIM_BOOL_NAME))
        {
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }

    #endregion

    #region Check Node
    INode.ENodeState CheckRangeLevelOne()
    {
        if (anim.GetInteger(_RANGE_ANIM_INT_NAME) == 1)
        {
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState CheckRangeLevelTwo()
    {
        if (anim.GetInteger(_RANGE_ANIM_INT_NAME) == 2)
        {
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState CheckRangeLevelThree()
    {
        if (anim.GetInteger(_RANGE_ANIM_INT_NAME) == 3)
        {
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState PressAttackCheck()
    {
        if(Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger(_COUNTER_ANIM_TRIGGER_NAME);
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState IsApproachCheck()
    {
        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState RandomDecisionCheck()
    {
        return INode.ENodeState.ENS_Failure;
    }
    #endregion

    #region Attack Node
    INode.ENodeState OnGuard()
    {
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState DefaultMeleeAttackEnemy()
    {
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState UpperAttackEnemy()
    {
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState JumpAttackEnemy()
    {
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState LongRangeAttackEnemy()
    {
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState RushEnemy()
    {
        return INode.ENodeState.ENS_Failure;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, level_One_Range);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, level_Two_Range);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, level_Three_Range);
    }
}
