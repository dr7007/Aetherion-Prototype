using UnityEngine;

public class BossRun : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject run;
    public string[] targetAnimationStates = { "run" };
    [SerializeField]private float trailEndOffset = 0.8f;
    private void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("FirstDetect"))
        {
            if (stateInfo.normalizedTime < trailEndOffset)
            {

                EnableParticleGameObject(run);

            }
            else
            {

                DisableParticleGameObject(run); 
            }
        }
    }
    private void DisableParticleGameObject(GameObject obj)
    {
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        if (particle != null && particle.isPlaying)
        {
            particle.Stop(); // ��ƼŬ ����
        }
        obj.SetActive(false); // GameObject ��Ȱ��ȭ

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
}
