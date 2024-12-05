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
        // �Է� �� ��������
        float horizontalInput = Input.GetAxis("Horizontal"); // A, D �Ǵ� ����/������ ȭ��ǥ
        float verticalInput = Input.GetAxis("Vertical");     // W, S �Ǵ� ��/�Ʒ� ȭ��ǥ

        // �̵� ���� ���
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);

        // ����ȭ�� �̵� ���Ϳ� �ӵ��� ���� �̵�
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
