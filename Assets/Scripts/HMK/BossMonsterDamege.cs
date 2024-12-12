using UnityEngine;

public class BossHitHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticle;  // 플레이어 공격 시 발생할 파티클 시스템
    [SerializeField] private string playerAttackTag = "PlayerAttack";  // 플레이어 공격 태그
    [SerializeField] private ScreenHitEffect screenHitEffect; // 화면 충격 효과 스크립트
    
    // 트리거 충돌 시 호출
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어의 공격 태그와 충돌했는지 확인
        if (other.CompareTag(playerAttackTag))
        {
            // 충돌 지점 계산
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            
            // 충돌 방향 계산
            Vector3 direction = (hitPoint - transform.position).normalized;

            // 충돌 지점에서 파티클 실행
            PlayHitParticle(hitPoint, direction);

            // 화면 충격 효과 실행
            TriggerScreenHitEffect();
        }
    }

    // 파티클 시스템 실행 함수
    private void PlayHitParticle(Vector3 position, Vector3 direction)
    {
        // 파티클 시스템 위치를 충돌 지점으로 이동
        hitParticle.transform.position = position;

        // 충돌 방향을 기준으로 파티클 시스템 회전 설정
        hitParticle.transform.rotation = Quaternion.LookRotation(direction);

        // 파티클 실행
        hitParticle.Play();
        
    }

    // 화면 충격 효과 실행 함수
    public void TriggerScreenHitEffect()
    {
        //효과 실행        
        screenHitEffect.TriggerHitEffect();
       
    }
}








