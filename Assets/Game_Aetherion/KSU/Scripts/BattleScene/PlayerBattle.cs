using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


// 플레이어 전투 판정 관련된 스크립트
public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private float detectRange = 50f;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private float shieldTime;
    [SerializeField] private BoxCollider sword;
    [SerializeField] private BoxCollider axe;

    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // 대미지 중복 적용 내부쿨 관리용

    private Collider[] colliders;
    private Vector3 mosterPosition;
    private PlayerAnim pAnim;
    private Animator anim;
    private Vector3 hitDir;
    private CharacterController controller;
    private PlayerWeaponChange weaponChange;
    private PlayerMove moveInfo;
    private BoxCollider attackCollider = null;

    public float PlayerAtk = 0f;
    public float PlayerMaxHp = 100f;
    public float PlayerCurHp;
    private float playerMaxStamina = 100f;
    private float playerCurStamina;
    private int curWeaponNum;
    private bool IsShieldHit = false;

    private const string _DIE_ANUM_BOOL_NAME = "Die";
    private const string _DIECHK_ANIM_BOOL_NAME = "DieChk";

    public delegate void OnHpChangedDelegate(float currentHp, float maxHp);

    // UI 업데이트를 위한 콜백 이벤트
    private OnHpChangedDelegate hpChangedCallback = null;
    public OnHpChangedDelegate HpChangedCallback
    {
        get { return hpChangedCallback; }
        set { hpChangedCallback = value; }
    }

    [SerializeField] private AnimationCurve reactCurve;
    private float reactTimer;

    public float PlayerCurStamina
    {
        get { return playerCurStamina; }
        set { playerCurStamina = value; }
    }


    private void Awake()
    {
        mosterPosition = Vector3.zero;
        playerCurStamina = playerMaxStamina;
    }

    private void Start()
    {
        PlayerCurHp = PlayerMaxHp;
        anim = GetComponent<Animator>();
        pAnim = GetComponent<PlayerAnim>();
        controller = GetComponent<CharacterController>();
        weaponChange = GetComponent<PlayerWeaponChange>();
        moveInfo = GetComponent<PlayerMove>();

        Keyframe reactCurve_lastFrame = reactCurve[reactCurve.length - 1];
        reactTimer = reactCurve_lastFrame.time;
    }

    private void Update()
    {
        // 몬스터의 위치 정보를 얻어오는 코드
        GetMonsterPosition();

        // 공격중일때 콜라이더를 키는 함수
        AttackOn();

        // 잠시 테스트용 죽음키
        if (Input.GetKeyDown(KeyCode.P))
        {
            anim.SetTrigger(_DIE_ANUM_BOOL_NAME);
            anim.SetTrigger(_DIECHK_ANIM_BOOL_NAME);
        }

        // 죽었을때 UI초기화 하는거
        if (pAnim.dieplayerUi)
        {
            HpChangedCallback?.Invoke(PlayerCurHp, PlayerMaxHp);
            pAnim.dieplayerUi = false;
        }
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
        curWeaponNum = weaponChange.CurWeaponNum;

        // 실드 무적쿨동안 피해안받음.
        if (IsShieldHit) return;

        // 방어중에 맞았을때
        if (other.CompareTag("MonsterWeapon") && gameObject.tag == "Blocking")
        {
            // 맞는 방향의 반대방향 (바라볼 방향)구하고
            hitDir = new Vector3(-(transform.position.x - mosterPosition.x), 0f, -(transform.position.z - mosterPosition.z)).normalized;

            // 그쪽으로 바라보도록
            transform.forward = hitDir;

            // 실드에서 맞는 애니메이션 실행
            pAnim.ShieldHit();

            // 몇초간 무적상태로 만듦.
            StartCoroutine(ShieldReactTime());

            // 쿨타임을 넣고싶다면 넣으면 되긴 함. 코드(쿨타임 코루틴)와 조건으로.

            return;
        }

        // 만약 피격모션 중이라면 return
        if ((CheckHitReact() || pAnim.CheckAnim(curWeaponNum) == PlayerAnim.EAnim.Death))
        {
            return;
        }

        // 2페이지 보스몬스터에게 맞았을떄
        if (other.CompareTag("BossMonster") && transform.gameObject.tag == "Player")
        {
            TakeDamage(30);

            // 맞는 방향을 구하고
            hitDir = new Vector3(transform.position.x - mosterPosition.x, 0f, transform.position.z - mosterPosition.z).normalized;

            // 히트 리액션 보정을 위한 움직임 코루틴
            StartCoroutine("HitReactCoroutine", hitDir);

            // 방향을 플레이어 방향에 맞춰서 한번 바꾸고
            hitDir = transform.InverseTransformDirection(hitDir);

            // 애니메이션 실행시킴.
            anim.SetFloat("HitDirX", hitDir.x);
            anim.SetFloat("HitDirZ", hitDir.z);
            anim.CrossFade("HitReact", 0f, curWeaponNum);

            // 맞았다면 무기도 상태에 맞게 초기화
            if (curWeaponNum == 0)
            {
                weaponChange.ChangeSword();
            }
            else
            {
                weaponChange.ChangeAxe();
            }

            // 회피 상태도 초기화
            anim.SetInteger("EvasionNum", 0);

            // 공격 상태 초기화
            pAnim.hitCombo = false;
            pAnim.critical = false;
            pAnim.guardbreak = false;
            pAnim.healbane = false;
        }

        // 무기에 맞았을때 리액션
        if (other.CompareTag("MonsterWeapon") && transform.gameObject.tag == "Player")
        {
            BossMonsterAI bossAttack = other.GetComponentInParent<BossMonsterAI>();
            // 대미지 적용
            if (bossAttack != null && PlayerCurHp != 0)
            {
                float damage = bossAttack.GetDamage(); // 보스의 공격력 가져오기
                TakeDamage(damage); // 플레이어에게 대미지 적용
            }

            // 맞는 방향을 구하고
            hitDir = new Vector3(transform.position.x - mosterPosition.x, 0f, transform.position.z - mosterPosition.z).normalized;

            // 히트 리액션 보정을 위한 움직임 코루틴
            StartCoroutine("HitReactCoroutine", hitDir);

            // 방향을 플레이어 방향에 맞춰서 한번 바꾸고
            hitDir = transform.InverseTransformDirection(hitDir);

            // 애니메이션 실행시킴.
            anim.SetFloat("HitDirX", hitDir.x);
            anim.SetFloat("HitDirZ", hitDir.z);
            anim.CrossFade("HitReact", 0.05f, curWeaponNum);
            

            // 맞았다면 무기도 상태에 맞게 초기화
            if (curWeaponNum == 0)
            {
                weaponChange.ChangeSword();
            }
            else
            {
                weaponChange.ChangeAxe();
            }

            // 회피 상태도 초기화
            anim.SetInteger("EvasionNum", 0);

            // 공격 상태 초기화
            pAnim.hitCombo = false;
            pAnim.critical = false;
            pAnim.guardbreak = false;
            pAnim.healbane = false;
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
        curWeaponNum = weaponChange.CurWeaponNum;
        if (curWeaponNum == 0)
        {
            attackCollider = sword;
        }
        else
        {
            attackCollider = axe;
        }

        if (pAnim.hitCombo)
        {
            attackCollider.enabled = true;
        }
        else
        {
            attackCollider.enabled = false;
        }
    }

    // 맞는 애니메이션 중인지 확인
    private bool CheckHitReact()
    {
        curWeaponNum = weaponChange.CurWeaponNum;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(curWeaponNum);

        if (stateInfo.IsName("HitReact"))
        {
            return true;
        }
        return false;
    }


    // 데미지 받는 함수(콜백으로 내용 보냄)
    public void TakeDamage(float _damage)
    {

        PlayerCurHp -= _damage;
        PlayerCurHp = Mathf.Max(0, PlayerCurHp); // 체력이 0 이하로 내려가지 않도록 처리

        // UI 업데이트를 위해 콜백 호출
        HpChangedCallback?.Invoke(PlayerCurHp, PlayerMaxHp);

        // 플레이어 사망 처리
        if (PlayerCurHp <= 0)
        {
            Debug.Log("정상 죽음");

            StartCoroutine(DieCoroutine());
        }
    }

    // 데미지 내보내는 함수
    public float GetDamage()
    {
        return PlayerAtk;
    }

    // 실드 후 몇초간 무적
    private IEnumerator ShieldReactTime()
    {
        IsShieldHit = true;
        moveInfo.HDR.EnableKeyword("_EMISSION");

        yield return new WaitForSeconds(shieldTime);

        IsShieldHit = false;
        moveInfo.HDR.DisableKeyword("_EMISSION");
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        anim.SetTrigger(_DIE_ANUM_BOOL_NAME);
        anim.SetTrigger(_DIECHK_ANIM_BOOL_NAME);
    }

}
