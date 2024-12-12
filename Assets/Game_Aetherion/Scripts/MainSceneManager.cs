using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loading = null;
    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject boss = null;
    [SerializeField] private GameObject followCam = null;
    [SerializeField] private Vector3 TestPosition = Vector3.zero;
    [SerializeField] private Vector3 StartPosition = Vector3.zero;
    [SerializeField] private Vector3 BossStartPosition = Vector3.zero;
    [SerializeField] private Vector3 Phase2Player = Vector3.zero;
    [SerializeField] private Vector3 Phase2Boss = Vector3.zero;
    [SerializeField] private Material BossHit = null;

    [SerializeField] private Animator BossAnimator = null;
    [SerializeField] private Animator PhaseBossAnimator = null;
    [SerializeField] private Animator DieBossAnimator = null;
    [SerializeField] private GameObject fadeOut = null;
    [SerializeField] private GameObject fadeOut2 = null;
    [SerializeField] private GameObject fadeWhite = null;
    [SerializeField] private GameObject gameClear = null;
    [SerializeField] private GameObject phaseBoss = null;
    [SerializeField] private GameObject phaseCam = null;

    [SerializeField] private GameObject CanvasUI = null;
    [SerializeField] private GameObject PhaseBossWeapon = null;
    [SerializeField] private Boss2PhaseAI boss2die = null;
    [SerializeField] private GameObject Audio1 = null;
    [SerializeField] private GameObject Audio2 = null;

    [SerializeField] private GameObject DieBoss = null;
    [SerializeField] private GameObject DieCam = null;

    private string ClickName = null;
    private Animator playerAnimInfo;
    private PlayerWeaponChange playerWeaponInfo;
    private PlayerBattle playerBattleInfo;
    private bool coroutineStart = false;
    private bool phase2 = false;

    private void Start()
    {
        BossHit.DisableKeyword("_EMISSION");
        // 초기화를 위한 플레이어 상태 가져옴.
        playerAnimInfo = player.GetComponent<Animator>();
        playerWeaponInfo = player.GetComponent<PlayerWeaponChange>();
        playerBattleInfo = player.GetComponent<PlayerBattle>();

        // 나중에 보스죽으면 콜백받을꺼임.
        boss2die.bossDieCallback += BossDie;

        // 프리펩에 저장된 정보를 가져옴.
        ClickName = PlayerPrefs.GetString("ClickBtn");

        // 플레이어 위치를 입력에 맞게 초기화
        SettingScene(ClickName);

        loading.SetActive(true);

        // 로딩씬 2초후 닫기
        Invoke("DisableLoading", 2f);

        // 플레이어가 죽고 타이밍 맞게 콜백을 받으면 상태를 초기화 하는 함수 실행
        player.GetComponent<PlayerAnim>().diedelegate += DieCallback;
    }

    private void Update()
    {
        if (BossAnimator.GetBool("Die") == true)
        {
            if (!coroutineStart)
            {
                StartCoroutine(PhaseChange());
            }

            coroutineStart = true;
        }
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

        // 플레이어 위치 (캐릭터 컨트롤러와 충돌이 있을때 이동 안되는 오류가 있어 잠시 끄고 위치 이동후 킴)
        StartCoroutine(NoRootMotionForDie());

        // 플레이어 체력
        playerBattleInfo.PlayerCurHp = playerBattleInfo.PlayerMaxHp;

        // 카메라 상태 초기화
        followCam.SetActive(true);

        // 보스 정보(애니메이션 상태, 체력) 초기화 - 페이즈 1일떄와 2일떄와 다름.
        if (!phase2)
        {
            // 체력 초기화 하는 코드
            InitBoss1AnimationParameter();
            boss.transform.position = BossStartPosition;
        }
        else
        {
            // 체력 초기화 하는 코드?
            // 기둥 초기화 하는 코드?
            InitBoss2AnimationParameter();
            boss.transform.position = Phase2Boss;
        }

        // UI초기화 하는것도 필요한가..?
    }

    // 0.5초간 캐릭터 콜라이더 끄는 함수
    private IEnumerator NoRootMotionForDie()
    {
        player.GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(0.5f);

        player.transform.position = StartPosition;

        player.GetComponent<CharacterController>().enabled = true;
        yield return new WaitForSeconds(0.5f);
    }

    // 플레이어 애니메이션 초기화 및 무기레이어와 상태 초기화
    private void InitAnimationParameter()
    {
        playerAnimInfo.SetBool("IsMoving", false);
        playerAnimInfo.SetBool("IsShift", false);
        playerAnimInfo.SetBool("IsBattleMode", false);
        playerAnimInfo.SetBool("IsChange", false);
        playerAnimInfo.SetBool("IsBlocking", false);
        playerAnimInfo.SetBool("DieChk", false);
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

    private void InitBoss1AnimationParameter()
    {
        BossAnimator.SetBool("FirstDetect", true);
        BossAnimator.SetBool("Detect", false);
        BossAnimator.SetBool("JumpAtk", false);
        BossAnimator.SetBool("2ndPhase", false);
        BossAnimator.SetBool("nowheal", false);
        BossAnimator.ResetTrigger("Judge");
        BossAnimator.ResetTrigger("TpJumpAtk");
        BossAnimator.ResetTrigger("DefaultAtk");
        BossAnimator.ResetTrigger("RespAtk");
        BossAnimator.ResetTrigger("OnGuard");
        BossAnimator.ResetTrigger("Critical");
        BossAnimator.ResetTrigger("IsStun");
        BossAnimator.ResetTrigger("isGBreak");
        BossAnimator.ResetTrigger("Die");
        BossAnimator.ResetTrigger("healbane");
        BossAnimator.SetInteger("RangeLevel", 0);
        BossAnimator.Play("Peaceful");
    }

    private void InitBoss2AnimationParameter()
    {

    }

    private void BossDie()
    {
        StartCoroutine("BossDieScene");
    }

    private IEnumerator PhaseChange()
    {
        //
        followCam.GetComponent<CinemachineDecollider>().enabled = false;

        // 브금 끄기
        Audio1.SetActive(false);
        phase2 = true;

        //UI 끄고
        CanvasUI.SetActive(false);

        // 바로 페이드 아웃 setActive 하고
        fadeOut.SetActive(true);

        // 플레이어, 카메라, 보스 비활성화
        player.SetActive(false);
        boss.SetActive(false);
        followCam.SetActive(false);

        // phase보스와 캠을 활성화
        phaseBoss.SetActive(true);
        phaseCam.SetActive(true);

        // 1초정도 검정화면 지속
        yield return new WaitForSeconds(1f);

        // 페이드 인
        Image fadeOutImg = fadeOut.GetComponent<Image>();

        float elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            Color color = fadeOutImg.color;
            color.a = 1f - (elapsedTime / 1.5f);
            fadeOutImg.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // trigger on 시켜서 보스 애니메이션 하고
        PhaseBossAnimator.SetTrigger("Phase");
       
        // 3초후 페이드 아웃
        yield return new WaitForSeconds(6f);

        // 페이드 아웃
        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            Color color = fadeOutImg.color;
            color.a = elapsedTime / 1f;
            fadeOutImg.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Color colorB = fadeOutImg.color;
        colorB.a = 255;
        fadeOutImg.color = colorB;

        // 플레이어와 보스를 이동시키고
        player.transform.position = Phase2Player;
        phaseBoss.transform.position = Phase2Boss;

        // 보스 바라보는 카메라만 비활성화
        phaseCam.SetActive(false);

        // 플레이어, 카메라, 보스 활성화
        player.SetActive(true);
        phaseBoss.SetActive(true);
        followCam.SetActive(true);

        if (playerAnimInfo.applyRootMotion == false)
        {
            playerAnimInfo.applyRootMotion = !playerAnimInfo.applyRootMotion;
        }

        yield return new WaitForSeconds(1f);

        // 페이드 아웃한거 비활성화
        fadeOut.SetActive(false);

        // UI 키고
        CanvasUI.SetActive(true);
        
        // 무기 때고
        PhaseBossWeapon.SetActive(false);

        // phase2에 맞게 수정
        // 플레이어 시작지점
        // 보스 시작지점 (이건 알아서 되어있음)
        // 브금 바꾸기
        StartPosition = Phase2Player;
        Audio2.SetActive(true);

        yield return new WaitForSeconds(1f);

        // 온필드 on
        PhaseBossAnimator.SetTrigger("OnField");

    }

    private IEnumerator BossDieScene()
    {
        // 다시 페이드 아웃
        fadeOut2.SetActive(true);

        //플레이어, 카메라, 보스 비활성화
        player.SetActive(false);
        phaseBoss.SetActive(false);
        followCam.SetActive(false);
        CanvasUI.SetActive(false);

        //다이 프리펩, 카메라 on
        DieBoss.SetActive(true);
        DieCam.SetActive(true);

        // 페이드 인
        Image fadeOutImg = fadeOut2.GetComponent<Image>();

        float elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            Color color = fadeOutImg.color;
            color.a = 1f - (elapsedTime / 1.5f);
            fadeOutImg.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 보스 죽는 애니메이션
        DieBossAnimator.SetTrigger("Die");

        yield return new WaitForSeconds(15f);

        // 이미지의 Lerp로 알파값을 올리고
        Image fadeWhiteImg = fadeWhite.GetComponent<Image>();

        elapsedTime = 0f;
        while (elapsedTime < 1.5f)
        {
            Color color = fadeWhiteImg.color;
            color.a = (elapsedTime / 1.5f);
            fadeWhiteImg.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // game Clear 메세지 출력
        gameClear.SetActive(true);

        yield return null;
    }

}
