using UnityEngine;

public class BossHitHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticle;  // �÷��̾� ���� �� �߻��� ��ƼŬ �ý���
    [SerializeField] private string playerAttackTag = "PlayerAttack";  // �÷��̾� ���� �±�
    [SerializeField] private ScreenHitEffect screenHitEffect; // ȭ�� ��� ȿ�� ��ũ��Ʈ
    
    // Ʈ���� �浹 �� ȣ��
    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾��� ���� �±׿� �浹�ߴ��� Ȯ��
        if (other.CompareTag(playerAttackTag))
        {
            // �浹 ���� ���
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            
            // �浹 ���� ���
            Vector3 direction = (hitPoint - transform.position).normalized;

            // �浹 �������� ��ƼŬ ����
            PlayHitParticle(hitPoint, direction);

            // ȭ�� ��� ȿ�� ����
            TriggerScreenHitEffect();
        }
    }

    // ��ƼŬ �ý��� ���� �Լ�
    private void PlayHitParticle(Vector3 position, Vector3 direction)
    {
        // ��ƼŬ �ý��� ��ġ�� �浹 �������� �̵�
        hitParticle.transform.position = position;

        // �浹 ������ �������� ��ƼŬ �ý��� ȸ�� ����
        hitParticle.transform.rotation = Quaternion.LookRotation(direction);

        // ��ƼŬ ����
        hitParticle.Play();
        
    }

    // ȭ�� ��� ȿ�� ���� �Լ�
    public void TriggerScreenHitEffect()
    {
        //ȿ�� ����        
        screenHitEffect.TriggerHitEffect();
       
    }
}








