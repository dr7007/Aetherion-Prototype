using Unity.VisualScripting;
using UnityEngine;

// 플레이어 전투 판정 관련된 스크립트

// 추가할 기능
// 플레이어가 공격 입력 -> animation이 실행될꺼임 -> animation이벤트 호출 -> 해당 호출을 받아서 공격 콜라이더 키고 끄고..?
public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private float detectRange = 50f;

    private Animator anim;
    private Vector3 mosterTr;
    private Vector3 hitDir;

    private void Awake()
    {
        mosterTr = Vector3.zero;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // 몬스터의 위치 정보를 얻어오는 코드
        GetMonsterPosition();
    }

    // 몬스터의 위치 정보를 얻어오는 코드
    // 일정 범위를 탐지해서 가까이 있는 몬스터의 위치를 가져오는 코드로
    private void GetMonsterPosition()
    {
        
    }

    // 맞았을때 발생하는 피격애니메이션
    private void OnTriggerEnter(Collider other)
    {
        // 맞는 방향을 구하고
        hitDir = new Vector3(transform.position.x - mosterTr.x, 0f, transform.position.z - mosterTr.z).normalized;

        // 방향을 플레이어 방향에 맞춰서 한번 바꾸고
        hitDir = transform.InverseTransformDirection(hitDir);

        // 애니메이션 실행시킴.
        anim.SetFloat("HitDirX", hitDir.x);
        anim.SetFloat("HitDirZ", hitDir.z);
        anim.CrossFade("HitReact", 0.3f);
    }


    // 공격 애니메이션 호출 받았을때 -> 공격 collider활성화
    public void AttackOn()
    {

    }

    public void AttackOff()
    {

    }
}
