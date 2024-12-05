using System.Collections;
using System.Linq;
using UnityEngine;
using static PlayerAnim;


// 플레이어 전투 판정 관련된 스크립트

// 추가할 기능
// 플레이어가 공격 입력 -> animation이 실행될꺼임 -> animation이벤트 호출 -> 해당 호출을 받아서 공격 콜라이더 키고 끄고..?
public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private float detectRange = 50f;
    [SerializeField] private BoxCollider attackCollider;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private Material HDR;

    public Collider[] colliders;
    public Vector3 mosterPosition;
    private PlayerAnim pAnim;
    private Animator anim;
    private Vector3 hitDir;
    private CharacterController controller;

    private float PlayerHp = 30f;

    [SerializeField] private AnimationCurve reactCurve;
    private float reactTimer;


    private void Awake()
    {
        mosterPosition = Vector3.zero;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        pAnim = GetComponent<PlayerAnim>();
        controller = GetComponent<CharacterController>();

        Keyframe reactCurve_lastFrame = reactCurve[reactCurve.length - 1];
        reactTimer = reactCurve_lastFrame.time;
    }

    private void Update()
    {
        // 몬스터의 위치 정보를 얻어오는 코드
        GetMonsterPosition();

        // 공격중일때 콜라이더를 키는 함수
        AttackOn();
    }

    // 몬스터의 위치 정보를 얻어오는 코드
    // 일정 범위를 탐지해서 가까이 있는 몬스터의 위치를 가져오는 코드로
    private void GetMonsterPosition()
    {
        colliders = Physics.OverlapSphere(transform.position, detectRange, monsterLayer);

        // 탐지된게 없으면 retrun
        if (colliders.Length == 0) return;

        // 가장 가까운 몬스터 탐지 (콜라이더를 거리순으로 정렬해서 첫번째꺼 가져옴)
        Collider closestMonster = colliders.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).First();

        mosterPosition = closestMonster.transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        // 만약 피격모션 중이라면 return
        if (CheckHitReact())
        {
            Debug.Log("Heat motion중");
            return;
        }

        // 무기에 맞았을때 리액션
        if (other.CompareTag("MonsterWeapon") && transform.gameObject.tag == "Player")
        {
            //플레이어 hp 하락
            PlayerHp--;
            Debug.Log("PlayerHp : " + PlayerHp);

            // 맞는 방향을 구하고
            hitDir = new Vector3(transform.position.x - mosterPosition.x, 0f, transform.position.z - mosterPosition.z).normalized;

            // 히트 리액션 보정을 위한 움직임 코루틴
            StartCoroutine("HitReactCoroutine", hitDir);

            // 방향을 플레이어 방향에 맞춰서 한번 바꾸고
            hitDir = transform.InverseTransformDirection(hitDir);

            // 애니메이션 실행시킴.
            anim.SetFloat("HitDirX", hitDir.x);
            anim.SetFloat("HitDirZ", hitDir.z);
            anim.CrossFade("HitReact", 0.05f);

        }
    }

    // 애니메이션 시간동안 캐릭터를 움직여줄 예정
    private IEnumerator HitReactCoroutine(Vector3 _hitDir)
    {
        float timer = 0f;
        anim.applyRootMotion = !anim.applyRootMotion;

        while (timer < reactTimer)
        {
            float speed = reactCurve.Evaluate(timer);
            controller.SimpleMove(speed * _hitDir * 10f);
            timer += Time.deltaTime;
            yield return null;
        }

        anim.applyRootMotion = !anim.applyRootMotion;
    }

    // 만약 공격중이라면 공격 콜라이더를 킴.
    private void AttackOn()
    {
        if (CheckAttackVisible())
        {
            attackCollider.enabled = true;
        }
        else
        {
            attackCollider.enabled = false;
        }
    }

    // 어택시 60퍼 정도만 콜라이더 켜둠
    private bool CheckAttackVisible()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if ((stateInfo.IsName("ComboAttack.Combo1") && stateInfo.normalizedTime < 0.5f) ||
        (stateInfo.IsName("ComboAttack.Combo2") && stateInfo.normalizedTime < 0.4f) ||
        (stateInfo.IsName("ComboAttack.Combo3") && stateInfo.normalizedTime < 0.40f))
        {
            HDR.EnableKeyword("_EMISSION");
            return true;
        }
        HDR.DisableKeyword("_EMISSION");
        return false;
    }

    private bool CheckHitReact()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("HitReact"))
        {
            return true;
        }
        return false;
    }

}
