using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIHPBar : MonoBehaviour
{
    [SerializeField] private RectTransform yellowRectTr = null;
    [SerializeField] private Image redImg = null;

    private float maxWidth = 0f;
    private float maxHeight = 0f;

    private void Awake()
    {
        maxWidth = yellowRectTr.sizeDelta.x;
        maxHeight = yellowRectTr.sizeDelta.y;
    }

    // 체력 바를 업데이트 (외부에서 호출)
    public void UpdateHPBar(float _maxHp, float _curHp)
    {
        UpdateHpBar(_curHp / _maxHp);
    }

    // HP 비율에 따라 HP 바 갱신
    public void UpdateHpBar(float _amount)
    {
        float prevWidth = yellowRectTr.sizeDelta.x; // 이전 너비
        float newWidth = maxWidth * _amount;        // 새 너비

        StopAllCoroutines(); // 기존 코루틴 정지

        if (newWidth < prevWidth) // 데미지를 받은 경우
        {
            StartCoroutine(UpdateHpBarCoroutine(prevWidth, newWidth));
            redImg.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, maxHeight); // 빨간색 HP바 변경
        }
        else // 체력이 회복된 경우
        {
            yellowRectTr.sizeDelta = new Vector2(newWidth, maxHeight); // 노란색 HP 바 크기 변경
        }
    }

    // 코루틴: 노란색 HP 바를 서서히 줄이기
    private IEnumerator UpdateHpBarCoroutine(float _prevWidth, float _newWidth)
    {
        Vector2 size = new Vector2(_prevWidth, maxHeight);
        yellowRectTr.sizeDelta = size;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            size.x = Mathf.Lerp(_prevWidth, _newWidth, t);
            yellowRectTr.sizeDelta = size; // 갱신된 크기 적용           
            yield return null; // 다음 프레임까지 대기
        }
    }
}







