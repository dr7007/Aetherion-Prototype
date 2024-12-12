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

    private bool isAnimationLocked = false;                         // 애니메이션 도중 진행 방향 고정 판단.
    private Transform detectedPlayerTr = null;
    private Quaternion lockedRotation;                              // 애니메이션 시작 시 고정된 로테이션 값 버퍼
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
        if (_collider.CompareTag("pilar")) // 플레이어 무기에 의해 공격받은 경우
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
