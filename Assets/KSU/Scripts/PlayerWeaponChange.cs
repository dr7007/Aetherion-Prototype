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
        // Base Layer의 Weight를 0으로 설정 (무시)
        anim.SetLayerWeight(0, 0);

        // 다른 레이어의 Weight를 1로 설정 (활성화)
        anim.SetLayerWeight(1, 1);
    }


    // 도끼로 바꾸는 이벤트 콜백
    private void ChangeAxe()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        axe.SetActive(true);
    }

    // 창으로 바꾸는 이벤트 콜백
    private void ChangeSpear()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        spear.SetActive(true);
    }

    // 상태를 되돌리는 이벤트 콜백
    private void ResetState()
    {
        sword.SetActive(true);
        shield.SetActive(true);
        axe.SetActive(false);
        spear.SetActive(false);
    }
}
