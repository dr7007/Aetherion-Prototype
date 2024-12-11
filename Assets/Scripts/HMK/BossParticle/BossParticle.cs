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

        // ComboB1 ����
        if (stateInfo.IsName("NoReactAttack"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                
                EnableTrail(noattack);

            }
            else
            {
                
                DisableTrail(noattack); // 0.1�� ���� TrailRenderer ��Ȱ��ȭ
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

                DisableTrail(noattack); // 0.1�� ���� TrailRenderer ��Ȱ��ȭ
            }
        }
        if (stateInfo.IsName("Healing"))
        {
            if (!IsHeal)
            {
                IsHeal = true;
                Debug.Log("�ڷ�ƾ�� ��� ȣ��ɱ�");
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

                DisableParticleGameObject(StartAttack); // 0.1�� ���� TrailRenderer ��Ȱ��ȭ
            }
        }

    }
    public void FirstAttackParticle()
    {
            teleportParticleSystem.Play(); // ��ƼŬ ����
            Debug.Log("FirstAttackParticle �̺�Ʈ �����: ��ƼŬ ����");
        
    }
    public void tp()
    {
        teleportParticleSystem2.Play();
        Debug.Log("���� ��");
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
        obj.SetActive(true); // GameObject Ȱ��ȭ
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        if (particle != null && !particle.isPlaying)
        {
            particle.Play(); // ��ƼŬ ����
        }

    }
    private IEnumerator Particle()
    {
        while (true)
        {
            bossheal.SetActive(true);
            yield return new WaitForSeconds(1f); // 1�� ���
            bossheal.SetActive(false);

            Debug.Log("�� ��ƼŬ �����");
            // ���� ������ -> BREAK
            // if (animator.GetBool("������") == false) break;
        }

        IsHeal = false;
    }

    // GameObject�� ParticleSystem ��Ȱ��ȭ
    private void DisableParticleGameObject(GameObject obj)
    {
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        if (particle != null && particle.isPlaying)
        {
            particle.Stop(); // ��ƼŬ ����
        }
        obj.SetActive(false); // GameObject ��Ȱ��ȭ

    }
}
