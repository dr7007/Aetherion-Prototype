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


    // 도끼로 바꾸는 이벤트 콜백
    private void ChangeAxe()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        axe.SetActive(true);
    }

    // 상태를 되돌리는 이벤트 콜백
    private void ResetState()
    {
        sword.SetActive(true);
        shield.SetActive(true);
        axe.SetActive(false);
    }
}
