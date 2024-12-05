using System.Linq;
using UnityEngine;


// �÷��̾� ���� ���� ���õ� ��ũ��Ʈ

// �߰��� ���
// �÷��̾ ���� �Է� -> animation�� ����ɲ��� -> animation�̺�Ʈ ȣ�� -> �ش� ȣ���� �޾Ƽ� ���� �ݶ��̴� Ű�� ����..?
public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private float detectRange = 50f;
    [SerializeField] private BoxCollider attackCollider;
    [SerializeField] private LayerMask monsterLayer;

    private Collider[] colliders;
    private Vector3 mosterPosition;

    private PlayerAnim pAnim;
    private Animator anim;
    private Vector3 hitDir;

    private void Awake()
    {
        mosterPosition = Vector3.zero;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        pAnim = GetComponent<PlayerAnim>();
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
        Debug.Log("Trigger �߻�");

        if (other.CompareTag("Monster"))
        {
            // �´� ������ ���ϰ�
            hitDir = new Vector3(transform.position.x - mosterPosition.x, 0f, transform.position.z - mosterPosition.z).normalized;

            // ������ �÷��̾� ���⿡ ���缭 �ѹ� �ٲٰ�
            hitDir = transform.InverseTransformDirection(hitDir);

            // �ִϸ��̼� �����Ŵ.
            anim.SetFloat("HitDirX", hitDir.x);
            anim.SetFloat("HitDirZ", hitDir.z);
            anim.CrossFade("HitReact", 0.3f);
        }
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

        if ((stateInfo.IsName("ComboAttack.Combo1") && stateInfo.normalizedTime < 0.6f) ||
        (stateInfo.IsName("ComboAttack.Combo2") && stateInfo.normalizedTime < 0.55f) ||
        (stateInfo.IsName("ComboAttack.Combo3") && stateInfo.normalizedTime < 0.55f))
        {
            return true;
        }
        return false;
    }

}
