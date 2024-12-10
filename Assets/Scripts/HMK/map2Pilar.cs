using UnityEngine;

public class PillarManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pillars; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            for (int i = 0; i < pillars.Length; i++)
            {
                if (pillars[i] != null) 
                {
                    Collider pillarCollider = pillars[i].GetComponent<Collider>();
                    if (pillarCollider != null && pillarCollider.bounds.Intersects(other.bounds))
                    {
                        DestroyPillar(i);
                        break; 
                    }
                }
            }
        }
    }

    private void DestroyPillar(int index)
    {
        // ±âµÕ ÆÄ±«
        Destroy(pillars[index]);
        pillars[index] = null; 
    }
}


