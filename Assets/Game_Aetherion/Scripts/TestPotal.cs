using UnityEngine;

public class TestPotal : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition = Vector3.zero;
    [SerializeField] private GameObject player;

    private bool setPosition = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            setPosition = true;
        }
    }

    // �׳� TriggerEnter���� ó���ϸ� �浹�� ���� �÷��̾� ��ġ �̵� ���ϴ� ���� �߻� -> LateUpdate�� ó������.
    private void LateUpdate()
    {
        if (setPosition)
        {
            player.transform.position = startPosition;
            Invoke("SetPositionFalse", 0.5f);
        }
    }

    private void SetPositionFalse()
    {
        setPosition = false;
    }
}
