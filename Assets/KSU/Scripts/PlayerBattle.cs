using System.Linq;
using UnityEngine;


// 플레이어 전투 판정 관련된 스크립트

// 추가할 기능
// 플레이어가 공격 입력 -> animation이 실행될꺼임 -> animation이벤트 호출 -> 해당 호출을 받아서 공격 콜라이더 키고 끄고..?
public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private float detectRange = 50f;
    [SerializeField] private BoxCollider attackCollider;
    [SerializeField] private LayerMask monsterLayer;

    private Collider[] colliders;
    private Vector3 mosterPosition;

    private PlayerAnim pAnim;
    private Animator anim;
    private Vector3 hitDir;

    private void Awake()
    {
        mosterPosition = Vector3.zero;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        pAnim = GetComponent<PlayerAnim>();
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
        Debug.Log("Trigger 발생");

        if (other.CompareTag("Monster"))
        {
            // 맞는 방향을 구하고
            hitDir = new Vector3(transform.position.x - mosterPosition.x, 0f, transform.position.z - mosterPosition.z).normalized;

            // 방향을 플레이어 방향에 맞춰서 한번 바꾸고
            hitDir = transform.InverseTransformDirection(hitDir);

            // 애니메이션 실행시킴.
            anim.SetFloat("HitDirX", hitDir.x);
            anim.SetFloat("HitDirZ", hitDir.z);
            anim.CrossFade("HitReact", 0.3f);
        }
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

        if ((stateInfo.IsName("ComboAttack.Combo1") && stateInfo.normalizedTime < 0.6f) ||
        (stateInfo.IsName("ComboAttack.Combo2") && stateInfo.normalizedTime < 0.55f) ||
        (stateInfo.IsName("ComboAttack.Combo3") && stateInfo.normalizedTime < 0.55f))
        {
            return true;
        }
        return false;
    }

}
