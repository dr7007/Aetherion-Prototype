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
    public int curWeaponNum;

    public int CurWeaponNum
    {
        get { return curWeaponNum; }
        set { curWeaponNum = value; }
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
        anim.Play("Nothing", 1);
        curWeaponNum = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && curWeaponNum == 1)
        {
            anim.SetBool("IsChange", true);
            Invoke("SetSwordWeapon", 1f);
        }
    }

    // 검으로 바꾸는 이벤트 콜백
    private void ChangeSword()
    {
        sword.SetActive(true);
        shield.SetActive(true);
        axe.SetActive(false);
        spear.SetActive(false);
    }

    // 도끼로 바꾸는 이벤트 콜백
    private void ChangeAxe()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        axe.SetActive(true);
        spear.SetActive(false);
    }

    // 창으로 바꾸는 이벤트 콜백
    private void ChangeSpear()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        axe.SetActive(false);

        spear.SetActive(true);
    }

    // 바꾸는 애니메이션이 끝났을때, 무기 애니메이션을 알맞게 바꿈
    private void EndChange(int _num)
    {
        SetWeapon(_num);
    }

    private void EndSpear()
    {
        if (curWeaponNum == 0)
        {
            ChangeSword();
        }
        else
        {
            ChangeAxe();
        }
    }

    // 무기 레이어 설정하는 함수
    private void SetWeapon(int _num)
    {
        curWeaponNum = _num;
        // 검방패 모드
        if (_num == 0)
        {
            anim.SetLayerWeight(0, 1);
            anim.SetLayerWeight(1, 0);
            anim.CrossFade("Idle2", 0f, 0);
            anim.Play("Nothing", 1);
        }
        // 도끼모드
        else if (_num == 1)
        {
            anim.SetLayerWeight(0, 0);
            anim.SetLayerWeight(1, 1);
            anim.CrossFade("Idle2", 0f ,1);
            anim.Play("Nothing", 0);
        }
    }

    private void SetSwordWeapon()
    {
        SetWeapon(0);
        ChangeSword();
        anim.SetBool("IsChange", false);
    }
}
