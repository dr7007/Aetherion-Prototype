using UnityEngine;

public class Pillar : MonoBehaviour
{
    [SerializeField] ParticleSystem Destory;

    private void OnTriggerEnter(Collider other)
    {
        

        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // ��� ������Ʈ �ı�
            Destory.Play();
        }
    }
}

