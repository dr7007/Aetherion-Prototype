using UnityEngine;
using System.Collections;

public class BossParticle : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private TrailRenderer noattack;
    [SerializeField] private GameObject StartAttack;
    [SerializeField] private ParticleSystem teleportParticleSystem;
    [SerializeField] private ParticleSystem teleportParticleSystem2;
    [SerializeField] private GameObject bossheal;
    public string[] targetAnimationStates = { "NoReactAttack", "FirstDetect" , "Healing" };
    [SerializeField] private AudioSource audioSource;
    private float trailEndOffset = 0.8f;
    private bool IsHeal = false;

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
                
                EnableTrail(noattack);

            }
            else
            {
                
                DisableTrail(noattack); // 0.1초 일찍 TrailRenderer 비활성화
            }
        }
        if (stateInfo.IsName("ResponsiveAttack"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {

                EnableTrail(noattack);

            }
            else
            {

                DisableTrail(noattack); // 0.1초 일찍 TrailRenderer 비활성화
            }
        }
        if (stateInfo.IsName("Healing"))
        {
            if (!IsHeal)
            {
                IsHeal = true;
                Debug.Log("코루틴이 몇번 호출될까");
                StartCoroutine(Particle());
            }



        }
        if (stateInfo.IsName("FirstDetect"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {

                EnableParticleGameObject(StartAttack);

            }
            else
            {

                DisableParticleGameObject(StartAttack); // 0.1초 일찍 TrailRenderer 비활성화
            }
        }

    }
    public void FirstAttackParticle()
    {
            teleportParticleSystem.Play(); // 파티클 실행
            Debug.Log("FirstAttackParticle 이벤트 실행됨: 파티클 실행");
        
    }
    public void tp()
    {
        teleportParticleSystem2.Play();
        Debug.Log("텔포 후");
    }
    public void EnableParticleObject()
    {
        if (StartAttack != null)
        {
            StartAttack.SetActive(true);
        }
    }

    public void DisableParticleObject()
    {
        if (StartAttack != null)
        {
            StartAttack.SetActive(false);
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
    private IEnumerator Particle()
    {
        while (true)
        {
            bossheal.SetActive(true);
            yield return new WaitForSeconds(1f); // 1초 대기
            bossheal.SetActive(false);

            Debug.Log("힐 파티클 실행됨");
            // 힐밴 됬을때 -> BREAK
            // if (animator.GetBool("힐상태") == false) break;
        }

        IsHeal = false;
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
