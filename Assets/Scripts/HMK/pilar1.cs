using UnityEngine;

public class Pillar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // ��� ������Ʈ �ı�
            Debug.Log($"{gameObject.name}��{other.name}�� �浹");
        }
    }
}

