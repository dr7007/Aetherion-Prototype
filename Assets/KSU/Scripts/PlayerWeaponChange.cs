using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerWeaponChange : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject axe;
    [SerializeField] private GameObject spear;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
        anim.Play("Nothing", 1);
    }

    // ������ �ٲٴ� �̺�Ʈ �ݹ�
    private void ChangeSword()
    {
        sword.SetActive(true);
        shield.SetActive(true);
        axe.SetActive(false);

        SetWeapon(1);
    }

    // ������ �ٲٴ� �̺�Ʈ �ݹ�
    private void ChangeAxe()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        axe.SetActive(true);
    }

    // â���� �ٲٴ� �̺�Ʈ �ݹ�
    private void ChangeSpear()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        axe.SetActive(false);

        spear.SetActive(true);
    }

    // �ٲٴ� �ִϸ��̼��� ��������, ���⸦ �������� �ٲ�.
    private void EndChange()
    {
        SetWeapon(2);
    }

    // ���� ���̾� �����ϴ� �Լ�
    private void SetWeapon(int _num)
    {
        // �˹��� ���
        if (_num == 1)
        {
            anim.SetLayerWeight(0, 1);
            anim.SetLayerWeight(1, 0);
            anim.CrossFade("Idle2", 0f, 0);
            anim.Play("Nothing", 1);
        }

        // �������
        else if (_num == 2)
        {
            anim.SetLayerWeight(0, 0);
            anim.SetLayerWeight(1, 1);
            anim.CrossFade("Idle2", 0f ,1);
            anim.Play("Nothing", 0);
        }
    }
}
