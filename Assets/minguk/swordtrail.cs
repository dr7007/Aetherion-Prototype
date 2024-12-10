using UnityEngine;

public class SwordTrail : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private TrailRenderer combo1Trail;
    [SerializeField] private TrailRenderer combo2Trail;
    [SerializeField] private TrailRenderer combo3Trail;
    [SerializeField] private GameObject ComboC1Trail;
    [SerializeField] private GameObject ComboC2Trail;
    [SerializeField] private GameObject guard;

    public string[] targetAnimationStates = { "Combo1", "Combo2", "Combo3", "ComboC1", "ComboC2" };

    // 애니메이션 종료 조기 시점 (0.9는 90% 진행 시 종료)
    private float trailEndOffset = 0.8f;
    private float trailnomaloffset = 1f;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        DisableAllTrails();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Combo1 상태
        if (stateInfo.IsName("Combo1"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableTrail(combo1Trail);
            }
            else
            {
                DisableTrail(combo1Trail); // 0.1초 일찍 TrailRenderer 비활성화
            }
        }
        // Combo2 상태
        else if (stateInfo.IsName("Combo2"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableTrail(combo2Trail);
            }
            else
            {
                DisableTrail(combo2Trail); // 0.1초 일찍 TrailRenderer 비활성화
            }
        }
        // Combo3 상태
        else if (stateInfo.IsName("Combo3"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableTrail(combo3Trail);
            }
            else
            {
                DisableTrail(combo3Trail); // 0.1초 일찍 TrailRenderer 비활성화
            }
        }
        else if (stateInfo.IsName("ComboC1"))
        {
            if (stateInfo.normalizedTime < trailnomaloffset)
            {
                EnableParticleGameObject(ComboC1Trail); // GameObject와 파티클 활성화 및 실행
            }
            else
            {
                DisableParticleGameObject(ComboC1Trail); // GameObject와 파티클 비활성화
            }
        }
        else if (stateInfo.IsName("ComboC2"))
        {
            if (stateInfo.normalizedTime < trailnomaloffset)
            {
                EnableParticleGameObject(ComboC2Trail); // GameObject와 파티클 활성화 및 실행
            }
            else
            {
                DisableParticleGameObject(ComboC2Trail); // GameObject와 파티클 비활성화
            }
        }
        else if (stateInfo.IsName("Block"))
        {
            if (stateInfo.normalizedTime < trailnomaloffset)
            {
                EnableParticleGameObject(guard); // GameObject와 파티클 활성화 및 실행
            }
            else
            {
                DisableParticleGameObject(guard); // GameObject와 파티클 비활성화
            }
        }

        // 다른 상태에서는 모든 TrailRenderer 비활성화
        else
        {
            DisableAllTrails();
        }
    }

    // 특정 TrailRenderer만 활성화
    private void EnableTrail(TrailRenderer trail)
    {
        DisableAllTrails(); // 먼저 모든 TrailRenderer를 비활성화
        if (trail != null)
        {
            trail.emitting = true;
        }
    }

    // 특정 TrailRenderer만 비활성화
    private void DisableTrail(TrailRenderer trail)
    {
        if (trail != null)
        {
            trail.emitting = false;
        }
    }

    // 모든 TrailRenderer 비활성화
    private void DisableAllTrails()
    {
        if (combo1Trail != null) combo1Trail.emitting = false;
        if (combo2Trail != null) combo2Trail.emitting = false;
        if (combo3Trail != null) combo3Trail.emitting = false;
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





