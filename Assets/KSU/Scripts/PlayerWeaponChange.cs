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

    void Start()
    {
        // Base Layer�� Weight�� 0���� ���� (����)
        anim.SetLayerWeight(0, 0);

        // �ٸ� ���̾��� Weight�� 1�� ���� (Ȱ��ȭ)
        anim.SetLayerWeight(1, 1);
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
        spear.SetActive(true);
    }

    // ���¸� �ǵ����� �̺�Ʈ �ݹ�
    private void ResetState()
    {
        sword.SetActive(true);
        shield.SetActive(true);
        axe.SetActive(false);
        spear.SetActive(false);
    }
}
