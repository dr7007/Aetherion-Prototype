using UnityEngine;

public class SwordTrail : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private TrailRenderer combo1Trail;
    [SerializeField] private TrailRenderer combo2Trail;
    [SerializeField] private TrailRenderer combo3Trail;

    public string[] targetAnimationStates = { "Combo1", "Combo2", "Combo3" };

    // �ִϸ��̼� ���� ���� ���� (0.9�� 90% ���� �� ����)
    private float trailEndOffset = 0.8f;

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
}





