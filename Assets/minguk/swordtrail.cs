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

    // �ִϸ��̼� ���� ���� ���� (0.9�� 90% ���� �� ����)
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

        // Combo1 ����
        if (stateInfo.IsName("Combo1"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableTrail(combo1Trail);
            }
            else
            {
                DisableTrail(combo1Trail); // 0.1�� ���� TrailRenderer ��Ȱ��ȭ
            }
        }
        // Combo2 ����
        else if (stateInfo.IsName("Combo2"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableTrail(combo2Trail);
            }
            else
            {
                DisableTrail(combo2Trail); // 0.1�� ���� TrailRenderer ��Ȱ��ȭ
            }
        }
        // Combo3 ����
        else if (stateInfo.IsName("Combo3"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {
                EnableTrail(combo3Trail);
            }
            else
            {
                DisableTrail(combo3Trail); // 0.1�� ���� TrailRenderer ��Ȱ��ȭ
            }
        }
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
        else if (stateInfo.IsName("Block"))
        {
            if (stateInfo.normalizedTime < trailnomaloffset)
            {
                EnableParticleGameObject(guard); // GameObject�� ��ƼŬ Ȱ��ȭ �� ����
            }
            else
            {
                DisableParticleGameObject(guard); // GameObject�� ��ƼŬ ��Ȱ��ȭ
            }
        }

        // �ٸ� ���¿����� ��� TrailRenderer ��Ȱ��ȭ
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
        if (combo1Trail != null) combo1Trail.emitting = false;
        if (combo2Trail != null) combo2Trail.emitting = false;
        if (combo3Trail != null) combo3Trail.emitting = false;
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





