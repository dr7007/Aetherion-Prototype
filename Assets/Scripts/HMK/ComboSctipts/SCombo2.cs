using UnityEngine;

public class SCombo2 : MonoBehaviour
{
    [SerializeField] private Animator animator; // Animator 컴포넌트
    [SerializeField] private GameObject currentPrefab; // 현재 활성화된 프리팹
    [SerializeField] private GameObject[] combo1img; // 콤보 UI 이미지 배열
    [SerializeField] private GameObject newPrefab; // 새로운 활성화할 프리팹
    [SerializeField] private float timeout = 0.6f; // 다음 콤보로 넘어가는 제한 시간

    private float comboTimer = 0f; // 현재 상태에서 경과 시간
    private int currentComboIndex = -1; // 현재 활성화된 콤보 인덱스 (-1로 초기화)

    public string[] targetAnimationStates = { "Combo1", "ComboB2", "ComboB3" };

    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

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
            ResetState(); // 제한 시간 초과 시 초기화
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

        // 콤보 UI 비활성화
        UpdateComboUI(comboIndex);

        // 마지막 콤보(ComboB3)에서 애니메이션이 종료되었을 때 프리팹 교체
        if (comboIndex == targetAnimationStates.Length - 1 && stateInfo.normalizedTime >= 1f)
        {
            SwitchToNewPrefab();
        }
    }

    private void ResetState()
    {
        comboTimer = 0f; // 타이머 초기화
        currentComboIndex = -1; // 콤보 인덱스 초기화

        // 모든 콤보 이미지를 활성화
        foreach (GameObject img in combo1img)
        {
            if (img != null)
            {
                img.SetActive(true);
            }
        }
    }

    private void UpdateComboUI(int activeIndex)
    {
        // 콤보 UI 상태를 업데이트
        for (int i = 0; i < combo1img.Length; i++)
        {
            if (combo1img[i] != null)
            {
                combo1img[i].SetActive(i != activeIndex); // 활성화된 인덱스만 비활성화
            }
        }
    }

    private void SwitchToNewPrefab()
    {
        // 현재 프리팹 비활성화
        if (currentPrefab != null)
        {
            currentPrefab.SetActive(false);
        }

        // 새로운 프리팹 활성화
        if (newPrefab != null)
        {
            newPrefab.SetActive(true);
        }
    }
}




