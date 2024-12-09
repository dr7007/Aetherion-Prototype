using UnityEngine;

public class CirclePillarSpawner : MonoBehaviour
{
    public GameObject pillarPrefab; // ��� ������
    public GameObject blockPrefab; // ��� ������ (���� ���ݿ� ����)
    public int numberOfPillars = 10; // ��� ����
    public float radius = 10f; // ���� ������
    public Transform centerObject; // ���� �߽�

    void Start()
    {
        SpawnPillarsAndBlocks();
    }

    void SpawnPillarsAndBlocks()
    {
        // �߽� ��ġ
        Vector3 center = centerObject != null ? centerObject.position : Vector3.zero;

        for (int i = 0; i < numberOfPillars; i++)
        {
            // ���� ����� ����
            float angle = i * Mathf.PI * 2 / numberOfPillars;

            // ���� ����� ����
            float nextAngle = (i + 1) * Mathf.PI * 2 / numberOfPillars;

            // ���� ��� ��ġ ���
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            Vector3 pillarPosition = new Vector3(x, 0, z) + center;

            // ��� ����
            Instantiate(pillarPrefab, pillarPosition, Quaternion.identity);

            // ���(�߰� ����) ��ġ ���
            float midAngle = (angle + nextAngle) / 2; // �� ������ �߰���
            float midX = Mathf.Cos(midAngle) * radius;
            float midZ = Mathf.Sin(midAngle) * radius;
            Vector3 blockPosition = new Vector3(midX, 0, midZ) + center;

            // ��� ȸ�� ��� (�߽� ������ ����)
            Vector3 direction = pillarPosition - blockPosition; // ��� ������ ���ϴ� ����
            Quaternion blockRotation = Quaternion.LookRotation(direction);

            // ��� ���� (ȸ�� �� ����)
            Instantiate(blockPrefab, blockPosition, blockRotation);
        }
    }
}




