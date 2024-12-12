using UnityEngine;

public class SwordTrail : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private TrailRenderer combo1Trail;
    [SerializeField] private TrailRenderer combo2Trail;
    [SerializeField] private TrailRenderer combo3Trail;
    [SerializeField] private GameObject ComboC1Trail;
    [SerializeField] private GameObject ComboC2Trail;

    [SerializeField] private GameObject blockParticleEffect; // Block 상태에서 사용할 효과

    private string currentAnimationState = ""; // 현재 상태 추적

    private float trailEndOffset = 0.8f;
    private float trailnomaloffset = 1f;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        DisableAllTrails();
        DisableParticleGameObject(blockParticleEffect); // Block 효과 초기화
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 현재 애니메이션 상태 확인
        string newAnimationState = stateInfo.IsName("ComboC2") ? "ComboC2" :
                                    stateInfo.IsName("ComboC1") ? "ComboC1" :
                                    stateInfo.IsName("Block") ? "Block" : "";

        // 상태가 변경되었을 때 처리
        if (currentAnimationState != newAnimationState)
        {
            HandleStateExit(currentAnimationState); // 이전 상태 종료 처리
            currentAnimationState = newAnimationState; // 새로운 상태로 변경
        }

        // 현재 상태별 로직 처리
        if (stateInfo.IsName("Combo1"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
                EnableTrail(combo1Trail);
            else
                DisableTrail(combo1Trail);
        }
        else if (stateInfo.IsName("Combo2"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
                EnableTrail(combo2Trail);
            else
                DisableTrail(combo2Trail);
        }
        else if (stateInfo.IsName("Combo3"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
                EnableTrail(combo3Trail);
            else
                DisableTrail(combo3Trail);
        }
        else if (stateInfo.IsName("ComboC1"))
        {
            if (stateInfo.normalizedTime < trailnomaloffset)
                EnableParticleGameObject(ComboC1Trail);
            else
                DisableParticleGameObject(ComboC1Trail);
        }
        else if (stateInfo.IsName("ComboC2"))
        {
            if (stateInfo.normalizedTime < trailnomaloffset)
                EnableParticleGameObject(ComboC2Trail);
            else
                DisableParticleGameObject(ComboC2Trail);
        }
        else if (stateInfo.IsName("Block")) // Block 상태 처리
        {
            EnableParticleGameObject(blockParticleEffect);
        }
        else
        {
            DisableParticleGameObject(blockParticleEffect); // Block 상태가 아닐 때 비활성화
            DisableAllTrails();
        }
    }

    // 이전 상태 종료 처리
    private void HandleStateExit(string stateName)
    {
        if (stateName == "ComboC2")
        {
            DisableParticleGameObject(ComboC2Trail); // ComboC2Trail 비활성화
        }
        else if (stateName == "ComboC1")
        {
            DisableParticleGameObject(ComboC1Trail); // ComboC1Trail 비활성화
        }
        else if (stateName == "Block")
        {
            DisableParticleGameObject(blockParticleEffect); // Block 효과 비활성화
        }
    }

    private void EnableTrail(TrailRenderer trail)
    {
        DisableAllTrails();
        trail.emitting = true;
    }

    private void DisableTrail(TrailRenderer trail)
    {
        trail.emitting = false;
    }

    private void DisableAllTrails()
    {
        combo1Trail.emitting = false;
        combo2Trail.emitting = false;
        combo3Trail.emitting = false;
    }

    private void EnableParticleGameObject(GameObject obj)
    {
        obj.SetActive(true);
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        if (!particle.isPlaying)
        {
            particle.Play();
        }
    }

    private void DisableParticleGameObject(GameObject obj)
    {
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        if (particle.isPlaying)
        {
            particle.Stop();
        }
        obj.SetActive(false);
    }
}








