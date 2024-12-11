using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// 플레이어 애니메이션 관련된 스크립트
public class PlayerAnim : MonoBehaviour
{
    public delegate void DieDelegate();
    public DieDelegate diedelegate;

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
    [SerializeField] private Image fadeOut;
    [SerializeField] private GameObject loading;
    [SerializeField] private AudioClip footstep;
    [SerializeField] private AudioClip swordAttack;
    [SerializeField] private AudioClip axeAttack;

    private PlayerMove moveInfo;
    private PlayerBattle battleInfo;
    private Animator anim;
    public bool comboOn;
    public bool combo2On;
    public bool evasion = false;
    public bool blocking = false;
    public bool hitCombo = false;
    public bool critical = false;
    public bool guardbreak = false;
    public bool healbane = false;
    private PlayerWeaponChange pWeaponChange;

    private float attackStamina = 10f;
    private int curWeaponNum;

    private void Awake()
    {
        comboOn = false;
        combo2On = false;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        battleInfo = GetComponent<PlayerBattle>();
        moveInfo = GetComponent<PlayerMove>();
        pWeaponChange = GetComponent<PlayerWeaponChange>();
    }

    #region AnimEvent
    // 콤보 활성화가 되면 애니메이션에서 해당함수 호출
    private void ComboOn(int _comboNum)
    {
        comboOn = true;
    }
    // 콤보 비활성화가 되면 애니메이션에서 해당함수 호출
    private void ComboOff(int _comboNum)
    {
        comboOn = false;
    }

    private void Combo2On()
    {
        combo2On = true;
    }

    private void Combo2Off()
    {
        combo2On = false;
    }

    // 플레이어 죽었을때 호출되는 함수
    private void PlayerDIeCall()
    {
        followCam.SetActive(false);

        StartCoroutine(ChangeDieMessageCoroutine());
    }

    private void EvasionOn()
    {
        evasion = true;
    }
    private void HitOn()
    {
        hitCombo = true;
    }

    private void HitOff()
    {
        hitCombo = false;
    }

    private void CriticalOn()
    {
        critical = true;
    }

    private void CriticalOff()
    {
        critical = false;
    }

    private void GuardBreakOn()
    {
        guardbreak = true;
    }

    private void GuardBreakOff()
    {
        guardbreak = false;
    }

    private void HealBaneOn()
    {
        healbane = true;
    }

    private void HealBaneOff()
    {
        healbane = false;
    }

    private void FootStep()
    {
        // 이벤트가 발생하면 발소리 사운드 재생
        AudioSource.PlayClipAtPoint(footstep, followCam.transform.position, 0.5f);
    }

    private void SwordAttack()
    {
        // 이벤트가 발생하면 발소리 사운드 재생
        AudioSource.PlayClipAtPoint(swordAttack, followCam.transform.position, 0.5f);
    }

    private void AxeAttack()
    {
        // 이벤트가 발생하면 발소리 사운드 재생
        AudioSource.PlayClipAtPoint(axeAttack, followCam.transform.position, 0.5f);
    }
    #endregion


    #region PlayerInput
    // 공격과 콤보어택
    public void Attack(int _mouseNum)
    {
        curWeaponNum = pWeaponChange.curWeaponNum;
        // 때리면 attackTrigger 활성화 (이미 공격중이라면 트리거 발생X)
        if (!(CheckAnim(curWeaponNum) == EAnim.Attack))
        {
            anim.SetTrigger("AttackTrigger");
            // battleInfo.PlayerCurStamina -= attackStamina;
        }

        // 콤보때 때리면 comboTrigger 활성화, combo 비활성화 상태로 바꿈.
        if (comboOn && _mouseNum == 0)
        {
            anim.SetTrigger("ComboTrigger");
            // comboOn = false;
            // battleInfo.PlayerCurStamina -= attackStamina;
        }

        if (combo2On && _mouseNum == 1)
        {
            anim.SetTrigger("Combo2Trigger");
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
        // 왼쪽 회피
        if (_originInput.x < -0.1f) anim.SetInteger("EvasionNum", (int)EAnim.LeftEvasion);
        // 오른쪽 회피
        else if (_originInput.x > 0.1f) anim.SetInteger("EvasionNum", (int)EAnim.RightEvasion);
        // 앞 회피
        else if (_originInput.z > 0.1f) anim.SetInteger("EvasionNum", (int)EAnim.FrontEvasion);
        // 뒤 회피
        else if (_originInput.z < -0.1f) anim.SetInteger("EvasionNum", (int)EAnim.BackEvasion);

        // 콤보 
        comboOn = false;
        combo2On = false;

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

    public void Shield(bool _state)
    {
        if (_state)
        {
            blocking = true;
            gameObject.tag = "Blocking";
        }
        else
        {
            blocking = false;
            gameObject.tag = "Player";
        }
        anim.SetBool("IsBlocking", blocking);
    }

    public void ShieldHit()
    {
        anim.SetTrigger("ShieldHit");
        blocking = false;
        gameObject.tag = "Player";  
        anim.SetBool("IsBlocking", blocking);
    }
    #endregion

    // 현재 어떤 애니메이션이 진행중인지 확인하는 함수
    public EAnim CheckAnim(int _num)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(_num);

        if (stateInfo.IsName("Roll"))
        {
            return EAnim.Roll;
        }

        if (stateInfo.IsName("Combo1") || stateInfo.IsName("Combo2") || stateInfo.IsName("Combo3") || stateInfo.IsName("ComboB1") || stateInfo.IsName("ComboB2") || stateInfo.IsName("ComboB3") || stateInfo.IsName("ComboC1") || stateInfo.IsName("ComboC2"))
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

        if (stateInfo.IsName("Die"))
        {
            return EAnim.Death;
        }

        return EAnim.Nothing;
    }


    private IEnumerator ChangeDieMessageCoroutine()
    {
        // 죽고나서 3초 있다가
        yield return new WaitForSeconds(3f);

        // 3초동안 생겼다가 사라지는것까지 1.5초 생성 -> 1.5초 사라짐

        CanvasGroup canvasGroup = DieImage.GetComponent<CanvasGroup>();

        // 1.5초 동안 알파 값 증가 (0 -> 1)
        float elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            canvasGroup.alpha = elapsedTime / 1.5f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1f);

        // 1.5초 동안 알파 값 감소 (1 -> 0)
        elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            canvasGroup.alpha = 1f - (elapsedTime / 1.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f; // 보정

        // 페이드 아웃
        elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            Color color = fadeOut.color;
            color.a = elapsedTime / 1.5f;
            fadeOut.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 로딩 씬 On
        loading.SetActive(true);

        // 콜백 함수 호출
        // 플레이어 초기화
        // 보스 초기화
        diedelegate?.Invoke();


        // 페이드 인
        elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            Color color = fadeOut.color;
            color.a = 1f - (elapsedTime / 1.5f);
            fadeOut.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        loading.SetActive(false);
    }

}
