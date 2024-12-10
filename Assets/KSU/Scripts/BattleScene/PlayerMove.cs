using UnityEngine;

// �÷��̾� Ű�Է°� ������ ���õ� ��ũ��Ʈ
public class PlayerMove : MonoBehaviour
{
    public delegate void OnStaminaChangedDelegate(float curStamina, float maxStamina);

    [SerializeField] private Transform followCameraTr; // ������� ī�޶� ��ġ
    [SerializeField] private Transform orientation; // ���� �߽ɿ� �ִ� orientation (ī�޶� ���� ������ ���ư�����.)
    [SerializeField] private float gravityPower = -9.81f; // �߷� ����
    [SerializeField] private float rotSpeed = 7f; // �÷��̾� ���� ī�޶� ���� �������� ȸ���ϴ� �ӵ�
    [SerializeField] private float rollCooldown = 5f; // ������ ��Ÿ��
    [SerializeField] private PlayerAnim pAnim; // animation �����ϴ� PlayerAnim ��ũ��Ʈ
    [SerializeField] private float evasionTime = 1f;

    public Material HDR;

    private int curWeaponNum;
    private PlayerBattle battleInfo;
    private CharacterController controller;
    private PlayerWeaponChange weaponChange;
    private float startSpeed; // ó�� ���ۼӵ�(�ȴ� �ӵ�) - �޸��ٰ� ���ƿö� �ʿ�
    private bool canRoll; // ������ �ִ� �������� ��Ÿ��.
    private bool IsBattleMode; // �ο������� �ƴ��� ��Ÿ��.
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
        battleInfo = GetComponent<PlayerBattle>();
        controller = GetComponent<CharacterController>();
        canRoll = true;
        IsBattleMode = false;
        weaponChange = GetComponent<PlayerWeaponChange>();
    }


    private void Update()
    {
        curWeaponNum = weaponChange.CurWeaponNum;

        // �׾����� �Է�X
        if (pAnim.CheckAnim(curWeaponNum) == PlayerAnim.EAnim.Death) return;

        // ��Ʈ�ѷ��� ��Ȱ��ȭ �����϶� return
        if (controller == null) return;

        // ȸ�� �ִϸ��̼� �����
        if(pAnim.evasion)
        {
            // ȸ�� On
            gameObject.tag = "Evasion";
            Invoke("ResetTag", evasionTime);
            // �ӽÿ� �ڵ� (ȸ�� ����Ȯ��)
            HDR.EnableKeyword("_EMISSION");
            pAnim.evasion = false; 
        }

        // ���Ű �Է�
        InputShield();

        // ������϶� �ٸ� Ű�Է�X
        if (gameObject.tag == "Blocking") return;

        // Ű�Է� ����
        axisH = Input.GetAxis("Horizontal");
        axisV = Input.GetAxis("Vertical");

        // �Է¹��� ��
        Vector3 originInput = new Vector3(axisH, 0f, axisV);

        // �߷� ����
        Gravity();

        // ������ �ִϸ��̼� or ȸ�� �������̶�� Ű�Է� �ȹ���.
        if (pAnim.CheckAnim(curWeaponNum) == PlayerAnim.EAnim.Roll || pAnim.CheckAnim(curWeaponNum) == PlayerAnim.EAnim.Evasion || pAnim.CheckAnim(curWeaponNum) == PlayerAnim.EAnim.HitReact) return;

        // ���� �ִϸ��̼� �߿��� ���콺 �Է°�, space�ٸ� ����
        if (pAnim.CheckAnim(curWeaponNum) == PlayerAnim.EAnim.Attack)
        {
            // Space ��������
            InputSpace(originInput);

            // ���콺 ��Ŭ��
            InputAttack();

            // ������ ȸ�� ����
            ThirdPersonAtMove(axisV, axisH);
            return;
        }

        // Ű �Է´�� 3��Ī ������
        ThirdPersonMove(axisV, axisH);

        // �����Է�
        InputAttack();

        // Shift ��������
        InputShift();

        // Space ��������
        InputSpace(originInput);

        // E(��Ʋ���Ű) ��������
        InputKeyE();
    }

    // �߷��� �����ϴ� �Լ�
    private void Gravity()
    {
        Vector3 gravity = new Vector3(0f, gravityPower * Time.deltaTime, 0f);
        controller.Move(gravity);
    }

    // ������ �ִ� ���·� ����
    private void CanRoll()
    {
        canRoll = true;
    }

    // ȸ�Ƕ� �ٲ� �±� ����ȭ
    private void ResetTag()
    {
        // ȸ�� ���� ��
        HDR.DisableKeyword("_EMISSION");

        gameObject.tag = "Player";
    }

    // 3��Ī ������ �÷��̾� ȸ��
    private void ThirdPersonMove(float _axisV, float _axisH)
    {
        // ī�޶� �ٶ󺸴� �������� ���⼳��
        Vector3 viewDir = transform.position - new Vector3(followCameraTr.position.x, transform.position.y, followCameraTr.position.z);
        orientation.forward = viewDir.normalized;

        // orientation ��������(ī�޶� �ٶ󺸴� ����) Ű�Է� ������.
        Vector3 camInput = orientation.forward * _axisV + orientation.right * _axisH;

        // �Է��� ����������, �÷��̾��� ������ ī�޶� ���� �������� �ٲ�
        if (camInput != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, camInput.normalized, Time.deltaTime * rotSpeed);

            // move �ִϸ��̼� ����
            pAnim.Move(true);
        }
        else
        {
            // move �ִϸ��̼� �������.
            pAnim.Move(false);
        }
    }

    // 3��Ī ������ ���ݽ� ȸ��
    private void ThirdPersonAtMove(float _axisV, float _axisH)
    {
        // ī�޶� �ٶ󺸴� �������� ���⼳��
        Vector3 viewDir = transform.position - new Vector3(followCameraTr.position.x, transform.position.y, followCameraTr.position.z);
        orientation.forward = viewDir.normalized;

        // orientation ��������(ī�޶� �ٶ󺸴� ����) Ű�Է� ������.
        Vector3 camInput = orientation.forward * _axisV + orientation.right * _axisH;


        if (camInput != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, camInput.normalized, Time.deltaTime * rotSpeed * 0.1f);
        }

    }


    #region PlayerInput
    // ���� �Է½�(���콺 ��Ŭ��)
    private void InputAttack()
    {
        // ��Ʋ����϶� ���콺 ��Ŭ���� or ���콺 ��Ŭ����
        if (Input.GetMouseButtonDown(0) && IsBattleMode)
        {
            // ���� �ִϸ��̼� ����
            pAnim.Attack(0);
            isPlayerAttacking = true;
        }
        else if (Input.GetMouseButtonDown(1) && IsBattleMode)
        {
            pAnim.Attack(1);
            isPlayerAttacking = true;
        }
    }
    // Shift -> �ӵ� ���� �� �޸��� �ִϸ��̼�
    private void InputShift()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // ��Ʋ����϶� shift
            if (IsBattleMode)
            {
                pAnim.BattleModeShift(true);
            }
            else
            {
                // �޸��� �ִϸ��̼�
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
                // �޸��� �ִϸ��̼� off
                pAnim.Shift(false);
            }
        }
    }
    // Space -> ������ �ִϸ��̼� ����, ��Ÿ�� ����
    private void InputSpace(Vector3 _originInput)
    {
        if (Input.GetKeyDown(KeyCode.Space) && canRoll)
        {
            // roll ��Ÿ�� on
            // canRoll = false;
            // Invoke("CanRoll", rollCooldown);

            // ������ �ִϸ��̼� ����
            pAnim.Space();

            if (IsBattleMode)
            {
                // battleMode�϶� attack���߿� space�� anim�� �ʿ��� ����(������ �� ������ �Է�)�� �ѱ�
                pAnim.BattleModeAttackSpace(_originInput);
            }
        }
    }
    // ������� On/Off
    private void InputKeyE()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            IsBattleMode = !IsBattleMode;

            pAnim.BattleMode(IsBattleMode);
        }
    }

    private void InputShield()
    {
        if (Input.GetMouseButtonDown(2))
        {
            pAnim.Shield(true);
        }

        if (Input.GetMouseButtonUp(2))
        {
            pAnim.Shield(false);
        }
    }
    #endregion

}