using UnityEngine;

public class LCombo : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private TrailRenderer comboB1Trail;
    [SerializeField] private TrailRenderer comboB2Trail;
    [SerializeField] private TrailRenderer comboB3Trail;
    [SerializeField] private GameObject ComboC2Trail;
    [SerializeField] private GameObject ComboC1Trail;

    public string[] targetAnimationStates = { "Combo1", "Combo2", "Combo3", "ComboC1", "ComboC2"};

    // �ִϸ��̼� ���� ���� ���� (0.9�� 90% ���� �� ����)
    private float trailEndOffset = 0.8f;
    private float trailEndOffset7 = 0.7f;
    private float trailnomaloffset = 1f;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        DisableAllTrails();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);

        // ComboB1 ����
        if (stateInfo.IsName("Combo1"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableTrail(comboB1Trail);
            }
            else
            {
                DisableTrail(comboB1Trail); // 0.1�� ���� TrailRenderer ��Ȱ��ȭ
            }
        }
        // ComboB2 ����
        else if (stateInfo.IsName("Combo2"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableTrail(comboB2Trail);
            }
            else
            {
                DisableTrail(comboB2Trail); // 0.1�� ���� TrailRenderer ��Ȱ��ȭ
            }
        }
        // ComboB3 ����
        else if (stateInfo.IsName("Combo3"))
        {
            if (stateInfo.normalizedTime < trailEndOffset7)
            {
                EnableTrail(comboB3Trail);
            }
            else
            {
                DisableTrail(comboB3Trail); // 0.1�� ���� TrailRenderer ��Ȱ��ȭ
            }
        }
        // �ٸ� ���¿����� ��� TrailRenderer ��Ȱ��ȭ
        else if (stateInfo.IsName("ComboC1"))
        {
            if (stateInfo.normalizedTime < trailnomaloffset)
            {
                EnableParticleGameObject(ComboC1Trail); // GameObject�� ��ƼŬ Ȱ��ȭ �� ����
            }
            else
            {
                DisableParticleGameObject(ComboC1Trail); // GameObject�� ��ƼŬ ��Ȱ��ȭ
            }
        }
        else if (stateInfo.IsName("ComboC2"))
        {
            if (stateInfo.normalizedTime < trailnomaloffset)
            {
                EnableParticleGameObject(ComboC2Trail); // GameObject�� ��ƼŬ Ȱ��ȭ �� ����
            }
            else
            {
                DisableParticleGameObject(ComboC2Trail); // GameObject�� ��ƼŬ ��Ȱ��ȭ
            }
        }
        else
        {
            DisableAllTrails();
        }
    }

    // Ư�� TrailRenderer�� Ȱ��ȭ
    private void EnableTrail(TrailRenderer trail)
    {
        DisableAllTrails(); // ���� ��� TrailRenderer�� ��Ȱ��ȭ
        if (trail != null)
        {
            trail.emitting = true;
        }
    }

    // Ư�� TrailRenderer�� ��Ȱ��ȭ
    private void DisableTrail(TrailRenderer trail)
    {
        if (trail != null)
        {
            trail.emitting = false;
        }
    }

    // ��� TrailRenderer ��Ȱ��ȭ
    private void DisableAllTrails()
    {
        if (comboB1Trail != null) comboB1Trail.emitting = false;
        if (comboB2Trail != null) comboB2Trail.emitting = false;
        if (comboB3Trail != null) comboB3Trail.emitting = false;
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