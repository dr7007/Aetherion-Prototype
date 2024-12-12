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
        // �ʱ�ȭ�� ���� �÷��̾� ���� ������.
        playerAnimInfo = player.GetComponent<Animator>();
        playerWeaponInfo = player.GetComponent<PlayerWeaponChange>();
        playerBattleInfo = player.GetComponent<PlayerBattle>();

        // ���߿� ���������� �ݹ��������.
        boss2die.bossDieCallback += BossDie;

        // �����鿡 ����� ������ ������.
        ClickName = PlayerPrefs.GetString("ClickBtn");

        // �÷��̾� ��ġ�� �Է¿� �°� �ʱ�ȭ
        SettingScene(ClickName);

        loading.SetActive(true);

        // �ε��� 2���� �ݱ�
        Invoke("DisableLoading", 2f);

        // �÷��̾ �װ� Ÿ�̹� �°� �ݹ��� ������ ���¸� �ʱ�ȭ �ϴ� �Լ� ����
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
            // �Ʒ��� ��ġ�� �÷��̾� �̵�
            player.transform.position = TestPosition;
        }

         if (_ClickBtn == "Start")
        {
            // ������ ������ �÷��̾� �̵�
            player.transform.position = StartPosition;
        }
    }

    private void DisableLoading()
    {
        loading.SetActive(false);
    }

    private void DieCallback()
    {
        Debug.Log("������ �ݹ��");

        // �÷��̾� �ִϸ��̼� �ʱ�ȭ �� ���ⷹ�̾�� ���� �ʱ�ȭ
        InitAnimationParameter();

        // �÷��̾� ��ġ (ĳ���� ��Ʈ�ѷ��� �浹�� ������ �̵� �ȵǴ� ������ �־� ��� ���� ��ġ �̵��� Ŵ)
        StartCoroutine(NoRootMotionForDie());

        // �÷��̾� ü��
        playerBattleInfo.PlayerCurHp = playerBattleInfo.PlayerMaxHp;

        // ī�޶� ���� �ʱ�ȭ
        followCam.SetActive(true);

        // ���� ����(�ִϸ��̼� ����, ü��) �ʱ�ȭ - ������ 1�ϋ��� 2�ϋ��� �ٸ�.
        if (!phase2)
        {
            // ü�� �ʱ�ȭ �ϴ� �ڵ�
            InitBoss1AnimationParameter();
            boss.transform.position = BossStartPosition;
        }
        else
        {
            // ü�� �ʱ�ȭ �ϴ� �ڵ�?
            // ��� �ʱ�ȭ �ϴ� �ڵ�?
            InitBoss2AnimationParameter();
            boss.transform.position = Phase2Boss;
        }

        // UI�ʱ�ȭ �ϴ°͵� �ʿ��Ѱ�..?
    }

    // 0.5�ʰ� ĳ���� �ݶ��̴� ���� �Լ�
    private IEnumerator NoRootMotionForDie()
    {
        player.GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(0.5f);

        player.transform.position = StartPosition;

        player.GetComponent<CharacterController>().enabled = true;
        yield return new WaitForSeconds(0.5f);
    }

    // �÷��̾� �ִϸ��̼� �ʱ�ȭ �� ���ⷹ�̾�� ���� �ʱ�ȭ
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

        // ��� ����
        Audio1.SetActive(false);
        phase2 = true;

        //UI ����
        CanvasUI.SetActive(false);

        // �ٷ� ���̵� �ƿ� setActive �ϰ�
        fadeOut.SetActive(true);

        // �÷��̾�, ī�޶�, ���� ��Ȱ��ȭ
        player.SetActive(false);
        boss.SetActive(false);
        followCam.SetActive(false);

        // phase������ ķ�� Ȱ��ȭ
        phaseBoss.SetActive(true);
        phaseCam.SetActive(true);

        // 1������ ����ȭ�� ����
        yield return new WaitForSeconds(1f);

        // ���̵� ��
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

        // trigger on ���Ѽ� ���� �ִϸ��̼� �ϰ�
        PhaseBossAnimator.SetTrigger("Phase");
       
        // 3���� ���̵� �ƿ�
        yield return new WaitForSeconds(6f);

        // ���̵� �ƿ�
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

        // �÷��̾�� ������ �̵���Ű��
        player.transform.position = Phase2Player;
        phaseBoss.transform.position = Phase2Boss;

        // ���� �ٶ󺸴� ī�޶� ��Ȱ��ȭ
        phaseCam.SetActive(false);

        // �÷��̾�, ī�޶�, ���� Ȱ��ȭ
        player.SetActive(true);
        phaseBoss.SetActive(true);
        followCam.SetActive(true);

        if (playerAnimInfo.applyRootMotion == false)
        {
            playerAnimInfo.applyRootMotion = !playerAnimInfo.applyRootMotion;
        }

        yield return new WaitForSeconds(1f);

        // ���̵� �ƿ��Ѱ� ��Ȱ��ȭ
        fadeOut.SetActive(false);

        // UI Ű��
        CanvasUI.SetActive(true);
        
        // ���� ����
        PhaseBossWeapon.SetActive(false);

        // phase2�� �°� ����
        // �÷��̾� ��������
        // ���� �������� (�̰� �˾Ƽ� �Ǿ�����)
        // ��� �ٲٱ�
        StartPosition = Phase2Player;
        Audio2.SetActive(true);

        yield return new WaitForSeconds(1f);

        // ���ʵ� on
        PhaseBossAnimator.SetTrigger("OnField");

    }

    private IEnumerator BossDieScene()
    {
        // �ٽ� ���̵� �ƿ�
        fadeOut2.SetActive(true);

        //�÷��̾�, ī�޶�, ���� ��Ȱ��ȭ
        player.SetActive(false);
        phaseBoss.SetActive(false);
        followCam.SetActive(false);
        CanvasUI.SetActive(false);

        //���� ������, ī�޶� on
        DieBoss.SetActive(true);
        DieCam.SetActive(true);

        // ���̵� ��
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

        // ���� �״� �ִϸ��̼�
        DieBossAnimator.SetTrigger("Die");

        yield return new WaitForSeconds(15f);

        // �̹����� Lerp�� ���İ��� �ø���
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

        // game Clear �޼��� ���
        gameClear.SetActive(true);

        yield return null;
    }

}
