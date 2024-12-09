using UnityEngine;

public class SwordTrail : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private TrailRenderer combo1Trail;
    [SerializeField] private TrailRenderer combo2Trail;
    [SerializeField] private TrailRenderer combo3Trail;

    public string[] targetAnimationStates = { "Combo1", "Combo2", "Combo3" };

    // 애니메이션 종료 조기 시점 (0.9는 90% 진행 시 종료)
    private float trailEndOffset = 0.8f;

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
}





