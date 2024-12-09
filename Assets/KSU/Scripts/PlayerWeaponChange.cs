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

    // 검으로 바꾸는 이벤트 콜백
    private void ChangeSword()
    {
        sword.SetActive(true);
        shield.SetActive(true);
        axe.SetActive(false);

        SetWeapon(1);
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
        axe.SetActive(false);

        spear.SetActive(true);
    }

    // 바꾸는 애니메이션이 끝났을때, 무기를 도끼모드로 바꿈.
    private void EndChange()
    {
        SetWeapon(2);
    }

    // 무기 레이어 설정하는 함수
    private void SetWeapon(int _num)
    {
        // 검방패 모드
        if (_num == 1)
        {
            anim.SetLayerWeight(0, 1);
            anim.SetLayerWeight(1, 0);
            anim.CrossFade("Idle2", 0f, 0);
            anim.Play("Nothing", 1);
        }

        // 도끼모드
        else if (_num == 2)
        {
            anim.SetLayerWeight(0, 0);
            anim.SetLayerWeight(1, 1);
            anim.CrossFade("Idle2", 0f ,1);
            anim.Play("Nothing", 0);
        }
    }
}
