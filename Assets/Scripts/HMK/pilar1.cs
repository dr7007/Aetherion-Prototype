using UnityEngine;

public class Pillar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // ±âµÕ ¿ÀºêÁ§Æ® ÆÄ±«
            Debug.Log($"{gameObject.name}°¡{other.name}¶û Ãæµ¹");
        }
    }
}

