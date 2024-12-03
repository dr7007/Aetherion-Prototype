using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // 카메라 방향을 나타네는 orientation
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
        // 중력구현
        Vector3 gravity = new Vector3(0f, -9.81f * Time.deltaTime, 0f);
        controller.Move(gravity);

        // 카메라 바라보는 방향으로 방향설정
        Vector3 viewDir = transform.position - new Vector3(cameraTr.position.x, transform.position.y, cameraTr.position.z);
        orientation.forward = viewDir.normalized;

        // 움직임 감지
        float axisH = Input.GetAxis("Horizontal");
        float axisV = Input.GetAxis("Vertical");

        // 카메라 방향으로 움직임 감지했을때 값 저장
        Vector3 inputDir = orientation.forward * axisV + orientation.right * axisH;

        // 움직였을때 플레이어 방향 조절
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

        // 해당 방향으로 이동
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

