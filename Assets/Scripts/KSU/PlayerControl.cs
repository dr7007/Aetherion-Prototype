using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // ī�޶� ������ ��Ÿ�״� orientation
    public Transform orientation;
    public Transform cameraTr;
    public float rotSpeed = 7f;
    public float moveSpeed = 3f;


    private Animator anim;
    private CharacterController controller;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        // �߷±���
        Vector3 gravity = new Vector3(0f, -9.81f * Time.deltaTime, 0f);
        controller.Move(gravity);

        // ī�޶� �ٶ󺸴� �������� ���⼳��
        Vector3 viewDir = transform.position - new Vector3(cameraTr.position.x, transform.position.y, cameraTr.position.z);
        orientation.forward = viewDir.normalized;

        // ������ ����
        float axisH = Input.GetAxis("Horizontal");
        float axisV = Input.GetAxis("Vertical");

        // ī�޶� �������� ������ ���������� �� ����
        Vector3 inputDir = orientation.forward * axisV + orientation.right * axisH;

        // ���������� �÷��̾� ���� ����
        if (inputDir != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, inputDir.normalized, Time.deltaTime * rotSpeed);
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RollAnimCoroutine());
        }

        // �ش� �������� �̵�
        if (controller.isGrounded)
        {
            controller.SimpleMove(inputDir * moveSpeed);
        }
    }

    private IEnumerator RollAnimCoroutine()
    {
        anim.SetBool("IsSpace", true);

        yield return new WaitForSeconds(0.5f);

        anim.SetBool("IsSpace", false);
    }
}

