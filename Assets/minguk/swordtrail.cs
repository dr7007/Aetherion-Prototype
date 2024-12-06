using UnityEngine;

public class swordtrail : MonoBehaviour
{
    private TrailRenderer trailRenderer;
    private Animator animator;

    public string[] targetAnimationStates = { "Combo1", "Combo2", "Combo3" }; 

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        animator = GetComponentInParent<Animator>(); 
        trailRenderer.emitting = false; 
    }

    void Update()
    {
        if (IsInAnyAnimationState(targetAnimationStates))
        {
            trailRenderer.emitting = true;
        }
        else
        {
            trailRenderer.emitting = false;
        }
        
    }

    private bool IsInAnyAnimationState(string[] stateNames)
    {
        foreach (string stateName in stateNames)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                return true;
            }
        }
        return false;
    }
}


