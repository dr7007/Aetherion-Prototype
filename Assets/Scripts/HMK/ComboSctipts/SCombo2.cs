using UnityEngine;

public class SCombo2 : MonoBehaviour
{
    [SerializeField] private Animator animator; // Animator ������Ʈ
    [SerializeField] private GameObject currentPrefab; // ���� Ȱ��ȭ�� ������
    [SerializeField] private GameObject[] combo1img; // �޺� UI �̹��� �迭
    [SerializeField] private GameObject newPrefab; // ���ο� Ȱ��ȭ�� ������
    [SerializeField] private float timeout = 0.6f; // ���� �޺��� �Ѿ�� ���� �ð�

    private float comboTimer = 0f; // ���� ���¿��� ��� �ð�
    private int currentComboIndex = -1; // ���� Ȱ��ȭ�� �޺� �ε��� (-1�� �ʱ�ȭ)

    public string[] targetAnimationStates = { "Combo1", "ComboB2", "ComboB3" };

    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // ���� Ȱ��ȭ�� �ִϸ��̼� ���� Ȯ��
        for (int i = 0; i < targetAnimationStates.Length; i++)
        {
            if (stateInfo.IsName(targetAnimationStates[i]))
            {
                HandleComboState(i, stateInfo);
                return; // ���� ���°� Ȯ�εǸ� �� �̻� �˻����� ����
            }
        }

        // ���� Ȱ�� ���°� �ƴϸ� Ÿ�̸� ����
        comboTimer += Time.deltaTime;
        if (comboTimer > timeout)
        {
            ResetState(); // ���� �ð� �ʰ� �� �ʱ�ȭ
        }
    }

    private void HandleComboState(int comboIndex, AnimatorStateInfo stateInfo)
    {
        // ���ο� ���·� ��ȯ�Ǿ��� ��
        if (currentComboIndex != comboIndex)
        {
            currentComboIndex = comboIndex;
            comboTimer = 0f; // Ÿ�̸� �ʱ�ȭ
        }

        // �޺� UI ��Ȱ��ȭ
        UpdateComboUI(comboIndex);

        // ������ �޺�(ComboB3)���� �ִϸ��̼��� ����Ǿ��� �� ������ ��ü
        if (comboIndex == targetAnimationStates.Length - 1 && stateInfo.normalizedTime >= 1f)
        {
            SwitchToNewPrefab();
        }
    }

    private void ResetState()
    {
        comboTimer = 0f; // Ÿ�̸� �ʱ�ȭ
        currentComboIndex = -1; // �޺� �ε��� �ʱ�ȭ

        // ��� �޺� �̹����� Ȱ��ȭ
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
        // �޺� UI ���¸� ������Ʈ
        for (int i = 0; i < combo1img.Length; i++)
        {
            if (combo1img[i] != null)
            {
                combo1img[i].SetActive(i != activeIndex); // Ȱ��ȭ�� �ε����� ��Ȱ��ȭ
            }
        }
    }

    private void SwitchToNewPrefab()
    {
        // ���� ������ ��Ȱ��ȭ
        if (currentPrefab != null)
        {
            currentPrefab.SetActive(false);
        }

        // ���ο� ������ Ȱ��ȭ
        if (newPrefab != null)
        {
            newPrefab.SetActive(true);
        }
    }
}




