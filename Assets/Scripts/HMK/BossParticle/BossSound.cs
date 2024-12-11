using UnityEngine;

public class BossSound : MonoBehaviour
{
    [SerializeField] AudioClip attack;
    
    private void First()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 2f);
    }
    private void FirstDetect()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 2f);
    }
    private void ResponAttack()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 2f);
    }
    private void No1()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 2f);
    }
    private void No2()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 2f);
    }
    private void No3()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 2f);
    }
}
