using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class HPBar : MonoBehaviour
{
    [Header("Boss Reference")]
    [SerializeField] private BossMonsterAI boss; // BossMonsterAI ����

    [Header("UI Settings")]
    [SerializeField] private Image hpimg; // HP �� �̹���
    [SerializeField] private float maxWidth = 1000f; // HP �� �ִ� �ʺ�

    [Header("bose UI trigger")]
    [SerializeField] private Animator bossUI;

    private const string _DETECT_ANIM_BOOL_NAME = "Detect";
    [SerializeField]
    private GameObject bossHPUI = null;
    private void Start()
    {
        boss.HpChangedCallback += UpdateHPBar;
    }
    private void Update()
    {
        bossUiOn();
    }
    
    private void bossUiOn()
    {
        if(bossUI.GetBool(_DETECT_ANIM_BOOL_NAME))
        {
            bossHPUI.SetActive(true);
        }
        else
        {
            bossHPUI.SetActive(false);
        }
        
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
        if (currentHP <= 0)
        {
            DisableBossUI();
        }

    }
    private void DisableBossUI()
    {
        bossHPUI.SetActive(false);
    }
}
















