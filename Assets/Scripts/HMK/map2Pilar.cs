using System;
using UnityEngine;

public class PillarManager : MonoBehaviour
{
    public GameObject[] pillars; // ��� �迭
    public Action OnAllPillarsDestroyed; // ��� ����� ������ �� ȣ��Ǵ� �ݹ�

    private int remainingPillars;

    private void Start()
    {
        remainingPillars = pillars.Length;
        Debug.Log($"���� ��� ��: {remainingPillars}");

        foreach (GameObject pillar in pillars)
        {
            if (pillar != null)
            {
                Debug.Log($"��� ���: {pillar.name}");
            }
        }
    }


    public void DestroyPillar(GameObject pillar)
    {
        // ��� ����
        for (int i = 0; i < pillars.Length; i++)
        {
            if (pillars[i] == pillar)
            {
                Destroy(pillars[i]);
                pillars[i] = null; // �迭���� ����
                remainingPillars--;

                // ��� ����� �������� �ݹ� ȣ��
                if (remainingPillars <= 0)
                {
                    OnAllPillarsDestroyed?.Invoke();
                }
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Monster �±׸� ���� ������Ʈ�� �浹���� ��
        if (other.CompareTag("Player"))
        {
            Debug.Log("��հ� �浹");
            foreach (GameObject pillar in pillars)
            {
                if (pillar != null && other.gameObject == pillar)
                {
                    DestroyPillar(pillar);
                    break;
                }
            }
        }
    }
}



