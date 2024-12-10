using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossMonsterAI : MonoBehaviour
{
    #region Inspector Controll Region

    public Material bossHit;                                        // Boss Hit시 직관적 피드백을 위한 Material 호출.   (Inspector 설정)

    [Header("Range")]
    [SerializeField] private float level_One_Range = 10f;           // 1번째 보스인식 범위 (가까운 인식 영역)
    [SerializeField] private float level_Two_Range = 25f;           // 2번째 보스인식 범위 (중간 인식 영역)
    [SerializeField] private float level_Three_Range = 50f;         // 3번째 보스인식 범위 (먼 인식 영역)

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;                  // 보스의 걷는 속도
    [SerializeField] private float runSpeed = 12f;                  // 보스의 뛰는 속도
    [SerializeField] private float rotationSpeed = 2f;              // 플레이어를 LookAt할 때의 회전 속도

    [Header("BossStats")]
    [SerializeField] private float maxHp = 30000f;                  // 보스의 최대 체력
    [SerializeField] private float currentHp;                       // 보스의 현제 체력
    [SerializeField] private float atk = 30f;                       // 보스의 공격력

    [Header("Collider")]
    [SerializeField] private Collider weaponCollider;               // 보스 무기의 콜라이더 (공격시 On/Off를 위함)
    [SerializeField] private float hitCooldown = 0.2f;              // 공격 판정 시 중복 판정을 제거하기 위한 내부 쿨타임.

    #endregion

    #region Delegate Region
    
    public delegate void OnHpChangedDelegate(float currentHp, float maxHp);     // Boss HP 변화값을 UI에 전달하기 위한 Delegate

    #endregion

    #region Callback Region

    private OnHpChangedDelegate hpChangedCallback = null;                       // Boss HP 변화값을 UI에 전달하기 위한 Callback


    public OnHpChangedDelegate HpChangedCallback
    {
        get {  return hpChangedCallback; }
        set { hpChangedCallback = value; }
    }

    #endregion

    #region Buffer Region

    private BehaviorTreeRunner runnerBT = null;                     // Boss에 대한 Behavior Tree Runner 버퍼
    private PlayerMove player = null;                               // 플레이어의 정보 (위치, 공격력, 공격 입력 여부 등)을 받아오기 위한 버퍼
    private Quaternion lockedRotation;                              // 애니메이션 시작 시 고정된 로테이션 값 버퍼
    private Vector3 firstdetPos = Vector3.zero;                     // 플레이어를 FirstDetect한 첫 지점 좌표 버퍼
    private Animator anim = null;                                   // Boss의 Animator 버퍼
    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // 공격 중복 판정 무시 처리를 위한 hitTargets 관리 버퍼
    private Transform detectedPlayerTr = null;                      // Boss가 Detect상태일때의 실시간 플레이어 Transfrom 정보 버퍼. 

    #endregion

    #region CoolDown Region

    private float respAtkCooldown = 10f;                    // 플레이어 공격 상태 추격 쿨타임.
    private float tmpTime = 0f;                                     // Boss의 특정 행동 쿨타임.

    #endregion

    #region Judge Bool Region

    private bool isAnimationLocked = false;                         // 애니메이션 도중 진행 방향 고정 트리거.         (Animator Parameter로 변경 예정)
    private bool isrespAtkOnCooldown = false;               // 플레이어 공격 상태 추격 트리거.               (Animator Parameter로 변경 예정)

    #endregion

    #region Functional values

    private float offset = 5f;                                      // Boss의 FirstDetect Attack을 위한 offset 값.
    private int rangeLv = 0;

    #endregion

    #region Animator Parameters

    // 애니메이션 상태 이름
    private const string _FIRSTDETECT_ANIM_STATE_NAME = "FirstDetectAttack";
    private const string _RESPATK_ANIM_STATE_NAME = "ResponsiveAttack";

    // 애니메이터 Bool Parameter
    private const string _FIRSTDETECT_ANIM_BOOL_NAME = "FirstDetect";           // Boss의 시작연출 관리를 위한 First Detect 체크 트리거 (게임 시작 후 첫 Detect 이전까지 On, 첫 Detect 이후 Off)
    private const string _DETECT_ANIM_BOOL_NAME = "Detect";                     // Boss의 실시간 Player Detect 체크 트리거 (범위 내 Player가 존재 시 On, 미존재 시 Off)
    private const string _JUDGE_ANIM_BOOL_NAME = "Judge";

    // 애니메이터 Int Parameter
    private const string _RANGE_ANIM_INT_NAME = "RangeLevel";                   // Boss의 기준으로 Player가 Detect되는 영역 레벨 값 (Range Level = 1, 2 ,3)

    // 패턴별 공격 발동 트리거
    private const string _TPJUMPATTACK_ANIM_TRIGGER_NAME = "TpJumpAtk";         // Boss의 점프 공격 패턴 발동 트리거
    private const string _DEFAULTATK_ANIM_TRIGGER_NAME = "DefaultAtk";          // Boss의 1페이즈 기본 공격 동작의 발동 트리거
    private const string _RESPATK_ANIM_TRIGGER_NAME = "RespAtk";                // Boss의 1페이즈 반응형 공격 동작의 발동 트리거
    
    // 보스 상태 유발 트리거
    private const string _CRITICAL_ANIM_TRIGGER_NAME = "Critical";              // 반응형 공격 도중 Critical 상태 체크용 트리거
    private const string _STUNNED_ANIM_TRIGGER_NAME = "IsStun";

    // 단방향 트리거
    private const string _DIE_ANIM_TRIGGER_NAME = "Die";                        // Boss의 HP가 0이 될 시 그 즉시 die 트리거 활성화 (Hp = 0일 때 On)
    private const string _PHASE2_ANIM_TRIGGER_NAME = "2ndPhase";                // Boss의 2페이즈 판단. (HP <= 10% 일때 On)

    #endregion

    private void Awake()
    {
        currentHp = maxHp;
        anim = GetComponent<Animator>();
        runnerBT = new BehaviorTreeRunner(SettingBT());
        player = FindAnyObjectByType<PlayerMove>();
    }
    private void Start()
    {
        anim.SetBool(_FIRSTDETECT_ANIM_BOOL_NAME, true);
        anim.SetBool(_DETECT_ANIM_BOOL_NAME, false);
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
            
            // 만약 플레이어의 공격이 크리티컬이 동작하는 공격이고, boss가 Critical 상태일시 스턴
            if(anim.GetBool(_CRITICAL_ANIM_TRIGGER_NAME))
            {
                HitCritical(_collider);
            }
            HitJudge(_collider);
        }
        
    }

    #region Public Functions

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
            anim.SetTrigger(_DIE_ANIM_TRIGGER_NAME);
        }
    }

    public float GetDamage()
    {
        return atk;
    }

    #endregion

    #region Private Functions

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
                rangeLv = 1;
            }
            else if (distance <= level_Two_Range)
            {
                rangeLv = 2;
            }
            else if (distance <= level_Three_Range)
            {
                rangeLv = 3;
            }
            else
            {
                rangeLv = 0;
            }
        }
        else
        {
            detectedPlayerTr = null;
            anim.SetBool(_DETECT_ANIM_BOOL_NAME, false);
        }
    }

    private void LookAtPlayer()
    {
        if (detectedPlayerTr == null) return;

        Vector3 direction = (detectedPlayerTr.position - transform.position).normalized;
        direction.y = 0; // 수평 방향만 회전

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void HitJudge(Collider _collider)
    {
        if (!hitTargets.Contains(_collider))
        {
            PlayerBattle playerAttack = _collider.GetComponentInParent<PlayerBattle>();
            // 대미지 적용
            if (playerAttack != null && currentHp != 0)
            {
                float damage = playerAttack.GetDamage(); // 플레이어의 공격력 가져오기
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
    private void HitCritical(Collider _collider)
    {
        if (!hitTargets.Contains(_collider))
        {
            PlayerAnim criticalAttack = _collider.GetComponentInParent<PlayerAnim>();
            
            if (criticalAttack.critical)
            {
                anim.SetTrigger(_STUNNED_ANIM_TRIGGER_NAME);
            }
        }
    }

    private void CheckPlayerAttacking()
    {
        if (isrespAtkOnCooldown)
        {
            // 쿨타임 동안 아무런 작업도 하지 않음
            Debug.Log("RespAtk 트리거가 쿨타임 중입니다.");
            return; 
        }

        // 플레이어 공격 입력 탐지
        if (player.IsPlayerAttacking)
        {
            // 반응형 공격 애니메이션 트리거
            anim.SetTrigger(_RESPATK_ANIM_TRIGGER_NAME);
            anim.SetTrigger(_CRITICAL_ANIM_TRIGGER_NAME);
            Debug.Log("공격탐지");

            // 반응형 공격 내부 쿨타임 시작
            StartCoroutine(RespAtkCooldown()); 
        }
    }

    // Unity 작업 중 보스 인식 범위 Gizmos 확인
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, level_One_Range);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, level_Two_Range);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, level_Three_Range);
    }

    #endregion

    #region Coroutine Region

    private IEnumerator RespAtkCooldown()
    {
        isrespAtkOnCooldown = true; // 쿨타임 시작

        yield return new WaitForSeconds(respAtkCooldown);

        player.IsPlayerAttacking = false;
        anim.ResetTrigger(_RESPATK_ANIM_TRIGGER_NAME);
        isrespAtkOnCooldown = false; // 쿨타임 종료
        Debug.Log("RespAtk쿨종료");
    }

    private IEnumerator RemoveFromHitTrackerAfterCooldown(Collider _target)
    {
        bossHit.EnableKeyword("_EMISSION");

        PlayerAnim playerHit = _target.GetComponentInParent<PlayerAnim>();

        while(true)
        {
            if (playerHit.hitCombo == false) break;
            yield return null;
        }

        bossHit.DisableKeyword("_EMISSION");

        if (hitTargets.Contains(_target))
        {
            hitTargets.Remove(_target);
            Debug.Log("보스 내부 피격 판정 쿨다운 종료");
        }
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

    #endregion

    #region Animation-Event Essentials

    // 보스 무기 콜라이더 활성화
    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true; 
        }
    }

    // 보스 무기 콜라이더 비활성화
    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
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

    #endregion

    #region Animation-Event Custom

    public void FirstAttackTeleport()
    {
        StartCoroutine(FirstAttackTeleportCoroutine());
    }

    public void CriticalEnd()
    {
        anim.ResetTrigger(_CRITICAL_ANIM_TRIGGER_NAME);
    }

    public void DieCall()
    {
        Destroy(gameObject, 5f);
    }

    #endregion


    INode CreateAttackBehavior()
    {
        return new SelectorNode
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
                    );
    }

    INode SettingBT()
    {
        return new SelectorNode
        (
            new List<INode>()
            {
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(FirstDetectCheck),
                        new ActionNode(FirstDetectAttack),
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
            anim.SetTrigger(_TPJUMPATTACK_ANIM_TRIGGER_NAME);
            firstdetPos = detectedPlayerTr.position;
            anim.SetBool(_FIRSTDETECT_ANIM_BOOL_NAME,false);
            return INode.ENodeState.ENS_Success;
        }

        return INode.ENodeState.ENS_Failure;
    }
    
    INode.ENodeState NonDetect()
    {
        if(!anim.GetBool(_DETECT_ANIM_BOOL_NAME))
        {
            anim.SetInteger(_RANGE_ANIM_INT_NAME, 0);
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }

    #endregion

    #region Check Node
    INode.ENodeState CheckRangeLevelOne()
    {
        if (rangeLv == 1)
        {
            anim.SetInteger(_RANGE_ANIM_INT_NAME, 1);
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState CheckRangeLevelTwo()
    {
        if (rangeLv == 2)
        {
            anim.SetInteger(_RANGE_ANIM_INT_NAME, 2);
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState CheckRangeLevelThree()
    {
        if (rangeLv == 3)
        {
            anim.SetInteger(_RANGE_ANIM_INT_NAME, 3);
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState PressAttackCheck()
    {
        if(!isrespAtkOnCooldown)
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

    INode.ENodeState FirstDetectAttack()
    {
        // 현재 애니메이션 상태 정보를 가져옴
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // FirstDetectAttack 애니메이션이 실행 중인지 확인
        if (stateInfo.IsName(_FIRSTDETECT_ANIM_STATE_NAME))
        {
            if (stateInfo.normalizedTime < 1.0f)
            {
                // 애니메이션이 실행 중일 때 Running 반환
                return INode.ENodeState.ENS_Running;
            }
            else
            {
                // 애니메이션이 종료된 경우 Failure 반환
                return INode.ENodeState.ENS_Failure;
            }
        }
        // 애니메이션 상태가 맞지 않을 경우 Failure 반환
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState OnGuard()
    {
        if(tmpTime >= 10f)
        {
            tmpTime = 0;
            anim.SetTrigger(_DEFAULTATK_ANIM_TRIGGER_NAME);
            return INode.ENodeState.ENS_Success;
        }

        tmpTime += Time.deltaTime;

        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState DefaultMeleeAttackEnemy()
    {
        // 현재 애니메이션 상태 정보를 가져옴
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // FirstDetectAttack 애니메이션이 실행 중인지 확인
        if (stateInfo.IsName(_RESPATK_ANIM_STATE_NAME))
        {
            if (stateInfo.normalizedTime < 1.0f)
            {
                // 애니메이션이 실행 중일 때 Running 반환
                return INode.ENodeState.ENS_Running;
            }
            else
            {
                // 애니메이션이 종료된 경우 Failure 반환
                return INode.ENodeState.ENS_Failure;
            }
        }
        // 애니메이션 상태가 맞지 않을 경우 Failure 반환
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

}
