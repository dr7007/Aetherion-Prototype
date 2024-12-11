using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// �÷��̾� �ִϸ��̼� ���õ� ��ũ��Ʈ
public class PlayerAnim : MonoBehaviour
{
    public delegate void DieDelegate();
    public DieDelegate diedelegate;

    // �÷��̾� �ִϸ��̼� ���¸� Ȯ���ϱ� ���� Enum
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
    // �޺� Ȱ��ȭ�� �Ǹ� �ִϸ��̼ǿ��� �ش��Լ� ȣ��
    private void ComboOn(int _comboNum)
    {
        comboOn = true;
    }
    // �޺� ��Ȱ��ȭ�� �Ǹ� �ִϸ��̼ǿ��� �ش��Լ� ȣ��
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

    // �÷��̾� �׾����� ȣ��Ǵ� �Լ�
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
        // �̺�Ʈ�� �߻��ϸ� �߼Ҹ� ���� ���
        AudioSource.PlayClipAtPoint(footstep, followCam.transform.position, 0.5f);
    }

    private void SwordAttack()
    {
        // �̺�Ʈ�� �߻��ϸ� �߼Ҹ� ���� ���
        AudioSource.PlayClipAtPoint(swordAttack, followCam.transform.position, 0.5f);
    }

    private void AxeAttack()
    {
        // �̺�Ʈ�� �߻��ϸ� �߼Ҹ� ���� ���
        AudioSource.PlayClipAtPoint(axeAttack, followCam.transform.position, 0.5f);
    }
    #endregion


    #region PlayerInput
    // ���ݰ� �޺�����
    public void Attack(int _mouseNum)
    {
        curWeaponNum = pWeaponChange.curWeaponNum;
        // ������ attackTrigger Ȱ��ȭ (�̹� �������̶�� Ʈ���� �߻�X)
        if (!(CheckAnim(curWeaponNum) == EAnim.Attack))
        {
            anim.SetTrigger("AttackTrigger");
            // battleInfo.PlayerCurStamina -= attackStamina;
        }

        // �޺��� ������ comboTrigger Ȱ��ȭ, combo ��Ȱ��ȭ ���·� �ٲ�.
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
    // ������ ���ο� ���� move �ִϸ��̼�
    public void Move(bool _isMoving)
    {
        anim.SetBool("IsMoving", _isMoving);
    }
    // Shift �Է¿��ο� ���� �ִϸ��̼�
    public void Shift(bool _isShift)
    {
        anim.SetBool("IsShift", _isShift);
    }
    // ��Ʋ����϶� Shift ������ �ִϸ��̼� �ӵ��� �ٲ㼭 ������ �̵�
    public void BattleModeShift(bool _isShift)
    {
        anim.SetBool("IsShift", _isShift);

        // ��Ʋ��忡���� ���� �ȴ� �ӵ��� ����
        if (_isShift)
        {
            anim.SetFloat("WalkSpeed", battleModeShiftSpeed);
        }
        else
        {
            anim.SetFloat("WalkSpeed", 1f);
        }
    }
    // Space �Է½� ����Ǵ� Ʈ����
    public void Space()
    {
        anim.SetTrigger("SpaceTrigger");
    }
    // BattleMode���� Combo���϶� Space�ϸ� ����Ǵ� �Լ� (ĳ���� �������� ����)
    public void BattleModeAttackSpace(Vector3 _originInput)
    {
        // ���� ȸ��
        if (_originInput.x < -0.1f) anim.SetInteger("EvasionNum", (int)EAnim.LeftEvasion);
        // ������ ȸ��
        else if (_originInput.x > 0.1f) anim.SetInteger("EvasionNum", (int)EAnim.RightEvasion);
        // �� ȸ��
        else if (_originInput.z > 0.1f) anim.SetInteger("EvasionNum", (int)EAnim.FrontEvasion);
        // �� ȸ��
        else if (_originInput.z < -0.1f) anim.SetInteger("EvasionNum", (int)EAnim.BackEvasion);

        // �޺� 
        comboOn = false;
        combo2On = false;

    }

    // ȸ���� ȸ�� num�� �ʱ�ȭ
    public void ResetEvasionNum()
    {
        anim.SetInteger("EvasionNum", 0);
    }
    // ��Ʋ���� ��ȯ�Ǵ� �ִϸ��̼�
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

    // ���� � �ִϸ��̼��� ���������� Ȯ���ϴ� �Լ�
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
        // �װ��� 3�� �ִٰ�
        yield return new WaitForSeconds(3f);

        // 3�ʵ��� ����ٰ� ������°ͱ��� 1.5�� ���� -> 1.5�� �����

        CanvasGroup canvasGroup = DieImage.GetComponent<CanvasGroup>();

        // 1.5�� ���� ���� �� ���� (0 -> 1)
        float elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            canvasGroup.alpha = elapsedTime / 1.5f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1f);

        // 1.5�� ���� ���� �� ���� (1 -> 0)
        elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            canvasGroup.alpha = 1f - (elapsedTime / 1.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f; // ����

        // ���̵� �ƿ�
        elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            Color color = fadeOut.color;
            color.a = elapsedTime / 1.5f;
            fadeOut.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �ε� �� On
        loading.SetActive(true);

        // �ݹ� �Լ� ȣ��
        // �÷��̾� �ʱ�ȭ
        // ���� �ʱ�ȭ
        diedelegate?.Invoke();


        // ���̵� ��
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
