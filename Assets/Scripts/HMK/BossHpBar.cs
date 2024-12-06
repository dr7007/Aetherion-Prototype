using UnityEngine;

public class HPBar : MonoBehaviour
{
    [Header("HP Bar Settings")]
    [SerializeField] private RectTransform hpBarForeground; // 포어그라운드 RectTransform
    [SerializeField] private float maxHp = 100f; // 보스의 최대 HP
    private float currentHp; // 현재 HP

    private float baseWidth = 1000f; // HP 바의 고정 폭 (최대 HP에서의 폭)

    private void Start()
    {
        // 현재 HP를 최대 HP로 초기화
        currentHp = maxHp;

        // HP바 업데이트
        UpdateHPBar();
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // HP를 0~최대 HP로 제한
        UpdateHPBar();
    }

    public void Heal(float healAmount)
    {
        currentHp += healAmount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // HP를 0~최대 HP로 제한
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        // HP 비율 계산
        float hpRatio = currentHp / maxHp;

        // 비율에 따라 HP 바의 폭 설정
        hpBarForeground.sizeDelta = new Vector2(baseWidth * hpRatio, hpBarForeground.sizeDelta.y);
    }
}













