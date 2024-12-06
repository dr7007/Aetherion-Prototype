using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFadeOut : MonoBehaviour
{
    [SerializeField] private Image image; // ���̵� �ƿ� ��� �̹���

    private float fadeDuration = 3f; // 3�� ���� ���̵� �ƿ�

    public void StartFadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        Color startColor = image.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(startColor.a, 0, t / fadeDuration);
            image.color = newColor;
            yield return null;
        }

        Color finalColor = image.color;
        finalColor.a = 0;
        image.color = finalColor; // ���� ���� ����
    }
}

