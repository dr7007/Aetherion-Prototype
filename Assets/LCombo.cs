using UnityEngine;

public class LCombo : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private TrailRenderer comboB1Trail;
    [SerializeField] private TrailRenderer comboB2Trail;
    [SerializeField] private TrailRenderer comboB3Trail;

    public string[] targetAnimationStates = { "Combo1", "Combo2", "Combo3" };

    // 애니메이션 종료 조기 시점 (0.9는 90% 진행 시 종료)
    private float trailEndOffset = 0.8f;
    private float trailEndOffset7 = 0.7f;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        DisableAllTrails();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);

        // ComboB1 상태
        if (stateInfo.IsName("Combo1"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableTrail(comboB1Trail);
            }
            else
            {
                DisableTrail(comboB1Trail); // 0.1초 일찍 TrailRenderer 비활성화
            }
        }
        // ComboB2 상태
        else if (stateInfo.IsName("Combo2"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableTrail(comboB2Trail);
            }
            else
            {
                DisableTrail(comboB2Trail); // 0.1초 일찍 TrailRenderer 비활성화
            }
        }
        // ComboB3 상태
        else if (stateInfo.IsName("Combo3"))
        {
            if (stateInfo.normalizedTime < trailEndOffset7)
            {
                EnableTrail(comboB3Trail);
            }
            else
            {
                DisableTrail(comboB3Trail); // 0.1초 일찍 TrailRenderer 비활성화
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
        if (comboB1Trail != null) comboB1Trail.emitting = false;
        if (comboB2Trail != null) comboB2Trail.emitting = false;
        if (comboB3Trail != null) comboB3Trail.emitting = false;
    }
}
