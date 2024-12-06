using UnityEngine;

public class TestCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger ¹ß»ý");

        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            Invoke("SetColorInit", 1f);
        }
    }
    

    private void SetColorInit()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
    }
}
