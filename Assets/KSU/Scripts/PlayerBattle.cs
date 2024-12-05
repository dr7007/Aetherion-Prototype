using Unity.VisualScripting;
using UnityEngine;

// �÷��̾� ���� ���� ���õ� ��ũ��Ʈ

// �߰��� ���
// �÷��̾ ���� �Է� -> animation�� ����ɲ��� -> animation�̺�Ʈ ȣ�� -> �ش� ȣ���� �޾Ƽ� ���� �ݶ��̴� Ű�� ����..?
public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private float detectRange = 50f;

    private Animator anim;
    private Vector3 mosterTr;
    private Vector3 hitDir;

    private void Awake()
    {
        mosterTr = Vector3.zero;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // ������ ��ġ ������ ������ �ڵ�
        GetMonsterPosition();
    }

    // ������ ��ġ ������ ������ �ڵ�
    // ���� ������ Ž���ؼ� ������ �ִ� ������ ��ġ�� �������� �ڵ��
    private void GetMonsterPosition()
    {
        
    }

    // �¾����� �߻��ϴ� �ǰݾִϸ��̼�
    private void OnTriggerEnter(Collider other)
    {
        // �´� ������ ���ϰ�
        hitDir = new Vector3(transform.position.x - mosterTr.x, 0f, transform.position.z - mosterTr.z).normalized;

        // ������ �÷��̾� ���⿡ ���缭 �ѹ� �ٲٰ�
        hitDir = transform.InverseTransformDirection(hitDir);

        // �ִϸ��̼� �����Ŵ.
        anim.SetFloat("HitDirX", hitDir.x);
        anim.SetFloat("HitDirZ", hitDir.z);
        anim.CrossFade("HitReact", 0.3f);
    }


    // ���� �ִϸ��̼� ȣ�� �޾����� -> ���� colliderȰ��ȭ
    public void AttackOn()
    {

    }

    public void AttackOff()
    {

    }
}
