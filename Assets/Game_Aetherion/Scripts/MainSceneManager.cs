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
        // �ʱ�ȭ�� ���� �÷��̾� ���� ������.
        playerAnimInfo = player.GetComponent<Animator>();
        playerWeaponInfo = player.GetComponent<PlayerWeaponChange>();
        playerBattleInfo = player.GetComponent<PlayerBattle>();

        // �����鿡 ����� ������ ������.
        ClickName = PlayerPrefs.GetString("ClickBtn");

        // �÷��̾� ��ġ�� �Է¿� �°� �ʱ�ȭ
        SettingScene(ClickName);

        // �ε��� 2���� �ݱ�
        Invoke("DisableLoading", 2f);

        // �÷��̾ �װ� Ÿ�̹� �°� �ݹ��� ������ ���¸� �ʱ�ȭ �ϴ� �Լ� ����
        player.GetComponent<PlayerAnim>().diedelegate += DieCallback;
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

        // �÷��̾� ��ġ (��Ʈ����϶� �̵� �ȵǴ� ������ �־� ��� ���� ��ġ �̵��� Ŵ)
        StartCoroutine(NoRootMotionForDie());

        // �÷��̾� ü��
        playerBattleInfo.PlayerCurHp = playerBattleInfo.PlayerMaxHp;

        // ī�޶� ���� �ʱ�ȭ
        followCam.SetActive(true);

        // ���� ����(�ִϸ��̼� ����, ü��) �ʱ�ȭ

        // UI�ʱ�ȭ �ϴ°͵� �ʿ��Ѱ�..?
    }

    // 0.5�ʰ� rootmotion ���� �Լ�
    private IEnumerator NoRootMotionForDie()
    {
        playerAnimInfo.applyRootMotion = !playerAnimInfo.applyRootMotion;

        yield return new WaitForSeconds(0.5f);

        player.transform.position = TestPosition;

        yield return new WaitForSeconds(0.5f);

        playerAnimInfo.applyRootMotion = !playerAnimInfo.applyRootMotion;
    }

    // �÷��̾� �ִϸ��̼� �ʱ�ȭ �� ���ⷹ�̾�� ���� �ʱ�ȭ
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
