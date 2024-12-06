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

    // ü�� �ٸ� ������Ʈ (�ܺο��� ȣ��)
    public void UpdateHPBar(float _maxHp, float _curHp)
    {
        UpdateHpBar(_curHp / _maxHp);
    }

    // HP ������ ���� HP �� ����
    public void UpdateHpBar(float _amount)
    {
        float prevWidth = yellowRectTr.sizeDelta.x; // ���� �ʺ�
        float newWidth = maxWidth * _amount;        // �� �ʺ�

        StopAllCoroutines(); // ���� �ڷ�ƾ ����

        if (newWidth < prevWidth) // �������� ���� ���
        {
            StartCoroutine(UpdateHpBarCoroutine(prevWidth, newWidth));
            redImg.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, maxHeight); // ������ HP�� ����
        }
        else // ü���� ȸ���� ���
        {
            yellowRectTr.sizeDelta = new Vector2(newWidth, maxHeight); // ����� HP �� ũ�� ����
        }
    }

    // �ڷ�ƾ: ����� HP �ٸ� ������ ���̱�
    private IEnumerator UpdateHpBarCoroutine(float _prevWidth, float _newWidth)
    {
        Vector2 size = new Vector2(_prevWidth, maxHeight);
        yellowRectTr.sizeDelta = size;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            size.x = Mathf.Lerp(_prevWidth, _newWidth, t);
            yellowRectTr.sizeDelta = size; // ���ŵ� ũ�� ����           
            yield return null; // ���� �����ӱ��� ���
        }
    }
}







