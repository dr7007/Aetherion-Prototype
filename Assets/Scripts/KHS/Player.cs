using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 10f;
    [SerializeField]private bool isPlayerAttacking = false;

    public bool IsPlayerAttacking
    {
        get { return isPlayerAttacking; }
        set {  isPlayerAttacking = value; }
    }

    private void Update()
    {
        PlayerMoveFunc();
        PlayerAttackFunc();
    }

    private void PlayerMoveFunc()
    {
        // 입력 값 가져오기
        float horizontalInput = Input.GetAxis("Horizontal"); // A, D 또는 왼쪽/오른쪽 화살표
        float verticalInput = Input.GetAxis("Vertical");     // W, S 또는 위/아래 화살표

        // 이동 벡터 계산
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);

        // 정규화된 이동 벡터와 속도를 곱해 이동
        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
    }
    private void PlayerAttackFunc()
    {
        if (Input.GetMouseButtonDown(0))
            isPlayerAttacking = true;
        else
            return;

    }
}
