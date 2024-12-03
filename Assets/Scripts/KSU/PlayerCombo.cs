using UnityEngine;

public class PlayerCombo : MonoBehaviour
{
    private Animator anim;

    private bool comboOn;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("AttackTrigger");
        }

        if (comboOn && Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("ComboTrigger");
        }
    }
    private void ComboOn()
    {
        comboOn = true;
    }

    private void ComboOff()
    {
        comboOn = false;
    }
}
