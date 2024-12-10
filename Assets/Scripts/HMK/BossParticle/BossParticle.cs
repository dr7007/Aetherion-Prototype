using UnityEngine;

public class BossParticle : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private TrailRenderer noattack;
    [SerializeField] private GameObject StartAttack;
    public string[] targetAnimationStates = { "NoReactAttack" };

    private float trailEndOffset = 0.8f;
    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        DisableAllTrails();
    }
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // ComboB1 상태
        if (stateInfo.IsName("NoReactAttack"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableParticleGameObject(StartAttack);
                EnableTrail(noattack);

            }
            else
            {
                DisableParticleGameObject(StartAttack);
                DisableTrail(noattack); // 0.1초 일찍 TrailRenderer 비활성화
            }
        }

    }
    private void EnableTrail(TrailRenderer trail)
    {
        DisableAllTrails(); // 먼저 모든 TrailRenderer를 비활성화

        trail.emitting = true;

    }

    // 특정 TrailRenderer만 비활성화
    private void DisableTrail(TrailRenderer trail)
    {

        trail.emitting = false;
    }
    private void DisableAllTrails()
    {
        if (noattack != null) noattack.emitting = false;

    }

    private void EnableParticleGameObject(GameObject obj)
    {
        obj.SetActive(true); // GameObject 활성화
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        if (particle != null && !particle.isPlaying)
        {
            particle.Play(); // 파티클 실행
        }

    }

    // GameObject와 ParticleSystem 비활성화
    private void DisableParticleGameObject(GameObject obj)
    {
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        if (particle != null && particle.isPlaying)
        {
            particle.Stop(); // 파티클 중지
        }
        obj.SetActive(false); // GameObject 비활성화

    }
}
