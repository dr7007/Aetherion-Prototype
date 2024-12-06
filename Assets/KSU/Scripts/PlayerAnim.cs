using Unity.Cinemachine;
using UnityEngine;

// 플레이어 애니메이션 관련된 스크립트
public class PlayerAnim : MonoBehaviour
{
    // 플레이어 애니메이션 상태를 확인하기 위한 Enum
    public enum EAnim
    {
        Nothing = 0,
        Roll = 1,
        Attack = 2,
        LeftEvasion = 3,
        RightEvasion = 4,
        FrontEvasion = 5,
        BackEvasion = 6,
        Evasion = 7,
        AttackVisible = 8,
        HitReact = 9,
        Death = 10
    };

    [SerializeField] private float battleModeShiftSpeed = 1.5f;
    [SerializeField] private GameObject followCam;
    [SerializeField] private GameObject DieImage;

    private Animator anim;
    private bool[] comboOn;
    private int comboCnt;

    private void Awake()
    {
        comboOn = new bool[4];
        comboOn[0] = false;
        comboCnt = 0;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    #region AnimEvent
    // 콤보 활성화가 되면 애니메이션에서 해당함수 호출
    private void ComboOn(int _comboNum)
    {
        comboOn[_comboNum] = true;
        comboCnt = _comboNum;
    }
    // 콤보 비활성화가 되면 애니메이션에서 해당함수 호출
    private void ComboOff(int _comboNum)
    {
        comboOn[_comboNum] = false;
    }

    // 플레이어 죽었을때 호출되는 함수
    private void PlayerDIeCall()
    {
        followCam.SetActive(false);
        // DieImage.GetComponent<CanvasGroup>().alpha
    }
    #endregion


    #region PlayerInput
    // 공격과 콤보어택
    public void Attack()
    {
        // 때리면 attackTrigger 활성화 (이미 공격중이라면 트리거 발생X)
        if (!(CheckAnim() == EAnim.Attack))
        {
            anim.SetTrigger("AttackTrigger");
        }

        // 콤보때 때리면 comboTrigger 활성화, combo 비활성화 상태로 바꿈.
        if (comboOn[comboCnt])
        {
            anim.SetTrigger("ComboTrigger");
            comboOn[comboCnt] = false;
        }
    }
    // 움직임 여부에 따른 move 애니메이션
    public void Move(bool _isMoving)
    {
        anim.SetBool("IsMoving", _isMoving);
    }
    // Shift 입력여부에 따른 애니메이션
    public void Shift(bool _isShift)
    {
        anim.SetBool("IsShift", _isShift);
    }
    // 배틀모드일때 Shift 누르면 애니메이션 속도를 바꿔서 빠르게 이동
    public void BattleModeShift(bool _isShift)
    {
        anim.SetBool("IsShift", _isShift);

        // 배틀모드에서는 따로 걷는 속도를 증가
        if (_isShift)
        {
            anim.SetFloat("WalkSpeed", battleModeShiftSpeed);
        }
        else
        {
            anim.SetFloat("WalkSpeed", 1f);
        }
    }
    // Space 입력시 실행되는 트리거
    public void Space()
    {
        anim.SetTrigger("SpaceTrigger");
    }
    // BattleMode에서 Combo중일때 Space하면 실행되는 함수 (캐릭터 기준으로 점프)
    public void BattleModeAttackSpace(Vector3 _originInput)
    {
        if (comboOn[comboCnt])
        {
            Debug.Log(_originInput);
            // 왼쪽 회피
            if (_originInput.x < -0.01f) anim.SetInteger("EvasionNum", (int)EAnim.LeftEvasion);
            // 오른쪽 회피
            if (_originInput.x > 0.01f) anim.SetInteger("EvasionNum", (int)EAnim.RightEvasion);
            // 앞 회피
            if (_originInput.z > 0.01f) anim.SetInteger("EvasionNum", (int)EAnim.FrontEvasion);
            // 뒤 회피
            if (_originInput.z < -0.01f) anim.SetInteger("EvasionNum", (int)EAnim.BackEvasion);

            // 콤보 
            comboOn[comboCnt] = false;
        }
    }
    // 회피후 회피 num값 초기화
    public void ResetEvasionNum()
    {
        anim.SetInteger("EvasionNum", 0);
    }
    // 배틀모드로 전환되는 애니메이션
    public void BattleMode(bool _IsBattleMode)
    {
        anim.SetBool("IsBattleMode", _IsBattleMode);
    }
    #endregion

    // 현재 어떤 애니메이션이 진행중인지 확인하는 함수
    public EAnim CheckAnim()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Roll"))
        {
            return EAnim.Roll;
        }

        if (stateInfo.IsName("ComboAttack.Combo1") || stateInfo.IsName("ComboAttack.Combo2") || stateInfo.IsName("ComboAttack.Combo3"))
        {
            return EAnim.Attack;
        }

        if (stateInfo.IsName("EvasionBack") || stateInfo.IsName("EvasionForward") || stateInfo.IsName("EvasionRight") || stateInfo.IsName("EvasionLeft"))
        {
            return EAnim.Evasion;
        }

        if (stateInfo.IsName("HitReact"))
        {
            return EAnim.HitReact;
        }

        if (stateInfo.IsName("PlayerDead"))
        {
            return EAnim.Death;
        }

        return EAnim.Nothing;
    }
}
