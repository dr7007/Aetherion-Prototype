using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


// �÷��̾� ���� ���� ���õ� ��ũ��Ʈ
public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private float detectRange = 50f;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private float shieldTime;
    [SerializeField] private BoxCollider sword;
    [SerializeField] private BoxCollider axe;

    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // ����� �ߺ� ���� ������ ������

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

    // UI ������Ʈ�� ���� �ݹ� �̺�Ʈ
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
        // ������ ��ġ ������ ������ �ڵ�
        GetMonsterPosition();

        // �������϶� �ݶ��̴��� Ű�� �Լ�
        AttackOn();

        // ��� �׽�Ʈ�� ����Ű
        if (Input.GetKeyDown(KeyCode.P))
        {
            anim.SetTrigger(_DIE_ANUM_BOOL_NAME);
            anim.SetTrigger(_DIECHK_ANIM_BOOL_NAME);
        }

        // �׾����� UI�ʱ�ȭ �ϴ°�
        if (pAnim.dieplayerUi)
        {
            HpChangedCallback?.Invoke(PlayerCurHp, PlayerMaxHp);
            pAnim.dieplayerUi = false;
        }
    }

    // ������ ��ġ ������ ������ �ڵ�
    // ���� ������ Ž���ؼ� ������ �ִ� ������ ��ġ�� �������� �ڵ��
    private void GetMonsterPosition()
    {
        colliders = Physics.OverlapSphere(transform.position, detectRange, monsterLayer);

        // Ž���Ȱ� ������ retrun
        if (colliders.Length == 0) return;

        // ���� ����� ���� Ž�� (�ݶ��̴��� �Ÿ������� �����ؼ� ù��°�� ������)
        Collider closestMonster = colliders.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).First();

        mosterPosition = closestMonster.transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        curWeaponNum = weaponChange.CurWeaponNum;

        // �ǵ� �����𵿾� ���ؾȹ���.
        if (IsShieldHit) return;

        // ����߿� �¾�����
        if (other.CompareTag("MonsterWeapon") && gameObject.tag == "Blocking")
        {
            // �´� ������ �ݴ���� (�ٶ� ����)���ϰ�
            hitDir = new Vector3(-(transform.position.x - mosterPosition.x), 0f, -(transform.position.z - mosterPosition.z)).normalized;

            // �������� �ٶ󺸵���
            transform.forward = hitDir;

            // �ǵ忡�� �´� �ִϸ��̼� ����
            pAnim.ShieldHit();

            // ���ʰ� �������·� ����.
            StartCoroutine(ShieldReactTime());

            // ��Ÿ���� �ְ�ʹٸ� ������ �Ǳ� ��. �ڵ�(��Ÿ�� �ڷ�ƾ)�� ��������.

            return;
        }

        // ���� �ǰݸ�� ���̶�� return
        if ((CheckHitReact() || pAnim.CheckAnim(curWeaponNum) == PlayerAnim.EAnim.Death))
        {
            return;
        }

        // 2������ �������Ϳ��� �¾�����
        if (other.CompareTag("BossMonster") && transform.gameObject.tag == "Player")
        {
            TakeDamage(30);

            // �´� ������ ���ϰ�
            hitDir = new Vector3(transform.position.x - mosterPosition.x, 0f, transform.position.z - mosterPosition.z).normalized;

            // ��Ʈ ���׼� ������ ���� ������ �ڷ�ƾ
            StartCoroutine("HitReactCoroutine", hitDir);

            // ������ �÷��̾� ���⿡ ���缭 �ѹ� �ٲٰ�
            hitDir = transform.InverseTransformDirection(hitDir);

            // �ִϸ��̼� �����Ŵ.
            anim.SetFloat("HitDirX", hitDir.x);
            anim.SetFloat("HitDirZ", hitDir.z);
            anim.CrossFade("HitReact", 0f, curWeaponNum);

            // �¾Ҵٸ� ���⵵ ���¿� �°� �ʱ�ȭ
            if (curWeaponNum == 0)
            {
                weaponChange.ChangeSword();
            }
            else
            {
                weaponChange.ChangeAxe();
            }

            // ȸ�� ���µ� �ʱ�ȭ
            anim.SetInteger("EvasionNum", 0);

            // ���� ���� �ʱ�ȭ
            pAnim.hitCombo = false;
            pAnim.critical = false;
            pAnim.guardbreak = false;
            pAnim.healbane = false;
        }

        // ���⿡ �¾����� ���׼�
        if (other.CompareTag("MonsterWeapon") && transform.gameObject.tag == "Player")
        {
            BossMonsterAI bossAttack = other.GetComponentInParent<BossMonsterAI>();
            // ����� ����
            if (bossAttack != null && PlayerCurHp != 0)
            {
                float damage = bossAttack.GetDamage(); // ������ ���ݷ� ��������
                TakeDamage(damage); // �÷��̾�� ����� ����
            }

            // �´� ������ ���ϰ�
            hitDir = new Vector3(transform.position.x - mosterPosition.x, 0f, transform.position.z - mosterPosition.z).normalized;

            // ��Ʈ ���׼� ������ ���� ������ �ڷ�ƾ
            StartCoroutine("HitReactCoroutine", hitDir);

            // ������ �÷��̾� ���⿡ ���缭 �ѹ� �ٲٰ�
            hitDir = transform.InverseTransformDirection(hitDir);

            // �ִϸ��̼� �����Ŵ.
            anim.SetFloat("HitDirX", hitDir.x);
            anim.SetFloat("HitDirZ", hitDir.z);
            anim.CrossFade("HitReact", 0.05f, curWeaponNum);
            

            // �¾Ҵٸ� ���⵵ ���¿� �°� �ʱ�ȭ
            if (curWeaponNum == 0)
            {
                weaponChange.ChangeSword();
            }
            else
            {
                weaponChange.ChangeAxe();
            }

            // ȸ�� ���µ� �ʱ�ȭ
            anim.SetInteger("EvasionNum", 0);

            // ���� ���� �ʱ�ȭ
            pAnim.hitCombo = false;
            pAnim.critical = false;
            pAnim.guardbreak = false;
            pAnim.healbane = false;
        }
        
    }

    // �ִϸ��̼� �ð����� ĳ���͸� �������� ����
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

    // ���� �������̶�� ���� �ݶ��̴��� Ŵ.
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

    // �´� �ִϸ��̼� ������ Ȯ��
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


    // ������ �޴� �Լ�(�ݹ����� ���� ����)
    public void TakeDamage(float _damage)
    {

        PlayerCurHp -= _damage;
        PlayerCurHp = Mathf.Max(0, PlayerCurHp); // ü���� 0 ���Ϸ� �������� �ʵ��� ó��

        // UI ������Ʈ�� ���� �ݹ� ȣ��
        HpChangedCallback?.Invoke(PlayerCurHp, PlayerMaxHp);

        // �÷��̾� ��� ó��
        if (PlayerCurHp <= 0)
        {
            Debug.Log("���� ����");

            StartCoroutine(DieCoroutine());
        }
    }

    // ������ �������� �Լ�
    public float GetDamage()
    {
        return PlayerAtk;
    }

    // �ǵ� �� ���ʰ� ����
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
