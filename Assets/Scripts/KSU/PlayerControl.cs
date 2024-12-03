using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // ī�޶� ������ ��Ÿ�״� orientation
    public Transform orientation;
    public Transform cameraTr;
    public float rotSpeed = 7f;
    public float moveSpeed = 3f;


    private bool isAttacking = false;
    private Animator anim;
    private CharacterController controller;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        CheckAttack();

        // Ű�Է� ����
        float axisH = Input.GetAxis("Horizontal");
        float axisV = Input.GetAxis("Vertical");

        if (!isAttacking)
        {
            PlayerMove(axisV, axisH);
        }

        // �����̽��� ��������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RollAnimCoroutine());
        }

    }

    private IEnumerator RollAnimCoroutine()
    {
        anim.SetBool("IsSpace", true);

        yield return new WaitForSeconds(0.5f);

        anim.SetBool("IsSpace", false);
    }

    // �÷��̾� �̵� ���� �Լ�
    private void PlayerMove(float _axisV, float _axisH)
    {
        // �߷±���
        Vector3 gravity = new Vector3(0f, -9.81f * Time.deltaTime, 0f);
        controller.Move(gravity);

        // ī�޶� �ٶ󺸴� �������� ���⼳��
        Vector3 viewDir = transform.position - new Vector3(cameraTr.position.x, transform.position.y, cameraTr.position.z);
        orientation.forward = viewDir.normalized;

        // ī�޶� �������� ������ ���������� �� ����
        Vector3 inputDir = orientation.forward * _axisV + orientation.right * _axisH;

        // ���������� �÷��̾� ���� ����
        if (inputDir != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, inputDir.normalized, Time.deltaTime * rotSpeed);
            anim.SetBool("IsMoving", true);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }

        // ���� ����Ʈ ��������
        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("IsFast", true);
            moveSpeed = 6f;
        }
        else
        {
            anim.SetBool("IsFast", false);
            moveSpeed = 3f;
        }

        // �ش� �������� �̵�
        if (controller.isGrounded)
        {
            controller.SimpleMove(inputDir * moveSpeed);
        }
    }


    // ���� ���������� Ȯ���ϴ� �Լ�
    private void CheckAttack()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // ���� ���꽺����Ʈ ���� ���� Ȯ��
        if (stateInfo.IsName("SwordAttack.combo1") || stateInfo.IsName("SwordAttack.combo2") || stateInfo.IsName("SwordAttack.combo3"))
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
    }
}
