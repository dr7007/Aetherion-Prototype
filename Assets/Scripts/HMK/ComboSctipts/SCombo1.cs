using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Animator animator; // Animator ������Ʈ
    [SerializeField] private GameObject[] combo1img; // �޺� �̹��� �迭
    [SerializeField] private float timeout = 0.6f; // ���� �޺��� �Ѿ�� ���� �ð�

    private float comboTimer = 0f; // ���� ���¿��� ��� �ð�
    private int currentComboIndex = 0; // ���� Ȱ��ȭ�� �޺� �ε���

    public string[] targetAnimationStates = { "Combo1", "Combo2", "Combo3" };

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
            ResetImages(); // ���� �ð� �ʰ� �� �ʱ�ȭ
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

        // ���� �޺� �̹��� ��Ȱ��ȭ
        if (combo1img.Length > comboIndex && combo1img[comboIndex] != null)
        {
            combo1img[comboIndex].SetActive(false);
        }

        // ������ �޺�(Combo3)���� �ִϸ��̼��� ����Ǿ��� �� �ʱ�ȭ
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
                img.SetActive(true); // �̹��� Ȱ��ȭ
        }
        comboTimer = 0f; // Ÿ�̸� �ʱ�ȭ
        currentComboIndex = -1; // �޺� �ε��� �ʱ�ȭ
    }
}
