using UnityEngine;

public class LCombo : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private TrailRenderer comboB1Trail;
    [SerializeField] private TrailRenderer comboB2Trail;
    [SerializeField] private TrailRenderer comboB3Trail;

    public string[] targetAnimationStates = { "Combo1", "Combo2", "Combo3" };

    // �ִϸ��̼� ���� ���� ���� (0.9�� 90% ���� �� ����)
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
}
