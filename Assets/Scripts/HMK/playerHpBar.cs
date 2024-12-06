using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class PlayerHPBar : MonoBehaviour
{
    [Header("Boss Reference")]
    [SerializeField] private PlayerBattle player; // BossMonsterAI ����

    [Header("UI Settings")]
    [SerializeField] private Image hpimg; // HP �� �̹���
    [SerializeField] private float maxWidth = 1000f; // HP �� �ִ� �ʺ�

    private void Start()
    {
        player.HpChangedCallback += UpdateHPBar;
    }



    private void UpdateHPBar(float currentHP, float maxHP)
    {
        Debug.Log("���� ������  HPBar �� �Ѿ�� :" + currentHP);

        if (hpimg != null)
        {
            float fillWidth = (currentHP / maxHP) * maxWidth; // ���� ���
            fillWidth = Mathf.Clamp(fillWidth, 0, maxWidth); // �ּ�/�ִ밪 ����
            hpimg.rectTransform.sizeDelta = new Vector2(fillWidth, hpimg.rectTransform.sizeDelta.y);
        }
    }
}
