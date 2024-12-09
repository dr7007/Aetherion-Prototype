using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerWeaponChange : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject axe;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    // ������ �ٲٴ� �̺�Ʈ �ݹ�
    private void ChangeAxe()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        axe.SetActive(true);
    }

    // ���¸� �ǵ����� �̺�Ʈ �ݹ�
    private void ResetState()
    {
        sword.SetActive(true);
        shield.SetActive(true);
        axe.SetActive(false);
    }
}
