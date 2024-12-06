using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PlayerAnim;


// �÷��̾� ���� ���� ���õ� ��ũ��Ʈ

// �߰��� ���
// �÷��̾ ���� �Է� -> animation�� ����ɲ��� -> animation�̺�Ʈ ȣ�� -> �ش� ȣ���� �޾Ƽ� ���� �ݶ��̴� Ű�� ����..?
public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private float detectRange = 50f;
    [SerializeField] private BoxCollider attackCollider;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private Material HDR;

    private HashSet<Collider> hitTargets = new HashSet<Collider>(); // ����� �ߺ� ���� ������ ������

    public Collider[] colliders;
    public Vector3 mosterPosition;
    private PlayerAnim pAnim;
    private Animator anim;
    private Vector3 hitDir;
    private CharacterController controller;

    private float PlayerMaxHp = 100f;
    private float PlayerCurHp;

    private const string _DIE_ANUM_TRIGGER_NAME = "playerDie";

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


    private void Awake()
    {
        mosterPosition = Vector3.zero;
    }

    private void Start()
    {
        PlayerCurHp = PlayerMaxHp;
        anim = GetComponent<Animator>();
        pAnim = GetComponent<PlayerAnim>();
        controller = GetComponent<CharacterController>();

        Keyframe reactCurve_lastFrame = reactCurve[reactCurve.length - 1];
        reactTimer = reactCurve_lastFrame.time;
    }

    private void Update()
    {
        // ������ ��ġ ������ ������ �ڵ�
        GetMonsterPosition();

        // �������϶� �ݶ��̴��� Ű�� �Լ�
        AttackOn();
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
        // ���� �ǰݸ�� ���̶�� return
        if (CheckHitReact())
        {
            Debug.Log("Heat motion��");
            return;
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
            anim.CrossFade("HitReact", 0.05f);

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
        if (CheckAttackVisible())
        {
            attackCollider.enabled = true;
        }
        else
        {
            attackCollider.enabled = false;
        }
    }

    // ���ý� 60�� ������ �ݶ��̴� �ѵ�
    private bool CheckAttackVisible()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if ((stateInfo.IsName("ComboAttack.Combo1") && stateInfo.normalizedTime < 0.5f) ||
        (stateInfo.IsName("ComboAttack.Combo2") && stateInfo.normalizedTime < 0.4f) ||
        (stateInfo.IsName("ComboAttack.Combo3") && stateInfo.normalizedTime < 0.40f))
        {
            HDR.EnableKeyword("_EMISSION");
            return true;
        }
        HDR.DisableKeyword("_EMISSION");
        return false;
    }

    private bool CheckHitReact()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("HitReact"))
        {
            return true;
        }
        return false;
    }

    public void TakeDamage(float _damage)
    {

        PlayerCurHp -= _damage;
        PlayerCurHp = Mathf.Max(0, PlayerCurHp); // ü���� 0 ���Ϸ� �������� �ʵ��� ó��

        // UI ������Ʈ�� ���� �ݹ� ȣ��
        HpChangedCallback?.Invoke(PlayerCurHp, PlayerMaxHp);

        // �÷��̾� ��� ó��
        if (PlayerCurHp <= 0)
        {
            anim.SetTrigger(_DIE_ANUM_TRIGGER_NAME);
        }
    }
    public void DieCall()
    {
        Destroy(gameObject, 5f);
    }

}
