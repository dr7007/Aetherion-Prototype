using UnityEngine;

public class LCombo : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private TrailRenderer comboB1Trail;
    [SerializeField] private TrailRenderer comboB2Trail;
    [SerializeField] private TrailRenderer comboB3Trail;
    [SerializeField] private GameObject ComboC2Trail;
    [SerializeField] private GameObject ComboC1Trail;

    public string[] targetAnimationStates = { "Combo1", "Combo2", "Combo3", "ComboC1", "ComboC2" };

    private string currentAnimationState = ""; // ���� ���� ����

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

        // ���� �ִϸ��̼� ���� Ȯ��
        string newAnimationState = stateInfo.IsName("ComboC2") ? "ComboC2" : stateInfo.IsName("ComboC1") ? "ComboC1" : "";

        // ���°� ����Ǿ��� �� ó��
        if (currentAnimationState != newAnimationState)
        {
            HandleStateExit(currentAnimationState); // ���� ���� ���� ó��
            currentAnimationState = newAnimationState; // ���ο� ���·� ����
        }

        // ���� ���º� ���� ó��
        if (stateInfo.IsName("Combo1"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
                EnableTrail(comboB1Trail);
            else
                DisableTrail(comboB1Trail);
        }
        else if (stateInfo.IsName("Combo2"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
                EnableTrail(comboB2Trail);
            else
                DisableTrail(comboB2Trail);
        }
        else if (stateInfo.IsName("Combo3"))
        {
            if (stateInfo.normalizedTime < trailEndOffset7)
                EnableTrail(comboB3Trail);
            else
                DisableTrail(comboB3Trail);
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
        else
        {
            DisableAllTrails();
        }
    }

    // ���� ���� ���� ó��
    private void HandleStateExit(string stateName)
    {
        if (stateName == "ComboC2")
        {
            DisableParticleGameObject(ComboC2Trail); // ComboC2Trail ��Ȱ��ȭ
        }
        else if (stateName == "ComboC1")
        {
            DisableParticleGameObject(ComboC1Trail); // ComboC1Trail ��Ȱ��ȭ
        }
    }

    private void EnableTrail(TrailRenderer trail)
    {
        DisableAllTrails();
        if (trail != null)
        {
            trail.emitting = true;
        }
    }

    private void DisableTrail(TrailRenderer trail)
    {
        if (trail != null)
        {
            trail.emitting = false;
        }
    }

    private void DisableAllTrails()
    {
        if (comboB1Trail != null) comboB1Trail.emitting = false;
        if (comboB2Trail != null) comboB2Trail.emitting = false;
        if (comboB3Trail != null) comboB3Trail.emitting = false;
    }

    private void EnableParticleGameObject(GameObject obj)
    {
        obj.SetActive(true);
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        if (particle != null && !particle.isPlaying)
        {
            particle.Play();
        }
    }

    private void DisableParticleGameObject(GameObject obj)
    {
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        if (particle != null && particle.isPlaying)
        {
            particle.Stop();
        }
        obj.SetActive(false);
    }
}

