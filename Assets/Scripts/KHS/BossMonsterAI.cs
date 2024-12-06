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
    [SerializeField] private float rotationSpeed = 2f; // 회전 속도

    [Header("BossStats")]
    [SerializeField] private float maxHp = 30000f;
    [SerializeField] private float currentHp;
    [SerializeField] private float atk = 30f;

    [Header("Collider")]
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private float hitCooldown = 1.5f; // 쿨다운 시간

    private enum EPartDemege
    {
        CRIT = 4,
        NORMAL = 2,
        RESIST = 1
    }

    private PlayerMove player = null;

    private bool isAnimationLocked = false; // 애니메이션 도중 방향 고정 여부
    private Quaternion lockedRotation; // 고정된 회전값
    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // 이미 맞은 대상 추적
    private bool isPlayerAttackingOnCooldown = false; // 쿨타임 상태 추적
    private float playerAttackingCooldown = 10f;

    private Vector3 originPos = Vector3.zero;
    private Vector3 firstdetPos = Vector3.zero;
    private BehaviorTreeRunner runnerBT = null;
    private Transform detectedPlayerTr = null;
    private Animator anim = null;
    private float tmpTime = 0f;
    private float offset = 5f;

    public delegate void OnHpChangedDelegate(float currentHp, float maxHp);

    // UI 업데이트를 위한 콜백 이벤트
    private OnHpChangedDelegate hpChangedCallback = null;
    public OnHpChangedDelegate HpChangedCallback
    {
        get {  return hpChangedCallback; }
        set { hpChangedCallback = value; }
    }

    private const string _ATTACK_ANIM_STATE_NAME = "Attack";
    private const string _ATTACK_ANIM_TRIGGER_NAME = "attack";
    private const string _FIRSTDETECT_ANIM_BOOL_NAME = "first_detect";
    private const string _DETECT_ANIM_BOOL_NAME = "detect";

    private const string _COUNTER_ANIM_TRIGGER_NAME = "counter";
    private const string _RANGE_ANIM_INT_NAME = "range_level";
    private const string _PLAYERATTACK_ANIM_TRIGGER_NAME = "playerattacking";
    private const string _DIE_ANUM_TRIGGER_NAME = "die";


    private void Awake()
    {
        currentHp = maxHp;
        anim = GetComponent<Animator>();
        runnerBT = new BehaviorTreeRunner(SettingBT());
        originPos = transform.position;
        player = FindAnyObjectByType<PlayerMove>();
    }
    private void Start()
    {
        anim.SetBool(_FIRSTDETECT_ANIM_BOOL_NAME, true);
        anim.SetBool(_DETECT_ANIM_BOOL_NAME, false);
        anim.ResetTrigger(_ATTACK_ANIM_TRIGGER_NAME);
        anim.SetInteger(_RANGE_ANIM_INT_NAME, 0);
    }
    private void Update()
    {
        runnerBT.Operate();
        if (isAnimationLocked)
        {
            // 애니메이션 도중에는 고정된 방향 유지
            transform.rotation = lockedRotation;
        }
        else if (detectedPlayerTr != null)
        {
            // 애니메이션이 끝난 경우 플레이어 쪽으로 천천히 회전
            LookAtPlayer();
        }
    }

    private void FixedUpdate()
    {
        DetectPlayer();
    }

    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider.CompareTag("PlayerAttack")) // 플레이어 무기에 의해 공격받은 경우
        {
            if (!hitTargets.Contains(_collider))
            {
                Player playerAttack = _collider.GetComponentInChildren<Player>();
                // 대미지 적용
                if (playerAttack != null)
                {
                    float damage = playerAttack.GetDamage(); // 플레이어의 공격력 가져오기
                    Debug.Log("플레이어에 의해 대미지!" + damage);
                    TakeDamage(damage); // 보스에게 대미지 적용
                }

                // 충돌한 대상을 HitTracker에 추가
                hitTargets.Add(_collider);

                // 쿨다운 후 대상 제거
                StartCoroutine(RemoveFromHitTrackerAfterCooldown(_collider));
            }
            else
            {
                Debug.Log("피격 판정 겹침현상 예방 내부 쿨 동작 중");
            }
        }
    }
    private void ApplyDamageToPlayer(Collider _collider)
    {
        Debug.Log(_collider.gameObject.name + "플레이어에게 대미지 적용: " + atk);
    }
    private IEnumerator RemoveFromHitTrackerAfterCooldown(Collider _target)
    {
        yield return new WaitForSeconds(hitCooldown);

        if (hitTargets.Contains(_target))
        {
            hitTargets.Remove(_target);
            Debug.Log("보스 내부 피격 판정 쿨다운 종료");
        }
    }

    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true; // 콜라이더 활성화
        }
    }

    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // 콜라이더 비활성화
        }
    }
    public void EndAttack()
    {
        Debug.Log("EndAttack!");
        anim.ResetTrigger(_ATTACK_ANIM_TRIGGER_NAME);
    }

    public void LockDirection()
    {
        // 현재 회전값을 저장하고 방향 고정 활성화
        lockedRotation = transform.rotation;
        isAnimationLocked = true;
    }

    public void UnlockDirection()
    {
        // 방향 고정 해제
        isAnimationLocked = false;
    }

    private void LookAtPlayer()
    {
        if (detectedPlayerTr == null) return;

        Vector3 direction = (detectedPlayerTr.position - transform.position).normalized;
        direction.y = 0; // 수평 방향만 회전

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void CheckPlayerAttacking()
    {
        if (isPlayerAttackingOnCooldown)
        {
            Debug.Log("PlayerAttacking 트리거가 쿨타임 중입니다.");
            return; // 쿨타임 중일 경우 아무 작업도 하지 않음
        }

        // 플레이어 공격 트리거 활성화
        if (player.IsPlayerAttacking)
        {
            anim.SetTrigger(_PLAYERATTACK_ANIM_TRIGGER_NAME); // 공격 애니메이션 트리거
            Debug.Log("PlayerAttacking 트리거 동작!");

            // 쿨타임 시작
            StartCoroutine(StartPlayerAttackingCooldown());
        }
    }
    private IEnumerator StartPlayerAttackingCooldown()
    {
        isPlayerAttackingOnCooldown = true; // 쿨타임 시작
        Debug.Log("PlayerAttacking 쿨타임 시작!");

        yield return new WaitForSeconds(playerAttackingCooldown);

        player.IsPlayerAttacking = false;
        anim.ResetTrigger(_PLAYERATTACK_ANIM_TRIGGER_NAME);
        isPlayerAttackingOnCooldown = false; // 쿨타임 종료
        Debug.Log("PlayerAttacking 쿨타임 종료! 이제 다시 활성화할 수 있습니다.");
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

    public void FirstDetectRecord()
    {
        firstdetPos = detectedPlayerTr.position;
        Debug.Log(firstdetPos);
    }
    public void OnFirstDetectAnimationTrigger()
    {
        anim.SetBool(_FIRSTDETECT_ANIM_BOOL_NAME, false);
    }
    public void FirstAttackTeleport()
    {
        StartCoroutine(FirstAttackTeleportCoroutine());
    }
    public void OnFirstDetectAnimationEnd()
    {
        UnlockDirection();
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
            transform.position = detectedPlayerTr.position - transform.forward * offset + Vector3.up * 3f; // 플레이어 바로 위로 이동 공격
        }
        else
        {
            transform.position = firstdetPos - transform.forward * offset + Vector3.up * 3f; // 플레이어가 처음 포착된 위치로 이동 공격
        }
        
        anim.applyRootMotion = true;
        yield return new WaitForSeconds(0.5f); // 특정 지연 시간 (애니메이션 효과를 위한)
        LockDirection();
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
                                                        new ActionNode(DefaultMeleeAttackEnemy),
                                                    }
                                                ),
                                                new ActionNode(OnGuard),
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
            Debug.Log("FirstDetect!");
            return INode.ENodeState.ENS_Success;
        }

        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState CheckDetectEnemy()
    {
        if (anim.GetBool(_DETECT_ANIM_BOOL_NAME) && !anim.GetBool(_FIRSTDETECT_ANIM_BOOL_NAME))
        {
            Debug.Log("Detected!");
            return INode.ENodeState.ENS_Success;
        }

        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState NonDetect()
    {
        if(!anim.GetBool(_DETECT_ANIM_BOOL_NAME))
        {
            Debug.Log("Non_Detect");
            anim.ResetTrigger(_ATTACK_ANIM_TRIGGER_NAME);
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
        if(!isPlayerAttackingOnCooldown)
        {
            CheckPlayerAttacking();
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
        if (!anim.GetBool(_ATTACK_ANIM_TRIGGER_NAME) && anim.GetBool(_PLAYERATTACK_ANIM_TRIGGER_NAME))
        {
            anim.SetTrigger(_ATTACK_ANIM_TRIGGER_NAME);
            Debug.Log("보스가 근접 공격을 실행합니다.");
            return INode.ENodeState.ENS_Success;
        }

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

    public float GetDamage()
    {
        return atk;
    }

    public void TakeDamage(float _damage)
    {
        currentHp -= _damage;
        currentHp = Mathf.Max(0, currentHp); // 체력이 0 이하로 내려가지 않도록 처리

        Debug.Log($"Boss took damage: {_damage}, Current HP: {currentHp}");

        // UI 업데이트를 위해 콜백 호출
        HpChangedCallback?.Invoke(currentHp, maxHp);

        // 보스 사망 처리
        if (currentHp <= 0)
        {
            anim.SetTrigger(_DIE_ANUM_TRIGGER_NAME);
        }
    }
    public void DieCall()
    {
        Destroy(gameObject, 5f);
    }
}
