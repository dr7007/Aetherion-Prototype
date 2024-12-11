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

    // 그냥 TriggerEnter에서 처리하면 충돌이 나서 플레이어 위치 이동 안하는 현상 발생 -> LateUpdate로 처리해줌.
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
