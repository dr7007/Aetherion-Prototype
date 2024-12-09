using UnityEngine;
using UnityEngine.UI;
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject[] combo1img;
        
    public string[] targetAnimationStates = { "Combo1", "Combo2", "Combo3" };

    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Combo1"))
        {
            combo1img[0].SetActive(false);            
        }
        if (stateInfo.IsName("Combo2"))
        {
            combo1img[1].SetActive(false);
            
        }
        if(stateInfo.IsName("Combo3"))
        {
            combo1img[2].SetActive(false);
            if (stateInfo.normalizedTime >= 1f) 
            {
                ResetImages(); 
            }
        }
    }
    private void ResetImages()
    {
        foreach (GameObject img in combo1img)
        {
            if (img != null)
                img.SetActive(true); 
        }
    }
}
