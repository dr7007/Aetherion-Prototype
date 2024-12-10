using UnityEngine;
using UnityEngine.UI;

public class SCombo2 : MonoBehaviour
{
    [SerializeField] private Animator animator; // Animator 컴포넌트
    [SerializeField] private GameObject[] combo1img; // 콤보 이미지 배열
    [SerializeField] private float timeout = 0.6f; // 다음 콤보로 넘어가는 제한 시간
    [SerializeField] private PlayerWeaponChange weaponChange;

    private float comboTimer = 0f; // 현재 상태에서 경과 시간
    private int currentComboIndex = 0; // 현재 활성화된 콤보 인덱스
    private int curWeaponNum; // 현재 무기 상태

    public string[] targetAnimationStates = { "Combo1", "ComboB2", "ComboB3" };

    private void Update()
    {
        curWeaponNum = weaponChange.curWeaponNum;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(curWeaponNum);

        // 현재 활성화된 애니메이션 상태 확인
        for (int i = 0; i < targetAnimationStates.Length; i++)
        {
            if (stateInfo.IsName(targetAnimationStates[i]))
            {
                HandleComboState(i, stateInfo);
                return; // 현재 상태가 확인되면 더 이상 검사하지 않음
            }
        }

        // 현재 활성 상태가 아니면 타이머 증가
        comboTimer += Time.deltaTime;
        if (comboTimer > timeout)
        {
            ResetImages(); // 제한 시간 초과 시 초기화
        }
    }

    private void HandleComboState(int comboIndex, AnimatorStateInfo stateInfo)
    {
        // 새로운 상태로 전환되었을 때
        if (currentComboIndex != comboIndex)
        {
            currentComboIndex = comboIndex;
            comboTimer = 0f; // 타이머 초기화
        }

        // 현재 콤보 이미지 비활성화
        if (combo1img.Length > comboIndex && combo1img[comboIndex] != null)
        {
            combo1img[comboIndex].SetActive(false);
        }

        // 마지막 콤보(Combo3)에서 애니메이션이 종료되었을 때 초기화
        if (comboIndex == targetAnimationStates.Length - 1 && stateInfo.normalizedTime >= 1f)
        {
            ResetImages();
        }
    }

    private void ResetImages()
    {
        foreach (GameObject img in combo1img)
        {
            if (img != null)
                img.SetActive(true); // 이미지 활성화
        }
        comboTimer = 0f; // 타이머 초기화
        currentComboIndex = -1; // 콤보 인덱스 초기화
    }
}




