using UnityEngine;

public class HPBar : MonoBehaviour
{
    [Header("HP Bar Settings")]
    [SerializeField] private RectTransform hpBarForeground; // ����׶��� RectTransform
    [SerializeField] private float maxHp = 100f; // ������ �ִ� HP
    private float currentHp; // ���� HP

    private float baseWidth = 1000f; // HP ���� ���� �� (�ִ� HP������ ��)

    private void Start()
    {
        // ���� HP�� �ִ� HP�� �ʱ�ȭ
        currentHp = maxHp;

        // HP�� ������Ʈ
        UpdateHPBar();
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // HP�� 0~�ִ� HP�� ����
        UpdateHPBar();
    }

    public void Heal(float healAmount)
    {
        currentHp += healAmount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // HP�� 0~�ִ� HP�� ����
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        // HP ���� ���
        float hpRatio = currentHp / maxHp;

        // ������ ���� HP ���� �� ����
        hpBarForeground.sizeDelta = new Vector2(baseWidth * hpRatio, hpBarForeground.sizeDelta.y);
    }
}













