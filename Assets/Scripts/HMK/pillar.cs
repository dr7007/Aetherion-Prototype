using UnityEngine;

public class CirclePillarSpawner : MonoBehaviour
{
    public GameObject pillarPrefab; // 기둥 프리팹
    public GameObject blockPrefab; // 블록 프리팹 (사이 간격에 생성)
    public int numberOfPillars = 10; // 기둥 개수
    public float radius = 10f; // 원의 반지름
    public Transform centerObject; // 원의 중심

    void Start()
    {
        SpawnPillarsAndBlocks();
    }

    void SpawnPillarsAndBlocks()
    {
        // 중심 위치
        Vector3 center = centerObject != null ? centerObject.position : Vector3.zero;

        for (int i = 0; i < numberOfPillars; i++)
        {
            // 현재 기둥의 각도
            float angle = i * Mathf.PI * 2 / numberOfPillars;

            // 다음 기둥의 각도
            float nextAngle = (i + 1) * Mathf.PI * 2 / numberOfPillars;

            // 현재 기둥 위치 계산
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 pillarPosition = new Vector3(x, 0, z) + center;

            // 기둥 생성
            Instantiate(pillarPrefab, pillarPosition, Quaternion.identity);

            // 블록(중간 지점) 위치 계산
            float midAngle = (angle + nextAngle) / 2; // 두 각도의 중간값
            float midX = Mathf.Cos(midAngle) * radius;
            float midZ = Mathf.Sin(midAngle) * radius;
            Vector3 blockPosition = new Vector3(midX, 0, midZ) + center;

            // 블록 회전 계산 (중심 방향을 향함)
            Vector3 direction = pillarPosition - blockPosition; // 기둥 쪽으로 향하는 방향
            Quaternion blockRotation = Quaternion.LookRotation(direction);

            // 블록 생성 (회전 값 적용)
            Instantiate(blockPrefab, blockPosition, blockRotation);
        }
    }
}




