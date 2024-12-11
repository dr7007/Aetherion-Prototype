using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossMonsterAI : MonoBehaviour
{
    #region Inspector Controll Region

    public Material bossHit;                                        // Boss Hit�� ������ �ǵ���� ���� Material ȣ��.   (Inspector ����)

    [Header("Range")]
    [SerializeField] private float level_One_Range = 10f;           // 1��° �����ν� ���� (����� �ν� ����)
    [SerializeField] private float level_Two_Range = 25f;           // 2��° �����ν� ���� (�߰� �ν� ����)
    [SerializeField] private float level_Three_Range = 50f;         // 3��° �����ν� ���� (�� �ν� ����)

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;                  // ������ �ȴ� �ӵ�
    [SerializeField] private float runSpeed = 12f;                  // ������ �ٴ� �ӵ�
    [SerializeField] private float rotationSpeed = 2f;              // �÷��̾ LookAt�� ���� ȸ�� �ӵ�

    [Header("BossStats")]
    [SerializeField] private float maxHp = 30000f;                  // ������ �ִ� ü��
    [SerializeField] private float currentHp;                       // ������ ���� ü��
    [SerializeField] private float atk = 30f;                       // ������ ���ݷ�

    [Header("Collider")]
    [SerializeField] private Collider weaponCollider;               // ���� ������ �ݶ��̴� (���ݽ� On/Off�� ����)
    [SerializeField] private float hitCooldown = 0.2f;              // ���� ���� �� �ߺ� ������ �����ϱ� ���� ���� ��Ÿ��.

    #endregion

    #region Delegate Region
    
    public delegate void OnHpChangedDelegate(float currentHp, float maxHp);     // Boss HP ��ȭ���� UI�� �����ϱ� ���� Delegate

    #endregion

    #region Callback Region

    private OnHpChangedDelegate hpChangedCallback = null;                       // Boss HP ��ȭ���� UI�� �����ϱ� ���� Callback


    public OnHpChangedDelegate HpChangedCallback
    {
        get {  return hpChangedCallback; }
        set { hpChangedCallback = value; }
    }

    #endregion

    #region Buffer Region

    private BehaviorTreeRunner runnerBT = null;                     // Boss�� ���� Behavior Tree Runner ����
    private PlayerMove player = null;                               // �÷��̾��� ���� (��ġ, ���ݷ�, ���� �Է� ���� ��)�� �޾ƿ��� ���� ����
    private Quaternion lockedRotation;                              // �ִϸ��̼� ���� �� ������ �����̼� �� ����
    private Vector3 firstdetPos = Vector3.zero;                     // �÷��̾ FirstDetect�� ù ���� ��ǥ ����
    private Animator anim = null;                                   // Boss�� Animator ����
    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // ���� �ߺ� ���� ���� ó���� ���� hitTargets ���� ����
    private Transform detectedPlayerTr = null;                      // Boss�� Detect�����϶��� �ǽð� �÷��̾� Transfrom ���� ����. 

    #endregion

    #region CoolDown Region

    private float respAtkCooldown = 10f;                    // �÷��̾� ���� ���� �߰� ��Ÿ��.
    private float tmpTime = 0f;                                     // Boss�� Ư�� �ൿ ��Ÿ��.
    private float guardCooldown = 0f;
    private float jumpATKCooldown = 0f;
    private float breakbenefitCooldown = 15f;


    #endregion

    #region Judge Bool Region

    private bool isAnimationLocked = false;                         // �ִϸ��̼� ���� ���� ���� ���� �Ǵ�.
    private bool isrespAtkOnCooldown = false;                       // �÷��̾� ���� ���� �߰� ��Ÿ�� �Ǵ�.

    #endregion

    #region Functional values
    
    private const float GUARD_DEFAULT_INTERVAL = 10f;
    private const float JUMPATK_DEFAULT_INTERVAL = 3f;
    private const float JUDGE_TIME_INTERVAL = 3f;
    private const float NOREACT_ATTACK_INTERVAL = 5f;

    private float offset = 5f;                                      // Boss�� FirstDetect Attack�� ���� offset ��.
    private int rangeLv = 0;
    private float extraDamage = 0f;
    private float damageRedemtion = 0.0f;
    private float judgeTime = 0f;
    

    #endregion

    #region Animator Parameters

    // �ִϸ��̼� ���� �̸�
    private const string _FIRSTDETECT_ANIM_STATE_NAME = "FirstDetectAttack";
    private const string _STUNED_ANIM_STATE_NAME = "Stun";
    private const string _JUDGE_ANIM_STATE_NAME = "JudgeWalk";
    private const string _GUARD_ANIM_STATE_NAME = "Guard";
    private const string _GUARDBREAK_ANIM_STATE_NAME = "GuardBreak";

    // �ִϸ����� Bool Parameter
    private const string _FIRSTDETECT_ANIM_BOOL_NAME = "FirstDetect";           // Boss�� ���ۿ��� ������ ���� First Detect üũ Ʈ���� (���� ���� �� ù Detect �������� On, ù Detect ���� Off)
    private const string _DETECT_ANIM_BOOL_NAME = "Detect";                     // Boss�� �ǽð� Player Detect üũ Ʈ���� (���� �� Player�� ���� �� On, ������ �� Off)

    // �ִϸ����� Int Parameter
    private const string _RANGE_ANIM_INT_NAME = "RangeLevel";                   // Boss�� �������� Player�� Detect�Ǵ� ���� ���� �� (Range Level = 1, 2 ,3)

    // ���Ϻ� ���� �ߵ� Ʈ����
    private const string _JUDGE_ANIM_TRIGGER_NAME = "Judge";
    private const string _TPJUMPATTACK_ANIM_TRIGGER_NAME = "TpJumpAtk";         // Boss�� ���� ���� ���� �ߵ� Ʈ����
    private const string _DEFAULTATK_ANIM_TRIGGER_NAME = "DefaultAtk";          // Boss�� 1������ �⺻ ���� ������ �ߵ� Ʈ����
    private const string _RESPATK_ANIM_TRIGGER_NAME = "RespAtk";                // Boss�� 1������ ������ ���� ������ �ߵ� Ʈ����
    private const string _GUARD_ANIM_TRIGGER_NAME = "OnGuard";
    private const string _JUMPATK_ANIM_TRIGGER_NAME = "JumpAtk";

    // ���� ���� ���� Ʈ����
    private const string _CRITICAL_ANIM_TRIGGER_NAME = "Critical";              // ������ ���� ���� Critical ���� üũ�� Ʈ����
    private const string _STUNNED_ANIM_TRIGGER_NAME = "IsStun";
    private const string _GUARDBREAK_ANIM_TRIGGER_NAME = "isGBreak";

    // �ܹ��� Ʈ����
    private const string _DIE_ANIM_TRIGGER_NAME = "Die";                        // Boss�� HP�� 0�� �� �� �� ��� die Ʈ���� Ȱ��ȭ (Hp = 0�� �� On)
    private const string _PHASE2_ANIM_TRIGGER_NAME = "2ndPhase";                // Boss�� 2������ �Ǵ�. (HP <= 10% �϶� On)

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
            // �ִϸ��̼� ���߿��� ������ ���� ����
            transform.rotation = lockedRotation;
        }
        else if (detectedPlayerTr != null)
        {
            // �ִϸ��̼��� ���� ��� �÷��̾� ������ õõ�� ȸ��
            LookAtPlayer();
        }
        
    }

    private void FixedUpdate()
    {
        DetectPlayer();
    }

    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider.CompareTag("PlayerAttack")) // �÷��̾� ���⿡ ���� ���ݹ��� ���
        {
            
            // ���� �÷��̾��� ������ ũ��Ƽ���� �����ϴ� �����̰�, boss�� Critical �����Ͻ� ����
            if(anim.GetBool(_CRITICAL_ANIM_TRIGGER_NAME))
            {
                HitCritical(_collider);
            }
            if(anim.GetBool(_GUARD_ANIM_TRIGGER_NAME))
            {
                HitGBreak(_collider);
            }

            HitJudge(_collider);
        }
        
    }

    #region Public Functions

    public void TakeDamage(float _damage)
    {

        currentHp -= _damage;
        currentHp = Mathf.Max(0, currentHp); // ü���� 0 ���Ϸ� �������� �ʵ��� ó��

        Debug.Log($"Boss took damage: {_damage}, Current HP: {currentHp}");

        // UI ������Ʈ�� ���� �ݹ� ȣ��
        HpChangedCallback?.Invoke(currentHp, maxHp);

        // ���� ��� ó��
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

            // �÷��̾���� �Ÿ� Ȯ�� �� ���� �з�
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
        direction.y = 0; // ���� ���⸸ ȸ��

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void HitJudge(Collider _collider)
    {
        if (!hitTargets.Contains(_collider))
        {
            PlayerBattle playerAttack = _collider.GetComponentInParent<PlayerBattle>();
            // ����� ����
            if (playerAttack != null && currentHp != 0)
            {
                float damage = playerAttack.GetDamage() + extraDamage; // �÷��̾��� ���ݷ� ��������
                damage = damage * (1 - damageRedemtion);
                TakeDamage(damage); // �������� ����� ����
                //Debug.Log("���� �ߵ� Ȯ�ο�" + damage + " + " + extraDamage);
                Debug.Log("���� ������ Ȯ�ο� " + damageRedemtion);
            }

            // �浹�� ����� HitTracker�� �߰�
            hitTargets.Add(_collider);

            // ��ٿ� �� ��� ����
            StartCoroutine(RemoveFromHitTrackerAfterCooldown(_collider));
        }
        else
        {
            Debug.Log("�ǰ� ���� ��ħ���� ���� ���� �� ���� ��");
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
    private void HitGBreak(Collider _collider)
    {
        if (!hitTargets.Contains(_collider))
        {
            PlayerAnim GBreakAttack = _collider.GetComponentInParent<PlayerAnim>();

            if (GBreakAttack.critical)
            {
                Debug.Log("���� �극��ũ ����");
                anim.SetTrigger(_GUARDBREAK_ANIM_TRIGGER_NAME);
            }
        }
    }

    private void CheckPlayerAttacking()
    {
        if (isrespAtkOnCooldown)
        {
            // ��Ÿ�� ���� �ƹ��� �۾��� ���� ����
            Debug.Log("RespAtk Ʈ���Ű� ��Ÿ�� ���Դϴ�.");
            return; 
        }

        // �÷��̾� ���� �Է� Ž��
        if (player.IsPlayerAttacking)
        {
            // ������ ���� �ִϸ��̼� Ʈ����
            anim.SetTrigger(_RESPATK_ANIM_TRIGGER_NAME);
            Debug.Log("����Ž��");

            // ������ ���� ���� ��Ÿ�� ����
            StartCoroutine(RespAtkCooldown()); 
        }
    }

    // Unity �۾� �� ���� �ν� ���� Gizmos Ȯ��
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
        isrespAtkOnCooldown = true; // ��Ÿ�� ����

        yield return new WaitForSeconds(respAtkCooldown);

        player.IsPlayerAttacking = false;
        anim.ResetTrigger(_RESPATK_ANIM_TRIGGER_NAME);
        isrespAtkOnCooldown = false; // ��Ÿ�� ����
        Debug.Log("RespAtk������");
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
            Debug.Log("���� ���� �ǰ� ���� ��ٿ� ����");
        }
    }

    private IEnumerator FirstAttackTeleportCoroutine()
    {
        // �ִϸ��̼� �Ͻ� ����
        anim.speed = 0f;
        anim.applyRootMotion = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);

        // �÷��̾� ��ġ�� �̵�
        if (detectedPlayerTr != null)
        {
            transform.position = detectedPlayerTr.position - transform.forward * offset + Vector3.up * 3f; // �÷��̾� �ٷ� ���� �̵� ����
        }
        else
        {
            transform.position = firstdetPos - transform.forward * offset + Vector3.up * 3f; // �÷��̾ ó�� ������ ��ġ�� �̵� ����
        }
        
        anim.applyRootMotion = true;
        yield return new WaitForSeconds(0.5f); // Ư�� ���� �ð� (�ִϸ��̼� ȿ���� ����)
        LockDirection();
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        // �ִϸ��̼� �̾ ����
        anim.speed = 1f;
    }

    private IEnumerator GuardBreakChance()
    {
        damageRedemtion = -0.2f;
        Debug.Log("�����ı� �̵�ð� : " + damageRedemtion);
        yield return new WaitForSeconds(breakbenefitCooldown);
        damageRedemtion = 0.0f;
        Debug.Log("�����ı� �̵����� : " + damageRedemtion);
    }

    #endregion

    #region Animation-Event Essentials

    // ���� ���� �ݶ��̴� Ȱ��ȭ
    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true; 
        }
    }

    // ���� ���� �ݶ��̴� ��Ȱ��ȭ
    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    public void LockDirection()
    {
        // ���� ȸ������ �����ϰ� ���� ���� Ȱ��ȭ
        lockedRotation = transform.rotation;
        isAnimationLocked = true;
    }

    public void UnlockDirection()
    {
        // ���� ���� ����
        isAnimationLocked = false;
    }

    #endregion

    #region Animation-Event Custom

    public void FirstAttackTeleport()
    {
        StartCoroutine(FirstAttackTeleportCoroutine());
    }

    public void CriticalStart()
    {
        anim.SetTrigger(_CRITICAL_ANIM_TRIGGER_NAME);
    }
    public void CriticalEnd()
    {
        anim.ResetTrigger(_CRITICAL_ANIM_TRIGGER_NAME);
    }
    public void OnGuardStart()
    {
        damageRedemtion = 0.5f;
        Debug.Log("OGSTART : " + damageRedemtion);
        anim.SetTrigger(_GUARD_ANIM_TRIGGER_NAME);
    }
    public void OnGuardEnd()
    {
        Debug.Log("OGEND");
        anim.ResetTrigger(_GUARD_ANIM_TRIGGER_NAME);
    }
    public void GBbenefit()
    {
        Debug.Log("�̵� ��Ÿ�� ����");
        StartCoroutine(GuardBreakChance());
    }
    public void GBreakEnd()
    {
        Debug.Log("GBreakEnd");
        anim.ResetTrigger(_GUARD_ANIM_TRIGGER_NAME);
        anim.ResetTrigger(_GUARDBREAK_ANIM_TRIGGER_NAME);
    }

    public void StunEnd()
    {
        anim.ResetTrigger(_CRITICAL_ANIM_TRIGGER_NAME);
        anim.ResetTrigger(_STUNNED_ANIM_TRIGGER_NAME);
        extraDamage = 0f;
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
                                    new ActionNode(JudgeWalkCheck)
                                }
                            ),
                            new SequenceNode
                            (
                                new List<INode>()
                                {
                                    new ActionNode(CheckRangeLevelOne),
                                    new SequenceNode
                                    (
                                        new List<INode>()
                                        {
                                            new SelectorNode
                                            (
                                                new List<INode>()
                                                {
                                                    new ActionNode(PressAttackCheck),
                                                    new ActionNode(StunnedEnemy),
                                                }
                                            ),
                                            new ActionNode(NoReactAttack),
                                        }
                                    ),
                                }
                            ),
                            new SequenceNode
                            (
                                new List<INode>()
                                {
                                    new ActionNode(CheckRangeLevelTwo),
                                    new SequenceNode
                                    (
                                        new List<INode>()
                                        {
                                            new SelectorNode
                                            (
                                                new List<INode>()
                                                {
                                                    new ActionNode(OnGuardCheck),
                                                    new ActionNode(JumpAttackEnemy),
                                                }
                                            ),
                                        }
                                    ),
                                }
                            ),
                            new SequenceNode
                            (
                                new List<INode>()
                                {
                                    new ActionNode(CheckRangeLevelThree),
                                    new ActionNode(TPJAndHealing),
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

    INode.ENodeState JudgeWalkCheck()
    {
        // ���� �ִϸ����� �������� �޾ƿ���
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // JudgeWalk �ִϸ��̼��� ���� ������ Ȯ��
        if (stateInfo.IsName(_JUDGE_ANIM_STATE_NAME))
        {
            if (stateInfo.normalizedTime < 1.0f)
            {
                judgeTime += Time.deltaTime;
                if (rangeLv == 1)
                {
                    tmpTime += Time.deltaTime;
                }
                if (rangeLv == 2)
                {
                    guardCooldown += Time.deltaTime;
                    jumpATKCooldown += Time.deltaTime;
                }
                if(judgeTime >= JUDGE_TIME_INTERVAL)
                {
                    judgeTime = 0.0f;
                    anim.SetTrigger(_JUDGE_ANIM_TRIGGER_NAME);
                }
                // �ִϸ��̼��� ���� ���� �� Running ��ȯ
                return INode.ENodeState.ENS_Running;
            }
            else
            {
                // �ִϸ��̼��� ����� ��� Failure ��ȯ
                return INode.ENodeState.ENS_Failure;
            }
        }
        // �ִϸ��̼� ���°� ���� ���� ��� Failure ��ȯ
        return INode.ENodeState.ENS_Failure;
    }

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
        if (!isrespAtkOnCooldown)
        {
            //Debug.Log("������ ����üũ ������");
            CheckPlayerAttacking();
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState OnGuardCheck()
    {
        if (!anim.GetBool(_GUARD_ANIM_TRIGGER_NAME))
        {
            if (guardCooldown >= GUARD_DEFAULT_INTERVAL)
            {
                guardCooldown = 0.0f;
                Debug.Log("���� ����");
                anim.SetTrigger(_GUARD_ANIM_TRIGGER_NAME);

                return INode.ENodeState.ENS_Success;
            }
        }

        return INode.ENodeState.ENS_Failure;
    }

    #endregion

    #region Attack Node

    INode.ENodeState FirstDetectAttack()
    {
        // ���� �ִϸ����� �������� �޾ƿ���
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // FirstDetectAttack �ִϸ��̼��� ���� ������ Ȯ��
        if (stateInfo.IsName(_FIRSTDETECT_ANIM_STATE_NAME))
        {
            if (stateInfo.normalizedTime < 1.0f)
            {
                // �ִϸ��̼��� ���� ���� �� Running ��ȯ
                return INode.ENodeState.ENS_Running;
            }
            else
            {
                // �ִϸ��̼��� ����� ��� Failure ��ȯ
                return INode.ENodeState.ENS_Failure;
            }
        }
        // �ִϸ��̼� ���°� ���� ���� ��� Failure ��ȯ
        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState NoReactAttack()
    {
        if (!anim.GetBool(_RESPATK_ANIM_TRIGGER_NAME))
        {
            if (tmpTime >= NOREACT_ATTACK_INTERVAL)
            {
                tmpTime = 0;
                anim.SetTrigger(_DEFAULTATK_ANIM_TRIGGER_NAME);
                return INode.ENodeState.ENS_Success;
            }
        }

        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState StunnedEnemy()
    {
        // ���� �ִϸ����� �������� �޾ƿ���
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // Stun �ִϸ��̼��� ���� ������ Ȯ��
        if (stateInfo.IsName(_STUNED_ANIM_STATE_NAME))
        {
            if (stateInfo.normalizedTime < 1.0f)
            {
                extraDamage = 2000f;
                // �ִϸ��̼��� ���� ���� �� Running ��ȯ
                return INode.ENodeState.ENS_Running;
            }
            else
            {
                // �ִϸ��̼��� ����� ��� Failure ��ȯ
                return INode.ENodeState.ENS_Failure;
            }
        }
        // �ִϸ��̼� ���°� ���� ���� ��� Failure ��ȯ
        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState JumpAttackEnemy()
    {
        if (!anim.GetBool(_JUMPATK_ANIM_TRIGGER_NAME))
        {
            if (jumpATKCooldown >= JUMPATK_DEFAULT_INTERVAL)
            {
                Debug.Log("�������� ����");
                jumpATKCooldown = 0.0f;
                anim.SetTrigger(_JUMPATK_ANIM_TRIGGER_NAME);
                return INode.ENodeState.ENS_Success;
            }
        }

        return INode.ENodeState.ENS_Failure;
    }
    INode.ENodeState TPJAndHealing()
    {
        return INode.ENodeState.ENS_Failure;
    }

    #endregion

}
