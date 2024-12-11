using System.Collections;
using UnityEngine;
using static PlayerAnim;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loading = null;
    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject boss = null;
    [SerializeField] private GameObject followCam = null;
    [SerializeField] private Vector3 TestPosition = Vector3.zero;
    [SerializeField] private Vector3 StartPosition = Vector3.zero;
    
    private string ClickName = null;
    private Animator playerAnimInfo;
    private PlayerWeaponChange playerWeaponInfo;
    private PlayerBattle playerBattleInfo;

    private void Start()
    {
        // 초기화를 위한 플레이어 상태 가져옴.
        playerAnimInfo = player.GetComponent<Animator>();
        playerWeaponInfo = player.GetComponent<PlayerWeaponChange>();
        playerBattleInfo = player.GetComponent<PlayerBattle>();

        // 프리펩에 저장된 정보를 가져옴.
        ClickName = PlayerPrefs.GetString("ClickBtn");

        // 플레이어 위치를 입력에 맞게 초기화
        SettingScene(ClickName);

        // 로딩씬 2초후 닫기
        Invoke("DisableLoading", 2f);

        // 플레이어가 죽고 타이밍 맞게 콜백을 받으면 상태를 초기화 하는 함수 실행
        player.GetComponent<PlayerAnim>().diedelegate += DieCallback;
    }

    private void SettingScene(string _ClickBtn)
    {
         if (_ClickBtn == "Test")
        {
            // 훈련장 위치로 플레이어 이동
            player.transform.position = TestPosition;
        }

         if (_ClickBtn == "Start")
        {
            // 보스룸 앞으로 플레이어 이동
            player.transform.position = StartPosition;
        }
    }

    private void DisableLoading()
    {
        loading.SetActive(false);
    }

    private void DieCallback()
    {
        Debug.Log("다이콜 콜백됨");

        // 플레이어 애니메이션 초기화 및 무기레이어와 상태 초기화
        InitAnimationParameter();

        // 플레이어 위치 (루트모션일때 이동 안되는 오류가 있어 잠시 끄고 위치 이동후 킴)
        StartCoroutine(NoRootMotionForDie());

        // 플레이어 체력
        playerBattleInfo.PlayerCurHp = playerBattleInfo.PlayerMaxHp;

        // 카메라 상태 초기화
        followCam.SetActive(true);

        // 보스 정보(애니메이션 상태, 체력) 초기화

        // UI초기화 하는것도 필요한가..?
    }

    // 0.5초간 rootmotion 끄는 함수
    private IEnumerator NoRootMotionForDie()
    {
        playerAnimInfo.applyRootMotion = !playerAnimInfo.applyRootMotion;

        yield return new WaitForSeconds(0.5f);

        player.transform.position = TestPosition;

        yield return new WaitForSeconds(0.5f);

        playerAnimInfo.applyRootMotion = !playerAnimInfo.applyRootMotion;
    }

    // 플레이어 애니메이션 초기화 및 무기레이어와 상태 초기화
    private void InitAnimationParameter()
    {
        playerAnimInfo.SetBool("IsMoving", false);
        playerAnimInfo.SetBool("IsShift", false);
        playerAnimInfo.SetBool("IsBattleMode", false);
        playerAnimInfo.SetBool("IsChange", false);
        playerAnimInfo.SetBool("IsBlocking", false);
        playerAnimInfo.ResetTrigger("SpaceTrigger");
        playerAnimInfo.ResetTrigger("AttackTrigger");
        playerAnimInfo.ResetTrigger("ComboTrigger");
        playerAnimInfo.ResetTrigger("Die");
        playerAnimInfo.ResetTrigger("Combo2Trigger");
        playerAnimInfo.ResetTrigger("ShieldHit");
        playerAnimInfo.SetFloat("WalkSpeed", 0f);
        playerAnimInfo.SetFloat("ComboEvasionX", 0f);
        playerAnimInfo.SetFloat("ComboEvasionZ", 0f);
        playerAnimInfo.SetFloat("HitDirX", 0f);
        playerAnimInfo.SetFloat("HitDirZ", 0f);
        playerAnimInfo.SetInteger("EvasionNum", 0);

        playerAnimInfo.Play("Idle", 0);
        playerWeaponInfo.Init();
        playerWeaponInfo.ChangeSword();
    }
}
