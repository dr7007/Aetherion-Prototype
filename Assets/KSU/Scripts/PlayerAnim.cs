using Unity.Cinemachine;
using UnityEngine;

// �÷��̾� �ִϸ��̼� ���õ� ��ũ��Ʈ
public class PlayerAnim : MonoBehaviour
{
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
    // �޺� Ȱ��ȭ�� �Ǹ� �ִϸ��̼ǿ��� �ش��Լ� ȣ��
    private void ComboOn(int _comboNum)
    {
        comboOn[_comboNum] = true;
        comboCnt = _comboNum;
    }
    // �޺� ��Ȱ��ȭ�� �Ǹ� �ִϸ��̼ǿ��� �ش��Լ� ȣ��
    private void ComboOff(int _comboNum)
    {
        comboOn[_comboNum] = false;
    }

    // �÷��̾� �׾����� ȣ��Ǵ� �Լ�
    private void PlayerDIeCall()
    {
        followCam.SetActive(false);
        // DieImage.GetComponent<CanvasGroup>().alpha
    }
    #endregion


    #region PlayerInput
    // ���ݰ� �޺�����
    public void Attack()
    {
        // ������ attackTrigger Ȱ��ȭ (�̹� �������̶�� Ʈ���� �߻�X)
        if (!(CheckAnim() == EAnim.Attack))
        {
            anim.SetTrigger("AttackTrigger");
        }

        // �޺��� ������ comboTrigger Ȱ��ȭ, combo ��Ȱ��ȭ ���·� �ٲ�.
        if (comboOn[comboCnt])
        {
            anim.SetTrigger("ComboTrigger");
            comboOn[comboCnt] = false;
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
        if (comboOn[comboCnt])
        {
            Debug.Log(_originInput);
            // ���� ȸ��
            if (_originInput.x < -0.01f) anim.SetInteger("EvasionNum", (int)EAnim.LeftEvasion);
            // ������ ȸ��
            if (_originInput.x > 0.01f) anim.SetInteger("EvasionNum", (int)EAnim.RightEvasion);
            // �� ȸ��
            if (_originInput.z > 0.01f) anim.SetInteger("EvasionNum", (int)EAnim.FrontEvasion);
            // �� ȸ��
            if (_originInput.z < -0.01f) anim.SetInteger("EvasionNum", (int)EAnim.BackEvasion);

            // �޺� 
            comboOn[comboCnt] = false;
        }
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
    #endregion

    // ���� � �ִϸ��̼��� ���������� Ȯ���ϴ� �Լ�
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
