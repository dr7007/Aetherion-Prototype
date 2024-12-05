using UnityEngine;

public class TestCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            Invoke("SetColorInit", 1f);
        }
    }

    private void SetColorInit()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
