using UnityEngine;

public class SwordTrail : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private TrailRenderer combo1Trail;
    [SerializeField] private TrailRenderer combo2Trail;
    [SerializeField] private TrailRenderer combo3Trail;
    [SerializeField] private GameObject ComboC1Trail;
    [SerializeField] private GameObject ComboC2Trail;

    [SerializeField] private GameObject blockParticleEffect; // Block ���¿��� ����� ȿ��

    private string currentAnimationState = ""; // ���� ���� ����

    private float trailEndOffset = 0.8f;
    private float trailnomaloffset = 1f;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        DisableAllTrails();
        DisableParticleGameObject(blockParticleEffect); // Block ȿ�� �ʱ�ȭ
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // ���� �ִϸ��̼� ���� Ȯ��
        string newAnimationState = stateInfo.IsName("ComboC2") ? "ComboC2" :
                                    stateInfo.IsName("ComboC1") ? "ComboC1" :
                                    stateInfo.IsName("Block") ? "Block" : "";

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
        else if (stateInfo.IsName("Block")) // Block ���� ó��
        {
            EnableParticleGameObject(blockParticleEffect);
        }
        else
        {
            DisableParticleGameObject(blockParticleEffect); // Block ���°� �ƴ� �� ��Ȱ��ȭ
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
        else if (stateName == "Block")
        {
            DisableParticleGameObject(blockParticleEffect); // Block ȿ�� ��Ȱ��ȭ
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








