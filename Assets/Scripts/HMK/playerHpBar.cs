using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class PlayerHPBar : MonoBehaviour
{
    [Header("Boss Reference")]
    [SerializeField] private PlayerBattle player; // BossMonsterAI 참조

    [Header("UI Settings")]
    [SerializeField] private Image hpimg; // HP 바 이미지
    [SerializeField] private float maxWidth = 1000f; // HP 바 최대 너비

    private void Start()
    {
        player.HpChangedCallback += UpdateHPBar;
    }



    private void UpdateHPBar(float currentHP, float maxHP)
    {
        Debug.Log("보스 현재피  HPBar 에 넘어옴 :" + currentHP);

        if (hpimg != null)
        {
            float fillWidth = (currentHP / maxHP) * maxWidth; // 비율 계산
            fillWidth = Mathf.Clamp(fillWidth, 0, maxWidth); // 최소/최대값 제한
            hpimg.rectTransform.sizeDelta = new Vector2(fillWidth, hpimg.rectTransform.sizeDelta.y);
        }
    }
}
