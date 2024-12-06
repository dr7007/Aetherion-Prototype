using UnityEngine;

// 플레이어 키입력과 관련된 스크립트
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Transform followCameraTr; // 따라오는 카메라 위치
    [SerializeField] private Transform orientation; // 몸의 중심에 있는 orientation (카메라 보는 각도로 돌아가있음.)
    [SerializeField] private float gravityPower = -9.81f; // 중력 설정
    [SerializeField] private float rotSpeed = 7f; // 플레이어 몸을 카메라 보는 방향으로 회전하는 속도
    [SerializeField] private float rollCooldown = 5f; // 구르기 쿨타임
    [SerializeField] private PlayerAnim pAnim; // animation 관리하는 PlayerAnim 스크립트
    [SerializeField] private float evasionTime = 1f;
    [SerializeField] private Material HDR;
    [SerializeField] private float playerStamina = 100f; 

    private CharacterController controller;
    private float startSpeed; // 처음 시작속도(걷는 속도) - 달리다가 돌아올때 필요
    private bool canRoll; // 구를수 있는 상태인지 나타냄.
    private bool IsBattleMode; // 싸움모드인지 아닌지 나타냄.
    private float axisH;
    private float axisV;

    private bool isPlayerAttacking = false;
    public bool IsPlayerAttacking
    {
        get { return isPlayerAttacking; }
        set { isPlayerAttacking = value; }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        canRoll = true;
        IsBattleMode = false;
    }


    private void Update()
    {
        if (pAnim.CheckAnim() == PlayerAnim.EAnim.Death) return;

        // 컨트롤러가 비활성화 상태일때 return
        if (controller == null) return;

        // 키입력 감지
        axisH = Input.GetAxis("Horizontal");
        axisV = Input.GetAxis("Vertical");

        // 입력받은 값
        Vector3 originInput = new Vector3(axisH, 0f, axisV);

        // 중력 적용
        Gravity();

        // 구르기 애니메이션 or 회피 진행중이라면 키입력 안받음.
        if (pAnim.CheckAnim() == PlayerAnim.EAnim.Roll || pAnim.CheckAnim() == PlayerAnim.EAnim.Evasion || pAnim.CheckAnim() == PlayerAnim.EAnim.HitReact) return;

        // 공격 애니메이션 중에는 마우스 입력과, space바만 가능
        if (pAnim.CheckAnim() == PlayerAnim.EAnim.Attack && IsBattleMode)
        {
            // Space 눌렀을때
            InputSpace(originInput);

            // 마우스 좌클릭
            InputAttack();
            return;
        }

        // 키 입력대로 3인칭 움직임
        ThirdPersonMove(axisV, axisH);

        // 공격입력
        InputAttack();

        // Shift 눌렀을때
        InputShift();

        // Space 눌렀을때
        InputSpace(originInput);

        // E(배틀모드키) 눌렀을때
        InputKeyE();
    }

    // 중력을 적용하는 함수
    private void Gravity()
    {
        Vector3 gravity = new Vector3(0f, gravityPower * Time.deltaTime, 0f);
        controller.Move(gravity);
    }

    // 구를수 있는 상태로 설정
    private void CanRoll()
    {
        canRoll = true;
    }

    // 회피때 바꾼 태그 정상화
    private void ResetTag()
    {
        // 회피 무적 끝
        // HDR.DisableKeyword("_EMISSION");

        gameObject.tag = "Player";
    }

    // 3인칭 시점의 플레이어 회전
    private void ThirdPersonMove(float _axisV, float _axisH)
    {
        // 카메라 바라보는 방향으로 방향설정
        Vector3 viewDir = transform.position - new Vector3(followCameraTr.position.x, transform.position.y, followCameraTr.position.z);
        orientation.forward = viewDir.normalized;

        // orientation 기준으로(카메라 바라보는 방향) 키입력 감지함.
        Vector3 camInput = orientation.forward * _axisV + orientation.right * _axisH;

        // 입력이 감지됬을떄, 플레이어의 방향을 카메라 보는 방향으로 바꿈
        if (camInput != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, camInput.normalized, Time.deltaTime * rotSpeed);

            // move 애니메이션 실행
            pAnim.Move(true);
        }
        else
        {
            // move 애니메이션 실행안함.
            pAnim.Move(false);
        }
    }

    #region PlayerInput
    // 공격 입력시(마우스 좌클릭)
    private void InputAttack()
    {
        // 배틀모드일때 마우스 좌클릭시
        if (Input.GetMouseButtonDown(0) && IsBattleMode)
        {
            // 공격 애니메이션 실행
            pAnim.Attack();
            isPlayerAttacking = true;
        }
    }
    // Shift -> 속도 변경 및 달리는 애니메이션
    private void InputShift()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 스태미나 감소


            // 배틀모드일때 shift
            if (IsBattleMode)
            {
                pAnim.BattleModeShift(true);
            }
            else
            {
                // 달리는 애니메이션
                pAnim.Shift(true);
            }
        }
        else
        {
            if (IsBattleMode)
            {
                pAnim.BattleModeShift(false);
            }
            else
            {
                // 달리는 애니메이션 off
                pAnim.Shift(false);
            }
        }
    }
    // Space -> 구르는 애니메이션 실행, 쿨타임 설정
    private void InputSpace(Vector3 _originInput)
    {
        if (Input.GetKeyDown(KeyCode.Space) && canRoll)
        {
            // 스태미나 감소

            // roll 할때 -> 공격 안받는 tag로 변경
            gameObject.tag = "Evasion";
            Invoke("ResetTag", evasionTime);

            // 임시용 코드 (회피 무적확인)
            // HDR.EnableKeyword("_EMISSION");

            // roll 쿨타임 on
            // canRoll = false;
            // Invoke("CanRoll", rollCooldown);

            // 구르는 애니메이션 실행
            pAnim.Space();

            if (IsBattleMode)
            {
                // battleMode일때 attack도중에 space시 anim에 필요한 정보(구르기 할 방향을 입력)를 넘김
                pAnim.BattleModeAttackSpace(_originInput);
            }
        }
    }
    // 전투모드 On/Off
    private void InputKeyE()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            IsBattleMode = !IsBattleMode;

            pAnim.BattleMode(IsBattleMode);
        }
    }
    #endregion



}
