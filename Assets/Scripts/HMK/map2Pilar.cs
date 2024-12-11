using System;
using UnityEngine;

public class PillarManager : MonoBehaviour
{
    public GameObject[] pillars; // 기둥 배열
    public Action OnAllPillarsDestroyed; // 모든 기둥이 없어질 때 호출되는 콜백

    private int remainingPillars;

    private void Start()
    {
        remainingPillars = pillars.Length;
        Debug.Log($"남은 기둥 수: {remainingPillars}");

        foreach (GameObject pillar in pillars)
        {
            if (pillar != null)
            {
                Debug.Log($"기둥 등록: {pillar.name}");
            }
        }
    }


    public void DestroyPillar(GameObject pillar)
    {
        // 기둥 제거
        for (int i = 0; i < pillars.Length; i++)
        {
            if (pillars[i] == pillar)
            {
                Destroy(pillars[i]);
                pillars[i] = null; // 배열에서 제거
                remainingPillars--;

                // 모든 기둥이 없어지면 콜백 호출
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
        // Monster 태그를 가진 오브젝트가 충돌했을 때
        if (other.CompareTag("Player"))
        {
            Debug.Log("기둥과 충돌");
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



