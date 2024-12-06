using UnityEngine;

public class PointToClosestMonster : MonoBehaviour
{
    public float detectionRadius = 50f;
    public GameObject lightPrefab;
    public float lightDistance = 5f;
    public float lightHeightOffset = 4f;

    private GameObject currentLight;

    void Update()
    {
        GameObject closestMonster = FindClosestMonster();

        if (closestMonster != null)
        {
            Vector3 directionToMonster = (closestMonster.transform.position - transform.position).normalized;

            Vector3 lightPosition = transform.position + directionToMonster * lightDistance;
            lightPosition.y += lightHeightOffset;

            if (currentLight == null)
            {
                currentLight = Instantiate(lightPrefab, lightPosition, Quaternion.identity);
            }
            else
            {
                currentLight.transform.position = lightPosition;
            }

            currentLight.transform.rotation = Quaternion.LookRotation(directionToMonster);
        }
        
        else
        {
            GameObject boseMonster = FindClosestBoseMonster();

            { 
                Vector3 directionToMonster = (closestMonster.transform.position - transform.position).normalized;

                Vector3 lightPosition = transform.position + directionToMonster * lightDistance;
                lightPosition.y += lightHeightOffset;

                if (currentLight == null)
                {
                    currentLight = Instantiate(lightPrefab, lightPosition, Quaternion.identity);
                }
                else
                {
                    currentLight.transform.position = lightPosition;
                }

                currentLight.transform.rotation = Quaternion.LookRotation(directionToMonster);
            }
        }
    }
    GameObject FindClosestBoseMonster()
    {
        GameObject boseMonster = GameObject.FindGameObjectWithTag("boseMonster"); //보스 몬스터 태그 창
        GameObject closestBoseMonster = null;
        float closetBoseDistance = Mathf.Infinity;
        float distance = Vector3.Distance(transform.position, boseMonster.transform.position);
        if (distance < closetBoseDistance && distance <= detectionRadius)
        {
            closetBoseDistance = distance;
            closestBoseMonster = boseMonster;
        }
        return closestBoseMonster;
    }

    GameObject FindClosestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("monster");
        GameObject closestMonster = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject monster in monsters)
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);
            if (distance < closestDistance && distance <= detectionRadius)
            {
                closestDistance = distance;
                closestMonster = monster;
            }
        }

        return closestMonster;
    }
}


