using UnityEngine;


public class Boss2PhaseAI : MonoBehaviour
{
    public delegate void boss2Delegate();
    public boss2Delegate bossDieCallback;

    [Header("Condition")]
    [SerializeField] private float NeedClear = 5f;
    [SerializeField] private float CurCount = 0f;
    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float level_Three_Range = 40f;

    private bool isAnimationLocked = false;                         // �ִϸ��̼� ���� ���� ���� ���� �Ǵ�.
    private Transform detectedPlayerTr = null;
    private Quaternion lockedRotation;                              // �ִϸ��̼� ���� �� ������ �����̼� �� ����
    private Animator anim = null;
    private float tmpTime = 0.0f;

    private const string _ONFIELD_ANIM_TRIGGER_NAME = "OnField";
    private const string _JUDGE_ANIM_TRIGGER_NAME = "Judge";
    private const string _DEAD_ANIM_TRIGGER_NAME = "Die";
    private const string _RUSH_ANIM_TRIGGER_NAME = "Rush";
    private const string _STRIKE_ANIM_TRIGGER_NAME = "Strike";

    private void Start()
    {
        anim = GetComponent<Animator>();
        CurCount = NeedClear;
        anim.ResetTrigger(_ONFIELD_ANIM_TRIGGER_NAME);
        anim.ResetTrigger(_JUDGE_ANIM_TRIGGER_NAME);
        anim.ResetTrigger(_DEAD_ANIM_TRIGGER_NAME);
    }
    private void Update()
    {
        JudgeOn();
        
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
    private void DetectPlayer()
    {
        Collider[] overlapColliders = Physics.OverlapSphere(transform.position, level_Three_Range, LayerMask.GetMask("Player"));

        if (overlapColliders.Length > 0)
        {
            detectedPlayerTr = overlapColliders[0].transform;
        }
        else
        {
            detectedPlayerTr = null;
        }
    }


    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider.CompareTag("pilar")) // �÷��̾� ���⿡ ���� ���ݹ��� ���
        {
            PilerBreak();
            anim.SetTrigger(_STRIKE_ANIM_TRIGGER_NAME);
        }
        else if(_collider.CompareTag("Pilar"))
        {
            anim.SetTrigger(_STRIKE_ANIM_TRIGGER_NAME);
        }
        else if (_collider.CompareTag("Player"))
        {
            anim.SetTrigger(_STRIKE_ANIM_TRIGGER_NAME);
        }

    }

    public void DieCall()
    {
        bossDieCallback?.Invoke();
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

    private void PilerBreak()
    {
        --CurCount;
        if(CurCount <= 0)
        {
            anim.SetTrigger(_DEAD_ANIM_TRIGGER_NAME);
        }
    }

    private void JudgeOn()
    {
        tmpTime += Time.deltaTime;
        if(tmpTime >= 7f)
        {
            tmpTime = 0.0f;
            anim.SetTrigger(_JUDGE_ANIM_TRIGGER_NAME);
        }
    }
}
