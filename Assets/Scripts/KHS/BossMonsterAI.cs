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
    [SerializeField] private float rotationSpeed = 2f; // ȸ�� �ӵ�

    [Header("BossStats")]
    [SerializeField] private float maxHp = 30000f;
    [SerializeField] private float currentHp;
    [SerializeField] private float atk = 30f;

    [Header("Collider")]
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private float hitCooldown = 1.5f; // ��ٿ� �ð�

    private enum EPartDemege
    {
        CRIT = 4,
        NORMAL = 2,
        RESIST = 1
    }

    private PlayerMove player = null;

    private bool isAnimationLocked = false; // �ִϸ��̼� ���� ���� ���� ����
    private Quaternion lockedRotation; // ������ ȸ����
    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // �̹� ���� ��� ����
    private bool isPlayerAttackingOnCooldown = false; // ��Ÿ�� ���� ����
    private float playerAttackingCooldown = 10f;

    private Vector3 originPos = Vector3.zero;
    private Vector3 firstdetPos = Vector3.zero;
    private BehaviorTreeRunner runnerBT = null;
    private Transform detectedPlayerTr = null;
    private Animator anim = null;
    private float tmpTime = 0f;
    private float offset = 5f;

    public delegate void OnHpChangedDelegate(float currentHp, float maxHp);

    // UI ������Ʈ�� ���� �ݹ� �̺�Ʈ
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
            if (!hitTargets.Contains(_collider))
            {
                Player playerAttack = _collider.GetComponentInChildren<Player>();
                // ����� ����
                if (playerAttack != null)
                {
                    float damage = playerAttack.GetDamage(); // �÷��̾��� ���ݷ� ��������
                    Debug.Log("�÷��̾ ���� �����!" + damage);
                    TakeDamage(damage); // �������� ����� ����
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
    }
    private void ApplyDamageToPlayer(Collider _collider)
    {
        Debug.Log(_collider.gameObject.name + "�÷��̾�� ����� ����: " + atk);
    }
    private IEnumerator RemoveFromHitTrackerAfterCooldown(Collider _target)
    {
        yield return new WaitForSeconds(hitCooldown);

        if (hitTargets.Contains(_target))
        {
            hitTargets.Remove(_target);
            Debug.Log("���� ���� �ǰ� ���� ��ٿ� ����");
        }
    }

    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true; // �ݶ��̴� Ȱ��ȭ
        }
    }

    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
        }
    }
    public void EndAttack()
    {
        Debug.Log("EndAttack!");
        anim.ResetTrigger(_ATTACK_ANIM_TRIGGER_NAME);
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

    private void LookAtPlayer()
    {
        if (detectedPlayerTr == null) return;

        Vector3 direction = (detectedPlayerTr.position - transform.position).normalized;
        direction.y = 0; // ���� ���⸸ ȸ��

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void CheckPlayerAttacking()
    {
        if (isPlayerAttackingOnCooldown)
        {
            Debug.Log("PlayerAttacking Ʈ���Ű� ��Ÿ�� ���Դϴ�.");
            return; // ��Ÿ�� ���� ��� �ƹ� �۾��� ���� ����
        }

        // �÷��̾� ���� Ʈ���� Ȱ��ȭ
        if (player.IsPlayerAttacking)
        {
            anim.SetTrigger(_PLAYERATTACK_ANIM_TRIGGER_NAME); // ���� �ִϸ��̼� Ʈ����
            Debug.Log("PlayerAttacking Ʈ���� ����!");

            // ��Ÿ�� ����
            StartCoroutine(StartPlayerAttackingCooldown());
        }
    }
    private IEnumerator StartPlayerAttackingCooldown()
    {
        isPlayerAttackingOnCooldown = true; // ��Ÿ�� ����
        Debug.Log("PlayerAttacking ��Ÿ�� ����!");

        yield return new WaitForSeconds(playerAttackingCooldown);

        player.IsPlayerAttacking = false;
        anim.ResetTrigger(_PLAYERATTACK_ANIM_TRIGGER_NAME);
        isPlayerAttackingOnCooldown = false; // ��Ÿ�� ����
        Debug.Log("PlayerAttacking ��Ÿ�� ����! ���� �ٽ� Ȱ��ȭ�� �� �ֽ��ϴ�.");
    }
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
            Debug.Log("������ ���� ������ �����մϴ�.");
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
        currentHp = Mathf.Max(0, currentHp); // ü���� 0 ���Ϸ� �������� �ʵ��� ó��

        Debug.Log($"Boss took damage: {_damage}, Current HP: {currentHp}");

        // UI ������Ʈ�� ���� �ݹ� ȣ��
        HpChangedCallback?.Invoke(currentHp, maxHp);

        // ���� ��� ó��
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
