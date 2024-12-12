using UnityEngine;

public class Pillar : MonoBehaviour
{
    [SerializeField] ParticleSystem Destory;

    private void OnTriggerEnter(Collider other)
    {
        

        if (other.CompareTag("BossMonster"))
        {
            gameObject.SetActive(false); // ��� ������Ʈ �ı�
            Destory.Play();
        }
    }
}

